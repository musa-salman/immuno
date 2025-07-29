using UnityEngine;

public class SkillUnlockCollectible : MonoBehaviour
{
    public SkillManager.SkillType unlocksSkill;

    private void Start()
    {
        if (SkillManager.Instance.IsSkillActive(unlocksSkill))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpgradeMenuToggle upgradeMenuToggle = FindObjectOfType<UpgradeMenuToggle>();
            upgradeMenuToggle.ShowMenu();
            SkillManager.Instance.EnableSkillUpgrade(unlocksSkill);
            upgradeMenuToggle.HideMenu();
            Destroy(gameObject);
        }
    }
}
