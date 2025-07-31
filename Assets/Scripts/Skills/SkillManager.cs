using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    public enum SkillType
    {
        ToughenShell,
        Speed,
        DoubleJump,
        DashCooldownReduction,
        HealthRegenerationRate,
        ProjectilePower,
        AttackSpeed,
        RegenerationDelayReduction,
    }

    [Serializable]
    public class SkillData
    {
        public string name;
        public string description;
        public SkillType skillType;
        public int level;
        public int maxLevel;
        public int[] costPerLevel;
        [HideInInspector] public Skill ui;
        public Func<int, float, float> computeEffectiveLevel;
        public bool active;
    }


    private SkillsContainer skillsContainer;
    public GameObject skillUIPrefab;

    private readonly Dictionary<SkillType, SkillData> skillDict = new();
    private readonly Dictionary<SkillType, float> tempModifiers = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AddSkill("Iron Plating", "Increase defense with hardened cell walls.", SkillType.ToughenShell, new int[] { 100, 200, 400, 800, 1200, 2000 },
            (baseLevel, multiplier) => (1 + baseLevel) * multiplier);

        AddSkill("Cytoblast Surge", "Move faster to evade threats.", SkillType.Speed, new int[] { 100, 200, 300, 400, 600 },
            (baseLevel, multiplier) => (1 + baseLevel) * multiplier);

        AddSkill("Leaping Division", "Gain extra jumps for aerial mobility.", SkillType.DoubleJump, new int[] { 100, 400, 1000 },
            (baseLevel, multiplier) => (2 + baseLevel) * multiplier);

        AddSkill("Histamine Reflex", "Reduces cooldown between dashes.", SkillType.DashCooldownReduction, new int[] { 100, 300, 600, 900 },
(baseLevel, multiplier) => Mathf.Max(0.1f, 0.5f - 0.1f * baseLevel * multiplier));


        AddSkill("Cellular Renewal", "Regenerate health over time.", SkillType.HealthRegenerationRate, new int[] { 100, 400, 800, 1600 },
            (baseLevel, multiplier) => (baseLevel + 1) * 0.2f * multiplier);

        AddSkill("Primed Recovery", "Reduces delay before health regeneration begins.", SkillType.RegenerationDelayReduction, new int[] { 100, 300, 600, 900, 1200 },
(baseLevel, multiplier) => Mathf.Max(0f, 10f - (baseLevel * 1f * multiplier)));

        AddSkill("Enzyme Spike", "Enhance projectile attack damage.", SkillType.ProjectilePower, new int[] { 200, 400, 800, 1600 },
            (baseLevel, multiplier) => 1f + 0.15f * baseLevel * multiplier);

        AddSkill("Rapid Synthesis", "Reduce attack cooldown for faster strikes.", SkillType.AttackSpeed, new int[] { 200, 400, 800, 1600, 2000 },
            (baseLevel, multiplier) => Mathf.Max(0f, 0.8f - 0.2f * baseLevel * multiplier));
    }

    void Start()
    {
        skillsContainer = FindObjectOfType<SkillsContainer>();
    }

    void Update()
    {
        foreach (var skill in skillDict.Values)
        {
            int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
            skill.ui?.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);
        }
    }


    private void AddSkill(string name, string description, SkillType skillType, int[] costPerLevel, Func<int, float, float> computeEffectiveLevel)
    {
        if (skillDict.ContainsKey(skillType))
        {
            Debug.LogWarning($"Skill '{skillType}' already exists.");
            return;
        }

        SkillData newSkill = new()
        {
            name = name,
            description = description,
            skillType = skillType,
            level = 0,
            maxLevel = costPerLevel.Length - 1,
            costPerLevel = costPerLevel,
            computeEffectiveLevel = computeEffectiveLevel,
            active = false
        };

        skillDict[skillType] = newSkill;
    }

    public void EnableSkillUpgrade(SkillType skillType, bool force = false)
    {
        var newSkill = skillDict[skillType];
        if (newSkill.ui != null && newSkill.ui.gameObject.activeInHierarchy && !force)
        {
            Debug.LogWarning($"Skill UI for '{skillType}' already exists.");
            return;
        }

        if (skillsContainer == null)
        {
            skillsContainer = FindObjectOfType<SkillsContainer>();
        }


        GameObject uiObj = Instantiate(skillUIPrefab, skillsContainer.transform);
        uiObj.name = skillType.ToString();

        Skill skillUI = uiObj.GetComponent<Skill>();
        newSkill.ui = skillUI;

        skillUI.SetSkillName(newSkill.name);
        skillUI.SetDescription(newSkill.description);

        Button btn = skillUI.upgradeButton;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => TryUpgradeSkill(skillType));

        int upgradeXpCost = newSkill.level >= newSkill.maxLevel ? 0 : newSkill.costPerLevel[newSkill.level];
        skillUI.SetLevel(newSkill.level, newSkill.maxLevel, upgradeXpCost, ScoreManager.Instance.CurrentScore);

        newSkill.active = true;
    }

    public bool IsSkillActive(SkillType skillName)
    {
        return skillDict.ContainsKey(skillName) && skillDict[skillName].active;
    }

    public void TryUpgradeSkill(SkillType skillName)
    {
        if (!skillDict.ContainsKey(skillName)) return;

        var skill = skillDict[skillName];
        if (skill.level >= skill.maxLevel) return;

        int cost = skill.costPerLevel[skill.level];
        if (ScoreManager.Instance.CurrentScore < cost) return;

        ScoreManager.Instance.AddPoints(-cost);
        skill.level++;

        int requiredXP = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
        skill.ui?.SetLevel(skill.level, skill.maxLevel, requiredXP, ScoreManager.Instance.CurrentScore);
    }

    public float GetEffectiveLevel(SkillType skillName)
    {
        if (!skillDict.TryGetValue(skillName, out var skill)) return -1f;
        int baseLevel = skill.level;
        float multiplier = tempModifiers.ContainsKey(skillName) ? tempModifiers[skillName] : 1f;
        return skill.computeEffectiveLevel != null
            ? skill.computeEffectiveLevel(baseLevel, multiplier)
            : baseLevel * multiplier;
    }

    public IEnumerable<SkillData> GetAllSkills()
    {
        return skillDict.Values;
    }

    public void GenerateUi()
    {
        skillsContainer = FindObjectOfType<SkillsContainer>();

        // find the SkillsContainer in the scene not just in the current GameObject
        foreach (var skill in skillDict.Values)
        {
            if (skill.active)
            {
                EnableSkillUpgrade(skill.skillType, true);
            }
        }
    }

    public void BoostFor(SkillType skillName, float percent, float duration)
    {
        ModifyTemporarily(skillName, 1f + Mathf.Abs(percent), duration);
    }

    public void NerfFor(SkillType skillName, float percent, float duration)
    {
        ModifyTemporarily(skillName, 1f - Mathf.Abs(percent), duration);
    }

    private void ModifyTemporarily(SkillType skillName, float multiplier, float duration)
    {
        if (!skillDict.ContainsKey(skillName)) return;
        StartCoroutine(ApplyModifierTemporarily(skillName, multiplier, duration));
    }

    private IEnumerator ApplyModifierTemporarily(SkillType skillType, float multiplier, float duration)
    {
        if (!tempModifiers.ContainsKey(skillType))
            tempModifiers[skillType] = 1f;

        tempModifiers[skillType] *= multiplier;

        yield return new WaitForSeconds(duration);

        tempModifiers[skillType] /= multiplier;

        if (Mathf.Approximately(tempModifiers[skillType], 1f))
            tempModifiers.Remove(skillType);
    }
}