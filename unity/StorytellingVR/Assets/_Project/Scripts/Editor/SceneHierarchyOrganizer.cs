using UnityEngine;
using UnityEditor;

public class SceneHierarchyOrganizer : EditorWindow
{
    [MenuItem("Tools/Clean Up Scene Hierarchy")]
    public static void CleanUp()
    {
        Debug.Log("[SceneHierarchyOrganizer] Starting hierarchy cleanup...");

        // 1. Create or Find parent folders at the root level
        Transform envParent = GetOrCreateParent("ENVIRONMENT");
        Transform npcParent = GetOrCreateParent("NPC_SYSTEM");
        Transform uiParent = GetOrCreateParent("UI_SYSTEM");
        Transform gameParent = GetOrCreateParent("GAME_SYSTEM");
        Transform spawnParent = GetOrCreateParent("SPAWN_POINTS");

        // Create Subparents under ENVIRONMENT
        Transform stallParent = GetOrCreateSubParent(envParent, "Stall");
        Transform spicesParent = GetOrCreateSubParent(envParent, "Spices");
        Transform propsParent = GetOrCreateSubParent(envParent, "Props");

        // 2. Reorganize Environment elements
        // Move to Stall
        MoveObjectToParent("bazar pillar", stallParent);
        MoveObjectToParent("bazar pillar (1)", stallParent);
        MoveObjectToParent("bazar pillar (2)", stallParent);
        MoveObjectToParent("bazarflatstone", stallParent);
        MoveObjectToParent("WPSS_Flat_1", stallParent);

        // Move to Spices
        MoveObjectToParent("bazarspecies", spicesParent);

        // Move to Props
        MoveObjectToParent("Cube", propsParent);
        MoveObjectToParent("Cube(1)", propsParent);

        // 3. Reorganize NPC_SYSTEM elements
        MoveObjectToParent("indian m in kurta (1)", npcParent);
        MoveObjectToParent("indian man in kurta (1)", npcParent); // fallback names

        // 4. Reorganize UI_SYSTEM elements
        MoveObjectToParent("speechPoint", uiParent);
        MoveObjectToParent("EventSystem", uiParent);

        // 5. Reorganize GAME_SYSTEM elements
        MoveObjectToParent("GameManager", gameParent);
        MoveObjectToParent("[BuildingBlock] Speech To Text", gameParent);
        MoveObjectToParent("Speech To Text", gameParent); // fallback name

        // 6. Spawn Points (ensure any user spawn points are grouped)
        MoveObjectToParent("SpawnPoint", spawnParent);
        MoveObjectToParent("TradePoint", spawnParent);
        MoveObjectToParent("ExitPoint", spawnParent);

        Debug.Log("[SceneHierarchyOrganizer] Clean Up Hierarchy completed successfully! Press Ctrl+Z in Unity to undo if needed.");
        EditorApplication.Beep();
    }

    private static Transform GetOrCreateParent(string name)
    {
        GameObject go = GameObject.Find("/" + name);
        if (go == null)
        {
            go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, "Create parent " + name);
        }
        return go.transform;
    }

    private static Transform GetOrCreateSubParent(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == name)
            {
                return parent.GetChild(i);
            }
        }
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Create subparent " + name);
        return go.transform;
    }

    private static void MoveObjectToParent(string name, Transform parent)
    {
        GameObject go = GameObject.Find("/" + name);
        if (go == null)
        {
            go = GameObject.Find(name);
        }

        if (go != null)
        {
            Undo.SetTransformParent(go.transform, parent, "Move " + name);
            Debug.Log($"[SceneHierarchyOrganizer] Moved '{name}' to parent '{parent.name}'");
        }
    }
}
