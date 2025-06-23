using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
