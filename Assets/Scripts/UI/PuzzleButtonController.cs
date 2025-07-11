using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuzzleButtonController : MonoBehaviour
{
    [Header("Button References")]
    public Button cancelButton;
    public Button payExpertButton;
    private TMP_Text expertCostText;

    private HiddenRoomRevealer hiddenRoomRevealer;
    private Transform currentPuzzleTransform;

    private void Start()
    {
        hiddenRoomRevealer = FindObjectOfType<HiddenRoomRevealer>();

        cancelButton.onClick.AddListener(OnCancelClicked);
        payExpertButton.interactable = ScoreManager.Instance.CanPayForPuzzle();
        payExpertButton.onClick.AddListener(OnPayExpertClicked);

        expertCostText = payExpertButton.GetComponentInChildren<TMP_Text>();
        expertCostText.text = $"Pay: {ScoreManager.Instance.GetPointsPerPuzzle}";

        payExpertButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        currentPuzzleTransform = null;
    }

    public void SetCurrentPuzzle(Transform puzzleTransform)
    {
        currentPuzzleTransform = puzzleTransform;
        payExpertButton.interactable = ScoreManager.Instance.CanPayForPuzzle();
        expertCostText.text = $"Pay: {ScoreManager.Instance.GetPointsPerPuzzle}";

        StartCoroutine(ShowButtonsWithDelay(3f));

        Debug.Log($"Current puzzle position: {currentPuzzleTransform.position}, interactable: {payExpertButton.interactable}");
    }

    private IEnumerator ShowButtonsWithDelay(float delay)
    {
        payExpertButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay);

        payExpertButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        cancelButton.interactable = true;

        Debug.Log("Buttons activated after delay.");
    }

    public void OnCancelClicked()
    {
        Debug.Log("Cancel button clicked.");
        if (currentPuzzleTransform == null)
            return;

        Debug.Log("Canceling puzzle interaction.");

        payExpertButton.interactable = false;
        payExpertButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        hiddenRoomRevealer.CancelPuzzle();
    }

    public void OnPayExpertClicked()
    {
        payExpertButton.interactable = false;
        payExpertButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        hiddenRoomRevealer.PayExpertToSolvePuzzle(currentPuzzleTransform);
    }
}
