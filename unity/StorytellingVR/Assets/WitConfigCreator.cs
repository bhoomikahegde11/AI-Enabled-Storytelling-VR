using UnityEngine;
using Meta.WitAi.Data.Configuration;

public class WitConfigCreator : MonoBehaviour
{
    [ContextMenu("Create Wit Config")]
    void CreateConfig()
    {
        var config = ScriptableObject.CreateInstance<WitConfiguration>();

        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(config, "Assets/WitConfiguration.asset");
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        #endif

        Debug.Log("WitConfiguration created at Assets/WitConfiguration.asset");
    }
}