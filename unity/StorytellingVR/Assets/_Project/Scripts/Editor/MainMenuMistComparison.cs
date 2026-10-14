using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only A/B control. Copies settings onto the existing emitter; never adds another.
public static class MainMenuMistComparison
{
    private const string Menu = "Anubhava/Main Menu Mist/";
    private const string Presets = "Assets/_Project/Prefabs/MainMenu/";

    [MenuItem(Menu + "Cloud Mist")]
    private static void Cloud() => Apply("GroundMist_CloudPreset.prefab");

    [MenuItem(Menu + "Original Mist")]
    private static void Original() => Apply("GroundMist_OriginalPreset.prefab");

    [MenuItem(Menu + "Mist Disabled")]
    private static void Disable()
    {
        ParticleSystem target = FindTarget();
        if (!target) return;
        Undo.RecordObject(target.gameObject, "Disable main menu mist");
        target.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        target.gameObject.SetActive(false);
        MarkScene(target);
    }

    [MenuItem(Menu + "Cloud Mist", true)]
    [MenuItem(Menu + "Original Mist", true)]
    [MenuItem(Menu + "Mist Disabled", true)]
    private static bool CanCompare() => SceneManager.GetActiveScene().name == "MainMenu_Optimized";

    private static ParticleSystem FindTarget()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "MainMenu_Optimized") return null;
        ParticleSystem result = null;
        foreach (ParticleSystem candidate in Object.FindObjectsByType<ParticleSystem>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (candidate.gameObject.scene != scene || candidate.name != "Ground_Mist") continue;
            if (result)
            {
                Debug.LogError("Mist comparison requires exactly one Ground_Mist in MainMenu_Optimized.");
                return null;
            }
            result = candidate;
        }
        if (!result || !result.transform.parent || result.transform.parent.name != "MainMenu_Ambience")
        {
            Debug.LogError("Mist comparison requires Ground_Mist under MainMenu_Ambience. Preset poses are relative to that parent.");
            return null;
        }
        return result;
    }

    private static void Apply(string filename)
    {
        ParticleSystem target = FindTarget();
        GameObject preset = AssetDatabase.LoadAssetAtPath<GameObject>(Presets + filename);
        if (!target) return;
        ParticleSystem source = preset ? preset.GetComponent<ParticleSystem>() : null;
        ParticleSystemRenderer sourceRenderer = preset ? preset.GetComponent<ParticleSystemRenderer>() : null;
        ParticleSystemRenderer targetRenderer = target.GetComponent<ParticleSystemRenderer>();
        if (!source || !sourceRenderer || !targetRenderer)
        {
            Debug.LogError("Cannot apply mist preset. Check " + Presets + filename + " and Ground_Mist's ParticleSystemRenderer.");
            return;
        }

        Undo.RecordObjects(new Object[] { target, targetRenderer, target.transform, target.gameObject },
            "Change main menu mist preset");
        target.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        EditorUtility.CopySerialized(source, target);
        EditorUtility.CopySerialized(sourceRenderer, targetRenderer);
        target.transform.localPosition = source.transform.localPosition;
        target.transform.localRotation = source.transform.localRotation;
        target.transform.localScale = source.transform.localScale;
        target.gameObject.SetActive(true);
        if (Application.isPlaying) target.Play();
        MarkScene(target);
    }

    private static void MarkScene(ParticleSystem target)
    {
        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
        SceneView.RepaintAll();
    }
}
