using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [System.Serializable]
    public class SkillData
    {
        public string skillName;
        public int level = 0;
        public int maxLevel = 5;

        public int[] costPerLevel = { 100, 150, 200 };
        public Skill ui;
    }

    public SkillData[] skills;
    private readonly Dictionary<string, SkillData> skillDict = new();
    private readonly Dictionary<string, int> tempModifiers = new();


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var skill in skills)
            {
                skillDict[skill.skillName] = skill;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        foreach (var skill in skills)
        {
            int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
            skill.ui.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);


        }
    }

    public void TryUpgradeSkill(string skillName)
    {
        if (!skillDict.ContainsKey(skillName)) return;
        var skill = skillDict[skillName];

        if (skill.level >= skill.maxLevel) return;

        int cost = skill.costPerLevel[skill.level];
        if (ScoreManager.Instance.CurrentScore < cost) return;

        ScoreManager.Instance.AddPoints(-cost);
        int requiredXP = skill.costPerLevel[skill.level];
        skill.level++;
        if (skill.ui != null)
            skill.ui.SetLevel(skill.level, skill.maxLevel, requiredXP, ScoreManager.Instance.CurrentScore);
    }

    public int GetLevel(string skillName) =>
        skillDict.ContainsKey(skillName) ? skillDict[skillName].level : -1;


    public int GetEffectiveLevel(string skillName)
    {
        if (!skillDict.TryGetValue(skillName, out var skill)) return -1;

        Debug.Log($"Skill: {skillName}, Base Level: {skill.level}, Max Level: {skill.maxLevel}");
        int baseLevel = skill.level;
        int mod = tempModifiers.ContainsKey(skillName) ? tempModifiers[skillName] : 0;

        return Mathf.Clamp(baseLevel + mod, -1, skill.maxLevel);
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


    public void NerfFor(string skillName, int amount, float duration)
    {
        ModifyTemporarily(skillName, -Mathf.Abs(amount), duration);
    }

    public void BoostFor(string skillName, int amount, float duration)
    {
        ModifyTemporarily(skillName, Mathf.Abs(amount), duration);
    }

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



    public void RebindUI()
    {
        foreach (var skill in skills)
        {
            var uiObj = GameObject.FindWithTag(skill.skillName);
            if (uiObj != null && uiObj.TryGetComponent(out Skill skillUI))
            {
                skill.ui = skillUI;
                if (uiObj.TryGetComponent<Button>(out var btn))
                {
                    btn.onClick.RemoveAllListeners();
                    string skillToUpgrade = skill.skillName;
                    btn.onClick.AddListener(() => TryUpgradeSkill(skillToUpgrade));
                }

                int xp = skill.level >= skill.maxLevel ? 0 : skill.costPerLevel[skill.level];
                skill.ui.SetLevel(skill.level, skill.maxLevel, xp, ScoreManager.Instance.CurrentScore);
            }
        }
    }

}
