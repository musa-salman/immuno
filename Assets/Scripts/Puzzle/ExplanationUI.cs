using TMPro;
using UnityEngine;

public class ExplanationUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    public void ShowExplanation(PuzzleHelpData data)
    {
        gameObject.SetActive(true);
        titleText.text = data.title;
        bodyText.text = data.bodyText;
    }

    public void HideExplanation() => gameObject.SetActive(false);
    public void ToggleExplanation() => gameObject.SetActive(!gameObject.activeSelf);
}
