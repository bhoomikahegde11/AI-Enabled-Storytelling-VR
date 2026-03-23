using System.Collections.Generic;
using UnityEngine;
using Data;

public class CostBoardUI : MonoBehaviour
{
    [Tooltip("Parent transform under which rows will be instantiated")]
    public RectTransform contentParent;

    [Tooltip("A small UI prefab with a CostBoardRow component")]
    public GameObject rowPrefab;

    Dictionary<string, GameObject> rows = new Dictionary<string, GameObject>();

    public void Populate(IEnumerable<TradeGood> goods)
    {
        Clear();

        foreach (var g in goods)
        {
            if (rowPrefab == null || contentParent == null) break;

            var go = Instantiate(rowPrefab, contentParent);
            var row = go.GetComponent<CostBoardRow>();
            if (row != null)
            {
                row.Set(g.name, g.cost_price_per_unit);
                rows[g.id] = go;
            }
            else
            {
                // Fallback: set name on GameObject
                go.name = g.name + "_row";
                rows[g.id] = go;
            }
        }
    }

    public void Clear()
    {
        foreach (var kv in rows)
        {
            if (kv.Value != null)
                Destroy(kv.Value);
        }
        rows.Clear();
    }

    public void Highlight(string goodId)
    {
        foreach (var kv in rows)
        {
            var row = kv.Value.GetComponent<CostBoardRow>();
            if (row != null)
                row.SetHighlighted(kv.Key == goodId);
        }
    }
}
