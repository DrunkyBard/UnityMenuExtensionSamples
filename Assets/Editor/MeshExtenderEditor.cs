using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestObject))]
public sealed class MeshExtenderEditor : Editor
{
    private const int HandleSize = 5;
    
    private TestObject _gameObject;

    private SerializedProperty _controlId;
    private SerializedProperty _controlPosition;
    private SerializedProperty _controlOriginalPosition;
    
    private int _xPositiveId;
    private int _xNegativeId;
    private int _yPositiveId;
    private int _yNegativeId;
    private int _zPositiveId;
    private int _zNegativeId;
    
    private void OnEnable()
    {
        _gameObject = (TestObject)target;

        _controlId               = serializedObject.FindProperty(nameof(TestObject.ControlId));
        _controlPosition         = serializedObject.FindProperty(nameof(TestObject.ControlPosition));
        _controlOriginalPosition = serializedObject.FindProperty(nameof(TestObject.OriginalPosition));

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
        
        if (Event.current.type == EventType.MouseUp)
        {
            _controlId.intValue = -1;
        }
        
        var transform   = _gameObject.transform;
        var objPosition = transform.position;
        
        Handles.matrix = transform.localToWorldMatrix;
        
        var xAxis = transform.right;
        var yAxis = transform.up;
        var zAxis = transform.forward;

        if (Event.current.type == EventType.MouseDown)
        {
            var id = HandleUtility.nearestControl;
            _controlId.intValue = id;

            _controlPosition.vector3Value = objPosition;
            _controlOriginalPosition.vector3Value = _controlPosition.vector3Value; 
        }

        var xPositiveHandlePos = _controlId.intValue == _xPositiveId ? _controlPosition.vector3Value : objPosition;
        var xNegativeHandlePos = _controlId.intValue == _xNegativeId ? _controlPosition.vector3Value : objPosition;
        var yPositiveHandlePos = _controlId.intValue == _yPositiveId ? _controlPosition.vector3Value : objPosition;
        var yNegativeHandlePos = _controlId.intValue == _yNegativeId ? _controlPosition.vector3Value : objPosition;
        var zPositiveHandlePos = _controlId.intValue == _zPositiveId ? _controlPosition.vector3Value : objPosition;
        var zNegativeHandlePos = _controlId.intValue == _zNegativeId ? _controlPosition.vector3Value : objPosition;

        var newXPositivePos = DrawFreeMoveHandle(_xPositiveId, xPositiveHandlePos, Quaternion.LookRotation(xAxis), HandleSize);
        newXPositivePos.y = xPositiveHandlePos.y; newXPositivePos.z = xPositiveHandlePos.z;
        
        var newXNegativePos = DrawFreeMoveHandle(_xNegativeId, xNegativeHandlePos, Quaternion.LookRotation(-xAxis), HandleSize);
        newXNegativePos.y = xNegativeHandlePos.y; newXNegativePos.z = xNegativeHandlePos.z;
        
        var newYPositivePos = DrawFreeMoveHandle(_yPositiveId, yPositiveHandlePos, Quaternion.LookRotation(yAxis), HandleSize);
        newYPositivePos.x = yPositiveHandlePos.x; newYPositivePos.z = yPositiveHandlePos.z;
        
        var newYNegativePos = DrawFreeMoveHandle(_yNegativeId, yNegativeHandlePos, Quaternion.LookRotation(-yAxis), HandleSize);
        newYNegativePos.x = yNegativeHandlePos.x; newYNegativePos.z = yNegativeHandlePos.z;
        
        var newZPositivePos = DrawFreeMoveHandle(_zPositiveId, zPositiveHandlePos, Quaternion.LookRotation(zAxis), HandleSize);
        newZPositivePos.x = zPositiveHandlePos.x; newZPositivePos.y = zPositiveHandlePos.y;
        
        var newZNegativePos = DrawFreeMoveHandle(_zNegativeId, zNegativeHandlePos, Quaternion.LookRotation(-zAxis), HandleSize);
        newZNegativePos.x = zNegativeHandlePos.x; newZNegativePos.y = zNegativeHandlePos.y;

        CheckDiff(_xPositiveId, _controlOriginalPosition.vector3Value.x, xPositiveHandlePos, newXPositivePos.x, newXPositivePos,xAxis);
        CheckDiff(_xNegativeId, _controlOriginalPosition.vector3Value.x, xNegativeHandlePos, newXNegativePos.x, newXNegativePos, -xAxis);
        CheckDiff(_yPositiveId, _controlOriginalPosition.vector3Value.y, yPositiveHandlePos, newYPositivePos.y, newYPositivePos, yAxis);
        CheckDiff(_yNegativeId, _controlOriginalPosition.vector3Value.y, yNegativeHandlePos, newYNegativePos.y, newYNegativePos, -yAxis);
        CheckDiff(_zPositiveId, _controlOriginalPosition.vector3Value.z, zPositiveHandlePos, newZPositivePos.z, newZPositivePos, zAxis);
        CheckDiff(_zNegativeId, _controlOriginalPosition.vector3Value.z, zNegativeHandlePos, newZNegativePos.z, newZNegativePos, -zAxis);

        serializedObject.ApplyModifiedProperties();
    }

    private void CheckDiff(int controlId, float originAxisPosition, Vector3 originalPosition, float newAxisPosition, Vector3 newPosition, Vector3 direction)
    {
        if (originalPosition == newPosition)
        {
            return;
        }

        if (!Mathf.Approximately(originAxisPosition, newAxisPosition))
        {
            _controlPosition.vector3Value = newPosition;
        }
        
        if (Mathf.Abs(originAxisPosition - newAxisPosition) < 10f)
        {
            return;
        }
        
        _controlOriginalPosition.vector3Value = newPosition;
        var newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.transform.position = _gameObject.transform.localToWorldMatrix.MultiplyVector(newPosition);
        newCube.transform.rotation = _gameObject.transform.rotation;
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