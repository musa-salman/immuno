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
        int height = 600;
        Rect boxRect = new Rect(
            (Screen.width - width) / 2,
            (Screen.height - height) / 2,
            width,
            height
        );

        GUI.Box(boxRect, "CHEAT MENU", headerStyle);

        float y = boxRect.y + 40;
        float buttonHeight = 45;
        float spacing = 8;

        GUI.Label(new Rect(boxRect.x + 20, y, width - 40, 30), "<b>CHEAT TOGGLES</b>", headerStyle);
        y += 30 + spacing;

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
        y += buttonHeight + spacing * 2;

        GUI.Label(new Rect(boxRect.x + 20, y, width - 40, 30), "<b>TELEPORT TO LOCATION</b>", headerStyle);
        y += 30 + spacing;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), "⤳ Start Area", buttonStyle))
            TeleportTo("Teleport_Start");
        y += buttonHeight + spacing;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), "⤳ Boss Room", buttonStyle))
            TeleportTo("Teleport_Boss");
        y += buttonHeight + spacing;

        if (GUI.Button(new Rect(boxRect.x + 20, y, width - 40, buttonHeight), "⤳ Exit Portal", buttonStyle))
            TeleportTo("Teleport_Exit");
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

        foreach (var skill in SkillManager.Instance.GetAllSkills())
            skill.level = skill.maxLevel;
    }


    private void TeleportTo(string locationName)
    {
        GameObject target = GameObject.Find(locationName);
        if (target != null && player != null)
        {
            player.transform.position = target.transform.position;
            Debug.Log($"Teleported to {locationName}.");
        }
        else
        {
            Debug.LogWarning($"Teleport failed. Missing: {locationName} or player.");
        }
    }
}
#endif
