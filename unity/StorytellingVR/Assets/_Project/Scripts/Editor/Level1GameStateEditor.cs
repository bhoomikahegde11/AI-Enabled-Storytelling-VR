using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level1GameState)), CanEditMultipleObjects]
public class Level1GameStateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "marketDayDurationSeconds");
        SerializedProperty duration = serializedObject.FindProperty("marketDayDurationSeconds");
        EditorGUI.showMixedValue = duration.hasMultipleDifferentValues;
        EditorGUI.BeginChangeCheck();
        float minutes = EditorGUILayout.FloatField(new GUIContent("Entire Main Loop Duration (Minutes)",
            "One shared timer for every customer, conversation and fulfillment. Default: 15 minutes."),
            duration.floatValue / 60f);
        if (EditorGUI.EndChangeCheck()) duration.floatValue = Mathf.Max(1f / 60f, minutes) * 60f;
        EditorGUI.showMixedValue = false;
        serializedObject.ApplyModifiedProperties();
    }
}
