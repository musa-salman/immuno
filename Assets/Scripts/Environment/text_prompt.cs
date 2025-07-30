using UnityEngine;

public class TextPrompt : MonoBehaviour
{
    private Canvas canvas;
    void Start()
    {
        Transform childTransform = transform.GetChild(0);

        canvas = childTransform.GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
    public void ShowPrompt()
    {
        Debug.Log("" + canvas);

        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }
    public void HidePrompt()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}
