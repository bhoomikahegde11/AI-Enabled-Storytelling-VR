#if UNITY_EDITOR

using TMPro;
using UnityEditor;
using UnityEngine;

public class TMPSceneFontReplacer : EditorWindow
{
    private TMP_FontAsset targetFont;
    private bool includeInactive = true;

    [MenuItem("Tools/UI/Replace TMP Font In Current Scene")]
    public static void ShowWindow()
    {
        GetWindow<TMPSceneFontReplacer>(
            "TMP Scene Font Replacer"
        );
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Replace TMP Font In Current Scene",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "Target Font",
            targetFont,
            typeof(TMP_FontAsset),
            false
        );

        includeInactive = EditorGUILayout.Toggle(
            "Include Inactive Objects",
            includeInactive
        );

        EditorGUILayout.Space();

        if (targetFont == null)
        {
            EditorGUILayout.HelpBox(
                "Assign a TMP Font Asset first.",
                MessageType.Info
            );
        }

        GUI.enabled = targetFont != null;

        if (GUILayout.Button("Replace Fonts In Current Scene"))
        {
            ReplaceFonts();
        }

        GUI.enabled = true;
    }

    private void ReplaceFonts()
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(
            includeInactive
                ? FindObjectsInactive.Include
                : FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        int changedCount = 0;

        Undo.SetCurrentGroupName(
            "Replace TMP Fonts In Current Scene"
        );

        int undoGroup = Undo.GetCurrentGroup();

        foreach (TMP_Text text in texts)
        {
            if (text == null)
                continue;

            // Ignore objects that are not part of a valid scene,
            // such as prefab assets being previewed in the editor.
            if (!text.gameObject.scene.IsValid())
                continue;

            if (text.font == targetFont)
                continue;

            Undo.RecordObject(
                text,
                "Replace TMP Font"
            );

            text.font = targetFont;

            EditorUtility.SetDirty(text);

            changedCount++;
        }

        Undo.CollapseUndoOperations(
            undoGroup
        );

        if (changedCount > 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene()
            );
        }

        Debug.Log(
            $"[TMP FONT TOOL] Replaced font on {changedCount} TMP text objects in the current scene."
        );
    }
}

#endif