using System.Collections.Generic;
using UnityEngine;
using Data;

public class RAGRetriever : MonoBehaviour
{
    string knowledge;

    public List<TradeGood> goods = new List<TradeGood>();
    public List<CustomerPersonality> personalities = new List<CustomerPersonality>();

    void Awake()
    {
        // Load grounding knowledge
        TextAsset data = Resources.Load<TextAsset>("vijayanagar_knowledge");
        knowledge = data != null ? data.text : "";

        // Load goods
        TextAsset goodsJson = Resources.Load<TextAsset>("trader_goods");
        if (goodsJson != null)
        {
            string wrapped = "{\"items\":" + goodsJson.text + "}";
            TradeGoodList list = JsonUtility.FromJson<TradeGoodList>(wrapped);
            if (list != null && list.items != null)
            {
                goods.AddRange(list.items);
            }
        }

        // Load customer personalities
        TextAsset customersJson = Resources.Load<TextAsset>("customers");
        if (customersJson != null)
        {
            string wrapped = "{\"items\":" + customersJson.text + "}";
            CustomerPersonalityList list = JsonUtility.FromJson<CustomerPersonalityList>(wrapped);
            if (list != null && list.items != null)
            {
                personalities.AddRange(list.items);
            }
        }
    }

    public string GetKnowledge()
    {
        return knowledge;
    }

    // Backwards-compatible method
    public string RetrieveContext(string item)
    {
        return GetKnowledge();
    }

    public TradeGood GetRandomGood()
    {
        if (goods == null || goods.Count == 0) return null;
        return goods[Random.Range(0, goods.Count)];
    }

    public CustomerPersonality GetRandomPersonality()
    {
        if (personalities == null || personalities.Count == 0) return null;
        return personalities[Random.Range(0, personalities.Count)];
    }

    public TradeGood GetGoodById(string id)
    {
        return goods.Find(g => g.id == id);
    }

    public CustomerPersonality GetPersonalityById(string id)
    {
        return personalities.Find(p => p.id == id);
    }
}