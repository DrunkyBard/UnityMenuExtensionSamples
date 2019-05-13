using UnityEditor;
using UnityEngine;

public class TestObject : MonoBehaviour
{
    public Vector3 CustomLocalPosition;

    public int ControlId;
    public Vector3 ControlPosition;
    public Vector3 OriginalPosition;

    private void OnEnable()
    {
        var objTransform = transform;
        var objPosition  = objTransform.position;
    }
}
