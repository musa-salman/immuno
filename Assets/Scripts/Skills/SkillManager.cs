using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public int level;
        public int maxLevel;
        public int[] costPerLevel;
        [HideInInspector] public Skill ui;
    }

    [Header("UI References")]
    public Transform skillsContainer;
    public GameObject skillUIPrefab;

    private readonly Dictionary<string, SkillData> skillDict = new();
    private readonly Dictionary<string, int> tempModifiers = new();

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
        AddSkill("Power", 0, 5, new int[] { 100, 150, 200, 250, 300 });
        AddSkill("toughen_shell", 1, 4, new int[] { 120, 180, 240, 300 });
        AddSkill("Speed", 0, 3, new int[] { 80, 110, 160 });
        AddSkill("Speed23", 0, 3, new int[] { 80, 110, 160 });
    }

    void Update()
    {
        foreach (var skill in skillDict.Values)
        {
            int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
            skill.ui?.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);
        }
    }

    public void AddSkill(string skillName, int level, int maxLevel, int[] costPerLevel)
    {
        if (skillDict.ContainsKey(skillName))
        {
            Debug.LogWarning($"Skill '{skillName}' already exists.");
            return;
        }

        SkillData newSkill = new SkillData
        {
            skillName = skillName,
            level = level,
            maxLevel = maxLevel,
            costPerLevel = costPerLevel
        };

        skillDict[skillName] = newSkill;

        GameObject uiObj = Instantiate(skillUIPrefab, skillsContainer);
        uiObj.name = skillName;

        Skill skillUI = uiObj.GetComponent<Skill>();
        newSkill.ui = skillUI;

        skillUI.SetSkillName(skillName);

        Button btn = skillUI.upgradeButton;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => TryUpgradeSkill(skillName));

        int xp = level >= maxLevel ? 0 : costPerLevel[level];
        skillUI.SetLevel(level, maxLevel, xp, ScoreManager.Instance.CurrentScore);
    }

    public void TryUpgradeSkill(string skillName)
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

    public int GetLevel(string skillName) =>
        skillDict.ContainsKey(skillName) ? skillDict[skillName].level : -1;

    public int GetEffectiveLevel(string skillName)
    {
        if (!skillDict.TryGetValue(skillName, out var skill)) return -1;
        int baseLevel = skill.level;
        int mod = tempModifiers.ContainsKey(skillName) ? tempModifiers[skillName] : 0;
        return Mathf.Clamp(baseLevel + mod, 0, skill.maxLevel);
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
            uiObj.name = skill.skillName;

            Skill skillUI = uiObj.GetComponent<Skill>();
            skill.ui = skillUI;

            Button btn = skillUI.upgradeButton;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => TryUpgradeSkill(skill.skillName));

            int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
            skillUI.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);
        }
    }

    public void SetSkillFor(string skillName, int newLevel, float duration)
    {
        if (!skillDict.ContainsKey(skillName)) return;
        int baseLevel = skillDict[skillName].level;
        int currentModifier = tempModifiers.ContainsKey(skillName) ? tempModifiers[skillName] : 0;
        int currentEffective = Mathf.Clamp(baseLevel + currentModifier, 0, skillDict[skillName].maxLevel);
        int diff = newLevel - currentEffective;
        StartCoroutine(ApplyModifierTemporarily(skillName, diff, duration));
    }

    public void BoostFor(string skillName, int amount, float duration) =>
        ModifyTemporarily(skillName, Mathf.Abs(amount), duration);

    public void NerfFor(string skillName, int amount, float duration) =>
        ModifyTemporarily(skillName, -Mathf.Abs(amount), duration);

    private void ModifyTemporarily(string skillName, int modValue, float duration)
    {
        if (!skillDict.ContainsKey(skillName)) return;
        StartCoroutine(ApplyModifierTemporarily(skillName, modValue, duration));
    }

    private IEnumerator ApplyModifierTemporarily(string skillName, int modValue, float duration)
    {
        if (!tempModifiers.ContainsKey(skillName))
            tempModifiers[skillName] = 0;

        tempModifiers[skillName] += modValue;
        yield return new WaitForSeconds(duration);
        tempModifiers[skillName] -= modValue;

        if (tempModifiers[skillName] == 0)
            tempModifiers.Remove(skillName);
    }
}