using UnityEngine;

public class TextPrompt : MonoBehaviour
{
    private Canvas canvas;
    void Start()
    {
        Transform childTransform = transform.GetChild(0);

        Canvas canvas = childTransform.GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }
    public void ShowPrompt()
    {   
        Debug.Log("" + canvas != null);
        if (canvas != null)
        {
            Debug.Log("Showing text prompt");
            canvas.enabled = true;
        }
    }
    public void HidePrompt()
    {
        if (canvas != null)
        {
        Debug.Log("Hiding text prompt");
            canvas.enabled = false;
        }
    }
}
