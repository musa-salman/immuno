#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private string[] sceneNames;
    private int selectedSceneIndex = 0;

    private bool showMenu = false;

    public static bool UndeadMode { get; private set; }
    public static bool OneShotKill { get; private set; }
    public static bool GhostMode { get; private set; }

    public static bool FlyMode { get; private set; }

    public static bool SelfKill { get; private set; } = false;

    private GameObject[] puzzleEntrances;

    private PlayerHealth playerHealth;
    private PlayerMovement player;

    private string scoreInput = "0";

    private GUIStyle headerStyle;
    private GUIStyle buttonStyle;
    private Vector2 scrollPosition;

    private struct CheckpointData
    {
        public string name;
        public Vector3 position;
    }
    private List<CheckpointData> checkpointList = new();


    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();

        puzzleEntrances = GameObject.FindGameObjectsWithTag("Puzzle_Entrance");

        if (puzzleEntrances.Length == 0)
        {
            List<GameObject> found = new();
            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
            {
                if (go.name.StartsWith("Puzzle_Entrance"))
                    found.Add(go);
            }
            puzzleEntrances = found.ToArray();
        }

        sceneNames = GetBuildSceneNames();

        Checkpoint[] foundCheckpoints = FindObjectsOfType<Checkpoint>();

        foreach (Checkpoint cp in foundCheckpoints)
        {
            if (cp != null)
            {
                checkpointList.Add(new CheckpointData
                {
                    name = cp.name,
                    position = cp.transform.position
                });
            }
        }

        Debug.Log("CheatManager initialized. Press 'C' to toggle the cheat menu.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            showMenu = !showMenu;
    }

    private string[] GetBuildSceneNames()
    {
        List<string> names = new();
        foreach (var scene in UnityEditor.EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                names.Add(name);
            }
        }
        return names.ToArray();
    }

    private void OnGUI()
    {
        if (!showMenu) return;

        InitStyles();

        int menuWidth = 400;
        int menuHeight = 750;
        int contentWidth = menuWidth - 40;

        Rect boxRect = new(
            (Screen.width - menuWidth) / 2,
            (Screen.height - menuHeight) / 2,
            menuWidth,
            menuHeight
        );

        GUI.Box(boxRect, "CHEAT MENU", headerStyle);

        float innerY = 0f;
        float buttonHeight = 45f;
        float spacing = 8f;

        float estimatedContentHeight = 2000f; // Oversized on purpose

        scrollPosition = GUI.BeginScrollView(
            new Rect(boxRect.x + 10, boxRect.y + 30, boxRect.width - 20, boxRect.height - 40),
            scrollPosition,
            new Rect(0, 0, contentWidth, estimatedContentHeight)
        );

        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>PLAYER STATE / STATS</b>", headerStyle);
        innerY += 30 + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Heal Player to Full", buttonStyle))
        {
            if (playerHealth != null)
            {
                playerHealth.FullHealth();
                Debug.Log("Player healed to full health.");
            }
        }
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"Undead Mode: {(UndeadMode ? "ON" : "OFF")}", buttonStyle))
            UndeadMode = !UndeadMode;
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"One Shot Kill: {(OneShotKill ? "ON" : "OFF")}", buttonStyle))
            OneShotKill = !OneShotKill;
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"Ghost Mode: {(GhostMode ? "ON" : "OFF")}", buttonStyle))
            GhostMode = !GhostMode;
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"Fly Mode: {(FlyMode ? "ON" : "OFF")}", buttonStyle))
        {
            FlyMode = !FlyMode;
            player.ToggleFlyMode();
        }
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Self-Kill", buttonStyle))
        {
            SelfKill = true;
            playerHealth.TakeDamage(playerHealth.CurrentHealth + 1);
        }
        innerY += buttonHeight + spacing;



        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Full Power", buttonStyle))
            FullPower();
        innerY += buttonHeight + spacing * 2;

        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>TELEPORT TO LOCATION</b>", headerStyle);
        innerY += 30 + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Start Area", buttonStyle))
            TeleportTo("Teleport_Start");
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Boss Room", buttonStyle))
            TeleportTo("Teleport_Boss");
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Exit Portal", buttonStyle))
            TeleportTo("Teleport_Exit");
        innerY += buttonHeight + spacing * 2;

        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>PUZZLE ENTRANCES</b>", headerStyle);
        innerY += 30 + spacing;

        foreach (GameObject entrance in puzzleEntrances)
        {
            if (entrance == null) continue;
            if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"{entrance.name}", buttonStyle))
                TeleportToPuzzleEntrance(entrance);
            innerY += buttonHeight + spacing;
        }

        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>PLAYER COLLECTIBLES</b>", headerStyle);
        innerY += 30 + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Add +5 All Collectibles", buttonStyle))
        {
            if (CollectionsManager.Instance != null)
                CollectionsManager.Instance.AddAllCollectibles(5);
        }

        innerY += buttonHeight + spacing * 2;
        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Reset All Collectibles", buttonStyle))
        {
            if (CollectionsManager.Instance != null)
                CollectionsManager.Instance.ResetAllCollectibles();
        }
        innerY += buttonHeight + spacing * 2;


        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>SCORE CHEATS</b>", headerStyle);
        innerY += 30 + spacing;

        scoreInput = GUI.TextField(new Rect(0, innerY, contentWidth, 30), scoreInput);
        innerY += 30 + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Add Score", buttonStyle))
        {
            if (int.TryParse(scoreInput, out int amount))
                ScoreManager.Instance.AddPoints(amount);
        }
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Subtract Score", buttonStyle))
        {
            if (int.TryParse(scoreInput, out int amount))
                ScoreManager.Instance.AddPoints(-amount);
        }
        innerY += buttonHeight + spacing;

        if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), "Set Score", buttonStyle))
        {
            if (int.TryParse(scoreInput, out int amount))
            {
                ScoreManager.Instance.ResetScore();
                ScoreManager.Instance.AddPoints(amount);
            }
        }
        innerY += buttonHeight + spacing * 2;

        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>SCENE MANAGEMENT</b>", headerStyle);
        innerY += 30 + spacing;

        if (sceneNames != null && sceneNames.Length > 0)
        {
            selectedSceneIndex = GUI.SelectionGrid(
                new Rect(0, innerY, contentWidth, sceneNames.Length * 25),
                selectedSceneIndex,
                sceneNames,
                1
            );
            innerY += (sceneNames.Length * 25) + spacing;

            if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"Load Scene: {sceneNames[selectedSceneIndex]}", buttonStyle))
            {
                string sceneToLoad = sceneNames[selectedSceneIndex];
                Time.timeScale = 1f;
                SceneController.Instance.LoadScene(sceneToLoad);
            }
            innerY += buttonHeight + spacing * 2;
        }
        else
        {
            GUI.Label(new Rect(0, innerY, contentWidth, 30), "No scenes found in Build Settings.", buttonStyle);
            innerY += 30 + spacing * 2;
        }

        GUI.Label(new Rect(0, innerY, contentWidth, 30), "<b>CHECKPOINTS</b>", headerStyle);
        innerY += 30 + spacing;

        int i = 0;
        foreach (var cp in checkpointList)
        {
            if (GUI.Button(new Rect(0, innerY, contentWidth, buttonHeight), $"{cp.name} {i}", buttonStyle))
            {
                if (player != null)
                {
                    player.transform.position = new Vector3(cp.position.x, cp.position.y, player.transform.position.z);
                    Debug.Log($"Teleported to checkpoint: {cp.name}");
                }
            }
            i++;
            innerY += buttonHeight + spacing;
        }


        GUI.EndScrollView();
    }

    private void TeleportToPuzzleEntrance(GameObject entrance)
    {
        if (entrance != null && player != null)
        {
            player.transform.position = new Vector3(entrance.transform.position.x, entrance.transform.position.y, player.transform.position.z);
            Debug.Log($"Teleported to {entrance.name}.");
        }
        else
        {
            Debug.LogWarning($"Teleport failed. Missing entrance or player.");
        }
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
