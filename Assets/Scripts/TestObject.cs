using UnityEngine;

public class TestObject : MonoBehaviour
{
    public Vector3 CustomLocalPosition;

    public Vector3 XHandlePosition;
    public Vector3 YHandlePosition;
    public Vector3 ZHandlePosition;

    private void OnEnable()
    {
        var objTransform = transform;
        var objPosition  = objTransform.position;
        
        XHandlePosition = objPosition + objTransform.right;
        YHandlePosition = objPosition + objTransform.up;
        ZHandlePosition = objPosition + objTransform.forward;
    }
}
