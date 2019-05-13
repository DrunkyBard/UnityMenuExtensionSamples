//using UnityEditor;
//using UnityEngine;
//
////[CustomEditor(typeof(TestObject))]
//public class TestObjectEditor : Editor
//{
//    private SerializedProperty _customLocalPositionProp;
//    private TestObject _testObject;
//    
//    private void OnEnable()
//    {
//        _customLocalPositionProp = serializedObject.FindProperty(nameof(TestObject.CustomLocalPosition));
//        _testObject              = (TestObject) target;
//    }
//    
//    private void OnSceneGUI()
//    {
//        serializedObject.Update();
//        var wPos = _testObject.transform.TransformPoint(_customLocalPositionProp.vector3Value);
//
//        EditorGUI.BeginChangeCheck();
//        
//        var newPos = Handles.FreeMoveHandle(
//            wPos, 
//            Quaternion.identity, 
//            0.02f, 
//            Vector3.zero,
//            Handles.DotHandleCap);
//        
//        if (EditorGUI.EndChangeCheck())
//        {
//            //_testObject.transform.position        = newPos;
//            _customLocalPositionProp.vector3Value = _testObject.transform.InverseTransformPoint(newPos);
//            Undo.RecordObject(_testObject, "Change position");
//        }
//
//        serializedObject.ApplyModifiedProperties();
//    }
//}
