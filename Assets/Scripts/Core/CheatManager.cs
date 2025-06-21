#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private bool showMenu = false;

    public static bool UndeadMode { get; private set; }
    public static bool OneShotKill { get; private set; }
    public static bool GhostMode { get; private set; }

    private PlayerHealth playerHealth;
    private PlayerMovement player;

    private GUIStyle headerStyle;
    private GUIStyle buttonStyle;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        Debug.Log("CheatManager initialized. Press 'C' to toggle the cheat menu.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            showMenu = !showMenu;
    }

    private void OnGUI()
    {
        if (!showMenu) return;

        InitStyles();

        int width = 400;
        int height = 350;
        Rect boxRect = new Rect(
            (Screen.width - width) / 2,
            (Screen.height - height) / 2,
            width,
            height
        );

        GUI.Box(boxRect, "CHEAT MENU", headerStyle);

        float y = boxRect.y + 40;
        float buttonHeight = 50;
        float spacing = 10;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), $"Undead Mode: {(UndeadMode ? "ON" : "OFF")}", buttonStyle))
            UndeadMode = !UndeadMode;

        y += buttonHeight + spacing;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), $"One Shot Kill: {(OneShotKill ? "ON" : "OFF")}", buttonStyle))
            OneShotKill = !OneShotKill;

        y += buttonHeight + spacing;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), $"Ghost Mode: {(GhostMode ? "ON" : "OFF")}", buttonStyle))
            GhostMode = !GhostMode;

        y += buttonHeight + spacing;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), "Full Power", buttonStyle))
            FullPower();
    }

    private void InitStyles()
    {
        headerStyle ??= new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = 24,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        buttonStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontSize = 18
        };
    }

    private void FullPower()
    {
        playerHealth.ResetStats();

        foreach (var skill in SkillManager.Instance.skills)
            skill.level = skill.maxLevel;
    }
}
#endif
