using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Condition
{
    public enum Action
    {
        CheckRect,
        None,
    }

    public Action condition = Action.CheckRect;
    public Node destination;
    public int timeToCheck = 0;
    // Is it a continuous action ?
    public bool isLooping = false;
    public Figures.Rect rectangle;
}

[System.Serializable]
public class Pattern
{
    public Node goTo;
    public int intervalUntilNextAction;
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
            return point.x >= topLeft.x  && point.x <= topRight.x && topRight.y >= point.y && bottomLeft.y <= point.y;
        }
    }
}

/*[CustomEditor(typeof(Monster))]
[CanEditMultipleObjects]
public class MonsterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lightDetectionRange"), new GUIContent("Range of light detection"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("
        "), new GUIContent("Range of sound detection"), true);
        EditorGUILayout.HelpBox("The field 'Interval Until Next Action' will be called after the end of the current movement", MessageType.Info);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementPattern"), new GUIContent("Patterns"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"), new GUIContent("Monster's speed"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_allNoiseListened"), new GUIContent("Noise listened"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetBehaviour"), new GUIContent("Behaviour when spotting target"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentFloor"), new GUIContent("Starting floor"), true);
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
            if ((int)Condition.Action.None != conditionProp.intValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("destination"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("timeToCheck"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("isLooping"));
                if ((int)Condition.Action.CheckRect == conditionProp.intValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("cond").FindPropertyRelative("rectangle"), true);

                    Debug.DrawLine(_monster.cond.rectangle.topLeft, _monster.cond.rectangle.topRight, Color.green, Time.deltaTime);
                    Debug.DrawLine(_monster.cond.rectangle.topLeft, _monster.cond.rectangle.bottomLeft, Color.green, Time.deltaTime);
                    Debug.DrawLine(_monster.cond.rectangle.topRight, _monster.cond.rectangle.bottomRight, Color.green, Time.deltaTime);
                    Debug.DrawLine(_monster.cond.rectangle.bottomLeft, _monster.cond.rectangle.bottomRight, Color.green, Time.deltaTime);
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
    }

    private AnimBool _showExtraFields = new AnimBool(true);
    private Monster _monster;
} */