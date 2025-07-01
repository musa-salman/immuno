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
        Spike,
        ToughenShell,
        Speed,
    }

    [Serializable]
    public class SkillData
    {
        public SkillType skillType;
        public int level;
        public int maxLevel;
        public int[] costPerLevel;
        [HideInInspector] public Skill ui;
        public Func<int, float, float> computeEffectiveLevel;
    }


    [Header("UI References")]
    public Transform skillsContainer;
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
    }

    void Start()
    {
        AddSkill("Spike", SkillType.Spike, new int[] { 100, 150, 200, 250, 300 },
            (baseLevel, multiplier) => (1 + baseLevel) * multiplier);
        AddSkill("Toughen Shell", SkillType.ToughenShell, new int[] { 120, 180, 240, 300 },
            (baseLevel, multiplier) => (1 + baseLevel) * multiplier);
        AddSkill("Speed", SkillType.Speed, new int[] { 80, 110, 160 },
            (baseLevel, multiplier) => (1 + baseLevel) * multiplier);
    }

    void Update()
    {
        foreach (var skill in skillDict.Values)
        {
            int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
            skill.ui?.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);
        }
    }

    public void AddSkill(string name, SkillType skillType, int[] costPerLevel, Func<int, float, float> computeEffectiveLevel)
    {
        if (skillDict.ContainsKey(skillType))
        {
            Debug.LogWarning($"Skill '{skillType}' already exists.");
            return;
        }

        SkillData newSkill = new()
        {
            skillType = skillType,
            level = 0,
            maxLevel = costPerLevel.Length - 1,
            costPerLevel = costPerLevel,
            computeEffectiveLevel = computeEffectiveLevel
        };

        skillDict[skillType] = newSkill;

        GameObject uiObj = Instantiate(skillUIPrefab, skillsContainer);
        uiObj.name = skillType.ToString();

        Skill skillUI = uiObj.GetComponent<Skill>();
        newSkill.ui = skillUI;

        skillUI.SetSkillName(name);

        Button btn = skillUI.upgradeButton;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => TryUpgradeSkill(skillType));

        int upgradeXpCost = newSkill.level >= newSkill.maxLevel ? 0 : costPerLevel[newSkill.level];
        skillUI.SetLevel(newSkill.level, newSkill.maxLevel, upgradeXpCost, ScoreManager.Instance.CurrentScore);
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
        Debug.Log($"Effective level for {skillName}: baseLevel={baseLevel}, multiplier={multiplier}");
        return skill.computeEffectiveLevel != null
            ? skill.computeEffectiveLevel(baseLevel, multiplier)
            : baseLevel * multiplier;
    }

    public IEnumerable<SkillData> GetAllSkills()
    {
        return skillDict.Values;
    }

    public void GenerateUI()
    {
        foreach (var skill in skillDict.Values)
        {
            GameObject uiObj = Instantiate(skillUIPrefab, skillsContainer);
            uiObj.name = skill.skillType.ToString();

            Skill skillUI = uiObj.GetComponent<Skill>();
            skill.ui = skillUI;

            Button btn = skillUI.upgradeButton;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => TryUpgradeSkill(skill.skillType));

            int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
            skillUI.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);
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

    private IEnumerator ApplyModifierTemporarily(SkillType skillName, float multiplier, float duration)
    {
        if (!tempModifiers.ContainsKey(skillName))
            tempModifiers[skillName] = 1f;

        tempModifiers[skillName] *= multiplier;

        yield return new WaitForSeconds(duration);

        tempModifiers[skillName] /= multiplier;

        if (Mathf.Approximately(tempModifiers[skillName], 1f))
            tempModifiers.Remove(skillName);
    }
}