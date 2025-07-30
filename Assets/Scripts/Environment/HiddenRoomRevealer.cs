using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenRoomRevealer : MonoBehaviour
{
    [Header("Room -> Puzzle Mappings")]
    [SerializeField] private List<RoomPuzzlePair> roomPuzzlePairs;

    [Header("Hidden Room Tilemap Renderer")]
    public TilemapRenderer hiddenLayerRenderer;
    public float fadeDuration = 1f;

    private Material _material;
    private Coroutine _fadeCoroutine;

    private PlayerMovement playerMovement;

    private PuzzleButtonController puzzleButtonController;

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        puzzleButtonController = FindObjectOfType<PuzzleButtonController>();

        _material = hiddenLayerRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(0f);
        }
    }

    public void StartPuzzle(Transform playerTransform)
    {
        float tolerance = 10f;
        float minDistance = Mathf.Infinity;
        RoomPuzzlePair? closestPair = null;


        for (int i = 0; i < roomPuzzlePairs.Count; i++)
        {
            var pair = roomPuzzlePairs[i];

            if (pair.isSolved)
                continue;

            float distance = Vector2.Distance(playerTransform.position, pair.roomTransform.position);
            Debug.Log($"Checking pair: {pair.puzzleTransform.name}, distance: {distance}");
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPair = pair;
            }
        }

        if (closestPair.HasValue && minDistance <= tolerance)
        {
            var pair = closestPair.Value;
            playerMovement.SetPuzzleTransform(pair.puzzleTransform);
            puzzleButtonController.SetCurrentPuzzle(pair.puzzleTransform);
            Debug.Log($"Starting puzzle at {pair.puzzleTransform.name}, distance: {minDistance}");
        }
        else
        {
            Debug.LogWarning("No puzzle found within tolerance or all puzzles are already solved.");
        }
    }


    public void CancelPuzzle()
    {
        Debug.Log("Puzzle cancelled by player. Skipping puzzle.");
        playerMovement.DoneSolvingPuzzle();
        FindObjectOfType<EnemyManager>()?.SetEnemiesActive(true);
    }


    public void PayExpertToSolvePuzzle(Transform puzzleTransform)
    {
        if (ScoreManager.Instance.CanPayForPuzzle())
        {
            // Deduct cost
            ScoreManager.Instance.PayForPuzzle();

            DonePuzzle(puzzleTransform);
        }
    }


    public void DonePuzzle(Transform puzzleTransform)
    {
        for (int i = 0; i < roomPuzzlePairs.Count; i++)
        {
            var pair = roomPuzzlePairs[i];
            if (pair.puzzleTransform == puzzleTransform)
            {
                pair.isSolved = true;
                playerMovement.DoneSolvingPuzzle();

                if (pair.barrierCollider != null)
                {
                    string id = pair.barrierCollider.GetComponent<UniqueID>().ID;
                    GameSaveManager.Instance.SolvedPuzzles.Add(id);

                    pair.barrierCollider.enabled = false;
                }
                FindObjectOfType<EnemyManager>()?.SetEnemiesActive(true);
                return;
            }
        }

        Debug.LogWarning("Puzzle transform not found in room-puzzle pairs.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(1f);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeTo(targetAlpha));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (_material == null)
        {
            Debug.LogWarning("Hidden room material not assigned.");
            yield break;
        }

        float elapsed = 0f;
        Color startColor = _material.color;
        Color targetColor = new(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _material.color = Color.Lerp(startColor, targetColor, elapsed / fadeDuration);
            yield return null;
        }

        _material.color = targetColor;
    }
}

[Serializable]
public struct RoomPuzzlePair
{
    public Transform roomTransform;
    public Transform puzzleTransform;
    public Collider2D barrierCollider;
    public bool isSolved;
}
