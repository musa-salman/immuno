using UnityEngine;

[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private string uniqueID;

    public string ID => uniqueID;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = gameObject.scene.name + "_" + gameObject.name + "_" + System.Guid.NewGuid();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
