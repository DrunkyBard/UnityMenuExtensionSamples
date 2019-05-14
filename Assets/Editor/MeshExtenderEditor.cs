using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestObject))]
public sealed class MeshExtenderEditor : Editor
{
    private const float HandleSize = 5f;
    private const float ActionDistance = 5f;
    
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

        var xPositiveHandlePos = DefineHandlePosition(_xPositiveId);
        var xNegativeHandlePos = DefineHandlePosition(_xNegativeId);
        var yPositiveHandlePos = DefineHandlePosition(_yPositiveId);
        var yNegativeHandlePos = DefineHandlePosition(_yNegativeId);
        var zPositiveHandlePos = DefineHandlePosition(_zPositiveId);
        var zNegativeHandlePos = DefineHandlePosition(_zNegativeId);

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

        CheckDiff(_controlOriginalPosition.vector3Value.x, xPositiveHandlePos, newXPositivePos.x, newXPositivePos, xAxis, objPosition);
        CheckDiff(_controlOriginalPosition.vector3Value.x, xNegativeHandlePos, newXNegativePos.x, newXNegativePos, -xAxis, objPosition);
        CheckDiff(_controlOriginalPosition.vector3Value.y, yPositiveHandlePos, newYPositivePos.y, newYPositivePos, yAxis, objPosition);
        CheckDiff(_controlOriginalPosition.vector3Value.y, yNegativeHandlePos, newYNegativePos.y, newYNegativePos, -yAxis, objPosition);
        CheckDiff(_controlOriginalPosition.vector3Value.z, zPositiveHandlePos, newZPositivePos.z, newZPositivePos, zAxis, objPosition);
        CheckDiff(_controlOriginalPosition.vector3Value.z, zNegativeHandlePos, newZNegativePos.z, newZNegativePos, -zAxis, objPosition);

        serializedObject.ApplyModifiedProperties();
    }

    private Vector3 DefineHandlePosition(int controlId)
        => _controlId.intValue == controlId ? _controlPosition.vector3Value : _gameObject.transform.position;

    private void CheckDiff(
        float originAxisPosition, 
        Vector3 oldPosition, 
        float newAxisPosition, Vector3 newPosition, 
        Vector3 direction, 
        Vector3 objPosition)
    {
        if (oldPosition == newPosition)
        {
            return;
        }

        if (!Mathf.Approximately(originAxisPosition, newAxisPosition))
        {
            _controlPosition.vector3Value = newPosition;
        }
        
        if (Mathf.Abs(originAxisPosition - newAxisPosition) < ActionDistance)
        {
            return;
        }
        
        var angle = Vector3.Angle(newPosition - oldPosition, direction);
        
        if (Mathf.Approximately(angle, 180f))
        {
            _controlOriginalPosition.vector3Value -= direction * ActionDistance;
            var hits = Physics.RaycastAll(newPosition + direction / 2, direction, HandleSize);

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == _gameObject.gameObject)
                {
                    continue;
                }
                
                if (hit.collider.gameObject.GetComponent<TestObject>() != null)
                {
                    DestroyImmediate(hit.collider.gameObject, false);
                }
            }
        }
        else
        {
            _controlOriginalPosition.vector3Value += direction * ActionDistance;

            var newObject          = Instantiate(_gameObject);
            var newObjectTransform = newObject.transform;
            var originalTransform  = _gameObject.transform;

            newObjectTransform.position = _controlOriginalPosition.vector3Value;
            newObjectTransform.rotation = originalTransform.rotation;            
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