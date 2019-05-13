using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestObject))]
public sealed class MeshExtenderEditor : Editor
{
    private const int HandleSize = 5;
    
    private TestObject _gameObject;

    private SerializedProperty _xAxisPosition;
    private SerializedProperty _yAxisPosition;
    private SerializedProperty _zAxisPosition;
    
    private int _xPositiveId;
    private int _xNegativeId;
    private int _yPositiveId;
    private int _yNegativeId;
    private int _zPositiveId;
    private int _zNegativeId;
    
    private void OnEnable()
    {
        _gameObject = (TestObject)target;

        _xAxisPosition = serializedObject.FindProperty(nameof(TestObject.XHandlePosition));
        _yAxisPosition = serializedObject.FindProperty(nameof(TestObject.YHandlePosition));
        _zAxisPosition = serializedObject.FindProperty(nameof(TestObject.ZHandlePosition));

        _xPositiveId = GUIUtility.GetControlID(FocusType.Passive);
        _xNegativeId = GUIUtility.GetControlID(FocusType.Passive);
        _yPositiveId = GUIUtility.GetControlID(FocusType.Passive);
        _yNegativeId = GUIUtility.GetControlID(FocusType.Passive);
        _zPositiveId = GUIUtility.GetControlID(FocusType.Passive);
        _zNegativeId = GUIUtility.GetControlID(FocusType.Passive);
    }


    private void OnSceneGUI()
    {
        serializedObject.Update();
        
        var transform   = _gameObject.transform;
        var objPosition = transform.position;

        var xAxis = transform.right;
        var yAxis = transform.up;
        var zAxis = transform.forward;

        var xPositiveHandlePos = objPosition + xAxis;
        var xNegativeHandlePos = objPosition - xAxis;
        var yPositiveHandlePos = objPosition + yAxis;
        var yNegativeHandlePos = objPosition - yAxis;
        var zPositiveHandlePos = objPosition + zAxis;
        var zNegativeHandlePos = objPosition - zAxis;

        var newXPositivePos = DrawFreeMoveHandle(_xPositiveId, xPositiveHandlePos, Quaternion.LookRotation(xAxis), HandleSize);
        var newXNegativePos = DrawFreeMoveHandle(_xNegativeId, xNegativeHandlePos, Quaternion.LookRotation(-xAxis), HandleSize);
        var newYPositivePos = DrawFreeMoveHandle(_yPositiveId, yPositiveHandlePos, Quaternion.LookRotation(yAxis), HandleSize);
        var newYNegativePos = DrawFreeMoveHandle(_yNegativeId, yNegativeHandlePos, Quaternion.LookRotation(-yAxis), HandleSize);
        var newZPositivePos = DrawFreeMoveHandle(_zPositiveId, zPositiveHandlePos, Quaternion.LookRotation(zAxis), HandleSize);
        var newZNegativePos = DrawFreeMoveHandle(_zNegativeId, zNegativeHandlePos, Quaternion.LookRotation(-zAxis), HandleSize);
        
        CheckDiff(xPositiveHandlePos.x, newXPositivePos.x, xAxis);
        CheckDiff(xNegativeHandlePos.x, newXNegativePos.x, -xAxis);
        CheckDiff(yPositiveHandlePos.y, newYPositivePos.y, yAxis);
        CheckDiff(yNegativeHandlePos.y, newYNegativePos.y, -yAxis);
        CheckDiff(zPositiveHandlePos.z, newZPositivePos.z, zAxis);
        CheckDiff(zNegativeHandlePos.z, newZNegativePos.z, -zAxis);
    }

    private void CheckDiff(float originAxisPosition, float newAxisPosition, Vector3 direction)
    {
        if (Mathf.Abs(originAxisPosition - newAxisPosition) < 2f)
        {
            return;
        }
        
        
    }

    private Vector3 DrawFreeMoveHandle(int controlId, Vector3 position, Quaternion rotation, float size)
        => Handles.FreeMoveHandle(
            controlId, 
            position,
            Quaternion.identity, 
            size, 
            Vector3.zero, 
            (id, cPosition, _, cSize, type) => DrawArrow(id, cPosition, rotation, size, type));

    private void DrawArrow(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType) 
        => Handles.ArrowHandleCap(controlId, position, rotation, size, eventType);
}