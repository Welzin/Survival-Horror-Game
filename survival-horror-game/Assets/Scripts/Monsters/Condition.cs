using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

[System.Serializable]
public class Condition
{
    public enum Action
    {
        CheckRect,
        CheckCircle,
    }

    public Action condition = Action.CheckRect;
    public GameObject destination;
    public int timeToCheck = 0;
    // Is it a continuous action ?
    public bool isLooping = false;
    public Figures.Rect rectangle;
    public Figures.Circ circle;
}
 
namespace Figures
{
    [System.Serializable]
    public class Rect
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;

        public bool Contains(Vector2 point)
        {
            return topLeft.x <= point.x && topRight.x >= point.x && topRight.y >= point.y && bottomLeft.y <= point.y;
        }
    }
    [System.Serializable]
    public class Circ
    {
        public Vector2 center;
        public float radius;
    }
}

[CustomEditor(typeof(Monster))]
[CanEditMultipleObjects]
public class MonsterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lightDetectionRange"), new GUIContent("Range of light detection"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("soundDetectionRange"), new GUIContent("Range of sound detection"), true);
        EditorGUILayout.HelpBox("The field 'Interval Until Next Action' will be called after the end of the current movement", MessageType.Info);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementPattern"), new GUIContent("Patterns"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("Monster's speed"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("yell"), new GUIContent("Monster's yell"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetBehaviour"), new GUIContent("Behaviour when spotting target"), true);
        DisplayCondition();
        serializedObject.ApplyModifiedProperties();
    }

    private void DisplayCondition()
    {
        _showExtraFields.target = EditorGUILayout.ToggleLeft("Condition to activate pattern", _showExtraFields.target);
        _monster = (Monster)target;
        if(EditorGUILayout.BeginFadeGroup(_showExtraFields.faded))
        {
            EditorGUI.indentLevel++;
            SerializedProperty conditionProp = serializedObject.FindProperty("cond").FindPropertyRelative("condition");
            EditorGUILayout.PropertyField(conditionProp);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("destination"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("timeToCheck"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("isLooping"));
            if((int)Condition.Action.CheckCircle == conditionProp.intValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("circle"), true);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("rectangle"), true);

                Debug.DrawLine(_monster.cond.rectangle.topLeft, _monster.cond.rectangle.topRight, Color.green, Time.deltaTime);
                Debug.DrawLine(_monster.cond.rectangle.topLeft, _monster.cond.rectangle.bottomLeft, Color.green, Time.deltaTime);
                Debug.DrawLine(_monster.cond.rectangle.topRight, _monster.cond.rectangle.bottomRight, Color.green, Time.deltaTime);
                Debug.DrawLine(_monster.cond.rectangle.bottomLeft, _monster.cond.rectangle.bottomRight, Color.green, Time.deltaTime);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
    }

    private AnimBool _showExtraFields = new AnimBool(true);
    private Monster _monster;
} 