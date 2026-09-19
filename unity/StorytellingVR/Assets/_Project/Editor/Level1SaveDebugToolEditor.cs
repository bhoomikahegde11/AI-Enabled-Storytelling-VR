#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level1SaveDebugTool))]
public class Level1SaveDebugToolEditor : Editor
{
    private SerializedProperty selectedTestStateNameProperty;
    private SerializedProperty newTestStateNameProperty;

    private void OnEnable()
    {
        selectedTestStateNameProperty = serializedObject.FindProperty("selectedTestStateName");
        newTestStateNameProperty = serializedObject.FindProperty("newTestStateName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Level1SaveDebugTool tool = (Level1SaveDebugTool)target;
        tool.EnsureDirectoriesExist();

        EditorGUILayout.LabelField("Current Save Path", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(tool.CurrentSavePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));
        EditorGUILayout.HelpBox(tool.ActiveSaveExists ? "Active Level 1 save found." : "No active Level 1 save file exists yet.", MessageType.Info);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Active Save Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Delete Active Save"))
        {
            tool.DeleteActiveSave();
            RefreshAssets();
        }

        if (GUILayout.Button("Reload Active Save"))
        {
            tool.ReloadActiveSave();
            RefreshAssets();
        }

        if (GUILayout.Button("Start Fresh"))
        {
            tool.StartFresh();
            RefreshAssets();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test States", EditorStyles.boldLabel);

        string[] availableStates = tool.GetAvailableTestStateNames();
        int selectedIndex = GetSelectedIndex(availableStates, selectedTestStateNameProperty.stringValue);

        if (availableStates.Length == 0)
        {
            EditorGUILayout.HelpBox("No test-state JSON files found in Assets/_Project/SaveStates/Level1/TestStates.", MessageType.None);
            selectedTestStateNameProperty.stringValue = string.Empty;
        }
        else
        {
            int newIndex = EditorGUILayout.Popup("Selected Test State", Mathf.Max(0, selectedIndex), availableStates);
            selectedTestStateNameProperty.stringValue = availableStates[newIndex];

            string selectedPath = tool.GetSelectedTestStatePath();
            EditorGUILayout.SelectableLabel(selectedPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));

            if (GUILayout.Button("Copy Selected Test State To Active Save"))
            {
                tool.CopySelectedTestStateToActiveSave();
                RefreshAssets();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create Test State Snapshot", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(newTestStateNameProperty, new GUIContent("New Test State Name"));

        if (GUILayout.Button("Save Current Active Save As New Test State"))
        {
            tool.SaveCurrentActiveSaveAsNewTestState();
            RefreshAssets();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test States Folder", EditorStyles.boldLabel);
        EditorGUILayout.SelectableLabel(tool.TestStatesDirectory, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));

        if (GUILayout.Button("Reveal Test States Folder"))
        {
            EditorUtility.RevealInFinder(tool.TestStatesDirectory);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static int GetSelectedIndex(string[] options, string selectedValue)
    {
        if (options == null || options.Length == 0)
        {
            return -1;
        }

        for (int i = 0; i < options.Length; i++)
        {
            if (options[i] == selectedValue)
            {
                return i;
            }
        }

        return 0;
    }

    private static void RefreshAssets()
    {
        AssetDatabase.Refresh();
    }
}
#endif
