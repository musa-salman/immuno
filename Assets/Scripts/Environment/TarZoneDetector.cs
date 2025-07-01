using UnityEngine;
using UnityEngine.Tilemaps;

public class TarZoneDetector : MonoBehaviour
{
    public Tilemap tarTilemap;
    private Transform playerTransform;
    public string speedSkillName = "surge_motion";
    public float slowDown = 0.59f;
    public float slowDuration = 2f;

    private bool inTar = false;
    private float lastSlowTime = -999f;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        Vector3Int cellPos = tarTilemap.WorldToCell(playerTransform.position);

        if (tarTilemap.HasTile(cellPos))
        {
            if (!inTar || Time.time - lastSlowTime >= slowDuration)
            {
                inTar = true;
                lastSlowTime = Time.time;

                SkillManager.Instance.NerfFor(SkillManager.SkillType.Speed, slowDown, slowDuration);
            }
        }
        else
        {
            inTar = false;
        }
    }
}
