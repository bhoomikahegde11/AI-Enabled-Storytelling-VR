using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Data;

public class RAGRetriever : MonoBehaviour
{
    [System.Serializable]
    public class KnowledgeChunk
    {
        public string id;
        public string content;
    }

    List<KnowledgeChunk> chunks = new List<KnowledgeChunk>();

    public List<TradeGood> goods = new List<TradeGood>();
    public List<CustomerPersonality> personalities = new List<CustomerPersonality>();

    void Awake()
    {
        LoadKnowledge();
        LoadGoods();
        LoadPersonalities();

        Debug.Log($"[RAGRetriever] Loaded {chunks.Count} knowledge chunks, {goods.Count} goods, {personalities.Count} personalities.");
    }

    void LoadKnowledge()
    {
        TextAsset data = Resources.Load<TextAsset>("vijayanagar_knowledge");
        if (data == null)
        {
            Debug.LogWarning("[RAGRetriever] vijayanagar_knowledge.txt not found in Resources.");
            return;
        }

        string[] sections = data.text.Split(new string[] { "---CHUNK:" }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string section in sections)
        {
            int newline = section.IndexOf('\n');
            if (newline < 0) continue;

            string id = section.Substring(0, newline).Trim().Replace("---", "").Trim();
            string content = section.Substring(newline + 1).Trim();

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(content))
            {
                chunks.Add(new KnowledgeChunk { id = id, content = content });
            }
        }
    }

    void LoadGoods()
    {
        TextAsset goodsJson = Resources.Load<TextAsset>("trader_goods");
        if (goodsJson != null)
        {
            string wrapped = "{\"items\":" + goodsJson.text + "}";
            TradeGoodList list = JsonUtility.FromJson<TradeGoodList>(wrapped);
            if (list?.items != null) goods.AddRange(list.items);
        }
    }

    void LoadPersonalities()
    {
        TextAsset customersJson = Resources.Load<TextAsset>("customers");
        if (customersJson != null)
        {
            string wrapped = "{\"items\":" + customersJson.text + "}";
            CustomerPersonalityList list = JsonUtility.FromJson<CustomerPersonalityList>(wrapped);
            if (list?.items != null) personalities.AddRange(list.items);
        }
    }

    // Retrieve top-k chunks relevant to the query using keyword scoring
    public string RetrieveContext(string query, int topK = 3)
    {
        if (chunks.Count == 0) return "";

        string[] queryWords = query.ToLower().Split(new char[] { ' ', '_', '-' }, System.StringSplitOptions.RemoveEmptyEntries);

        var scored = chunks
            .Select(chunk => new
            {
                chunk,
                score = queryWords.Count(w =>
                    chunk.id.ToLower().Contains(w) ||
                    chunk.content.ToLower().Contains(w))
            })
            .OrderByDescending(x => x.score)
            .Take(topK)
            .Select(x => x.chunk.content);

        return string.Join("\n\n", scored);
    }

    // Build a query string from the current negotiation context
    public string BuildQuery(string personalityId, string goodId, string goodCategory, string moment)
    {
        return $"{personalityId} {goodId} {goodCategory} {moment}";
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

    public TradeGood GetGoodById(string id) => goods.Find(g => g.id == id);
    public CustomerPersonality GetPersonalityById(string id) => personalities.Find(p => p.id == id);
}