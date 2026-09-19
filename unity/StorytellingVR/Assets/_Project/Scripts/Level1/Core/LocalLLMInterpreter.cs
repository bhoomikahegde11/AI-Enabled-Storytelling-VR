using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LlamaCppUnity;
using UnityEngine;

public class LLMIntentResult
{
    public string cleanedText = string.Empty;
    public NegotiationIntent intent = NegotiationIntent.UNKNOWN;
    public int? sellerPrice;
    public int? quantity;
    public bool confidence;
}

public class LocalLLMInterpreter
{
    private const int TimeoutMs = 8000;
    internal static readonly object SyncRoot = new object();
    private static Llama sharedLlama;
    private static bool loadAttempted;
    internal static string SharedModelPath => NormalizeModelPath(
        Path.Combine(Application.streamingAssetsPath, "LLM", "Qwen2.5-1.5B-Instruct-Q4_K_M.gguf"));

    public bool TryInterpret(string rawPlayerText, LocalTradeState trade, out LLMIntentResult result)
    {
        result = null;
        Debug.Log("[LLM-INTENT] Disabled. Using rule-based interpreter.");
        return false;
    }

    public string BuildPrompt(string rawPlayerText, LocalTradeState trade)
    {
        string spice = trade != null ? trade.spiceDisplayName : "spice";
        string quantity = trade != null ? trade.quantityLabel : "unknown";
        int currentOffer = trade != null ? trade.npcOffer : 0;

        return
            "<|im_start|>system\n" +
            "You are only an interpreter.\n" +
            "Do not negotiate.\n" +
            "Do not answer the player.\n" +
            "Do not explain.\n" +
            "Return JSON only.\n" +
            "Intent must be one of:\n" +
            "PRICE | COUNTER | ACCEPT | QUERY_BUYER_BUDGET | QUERY_QUANTITY | SOCIAL | OFF_TOPIC | UNKNOWN\n" +
            "If the player is asking what quantity is wanted, use QUERY_QUANTITY.\n" +
            "If the player states a price, extract the numeric price.\n" +
            "Examples:\n" +
            "Input: 100 and 10\n" +
            "{\"intent\":\"PRICE\",\"price\":\"110\",\"quantity\":null,\"cleanedText\":\"I offer 110\"}\n" +
            "Input: okay what do you want\n" +
            "{\"intent\":\"QUERY_QUANTITY\",\"price\":null,\"quantity\":null,\"cleanedText\":\"What quantity do you want?\"}\n" +
            "Input: i can give one hundred\n" +
            "{\"intent\":\"PRICE\",\"price\":\"100\",\"quantity\":null,\"cleanedText\":\"I can give 100\"}\n" +
            "Input: deal\n" +
            "{\"intent\":\"ACCEPT\",\"price\":null,\"quantity\":null,\"cleanedText\":\"deal\"}\n" +
            "<|im_end|>\n" +
            "<|im_start|>user\n" +
            "Context:\n" +
            "- spice: " + spice + "\n" +
            "- quantity: " + quantity + "\n" +
            "- currentOffer: " + currentOffer + "\n" +
            "Player input:\n" +
            rawPlayerText + "\n" +
            "Return JSON only with keys intent, price, quantity, cleanedText.\n" +
            "<|im_end|>\n" +
            "<|im_start|>assistant\n";
    }

    private static Llama GetOrLoadModel()
    {
        return GetSharedLlama();
    }

    private static string NormalizeModelPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        return Path.GetFullPath(path).Replace('\\', '/');
    }

    internal static Llama GetSharedLlama()
    {
        if (sharedLlama != null)
        {
            return sharedLlama;
        }

        if (loadAttempted)
        {
            return null;
        }

        lock (SyncRoot)
        {
            if (sharedLlama != null)
            {
                return sharedLlama;
            }

            if (loadAttempted)
            {
                return null;
            }

            loadAttempted = true;
            string modelPath = SharedModelPath;
            if (!File.Exists(modelPath))
            {
                Debug.LogWarning("[LocalLLMInterpreter] Model not found: " + modelPath);
                return null;
            }

            try
            {
                sharedLlama = new Llama(
                    modelPath,
                    nCtx: 1024,
                    nBatch: 512,
                    nThreads: (uint)Mathf.Max(1, SystemInfo.processorCount / 2),
                    nThreadsBatch: (uint)Mathf.Max(1, SystemInfo.processorCount / 2),
                    chatFormat: "chatml",
                    verbose: false);
                Debug.Log("[LocalLLMInterpreter] Loaded model: " + modelPath);
                return sharedLlama;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[LocalLLMInterpreter] Failed to load model: " + modelPath + " | " + ex.Message);
                sharedLlama = null;
                return null;
            }
        }
    }

    private static string ExtractJson(string response)
    {
        Match match = Regex.Match(response, "\\{[\\s\\S]*\\}");
        return match.Success ? match.Value : string.Empty;
    }

    private static string ExtractJsonString(string json, string key)
    {
        Match match = Regex.Match(json, "\"" + Regex.Escape(key) + "\"\\s*:\\s*\"([\\s\\S]*?)\"");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static int? ExtractJsonNullableInt(string json, string key)
    {
        Match nullMatch = Regex.Match(json, "\"" + Regex.Escape(key) + "\"\\s*:\\s*null", RegexOptions.IgnoreCase);
        if (nullMatch.Success)
        {
            return null;
        }

        Match numberMatch = Regex.Match(json, "\"" + Regex.Escape(key) + "\"\\s*:\\s*(-?\\d+)");
        if (!numberMatch.Success)
        {
            return null;
        }

        return int.TryParse(numberMatch.Groups[1].Value, out int parsed) ? parsed : null;
    }

    private static bool TryMapIntent(string intent, out NegotiationIntent mappedIntent)
    {
        switch ((intent ?? string.Empty).Trim().ToUpperInvariant())
        {
            case "PRICE":
                mappedIntent = NegotiationIntent.PRICE;
                return true;
            case "COUNTER":
                mappedIntent = NegotiationIntent.COUNTER;
                return true;
            case "ACCEPT":
                mappedIntent = NegotiationIntent.ACCEPT;
                return true;
            case "QUERY_BUYER_BUDGET":
                mappedIntent = NegotiationIntent.QUERY_BUYER_BUDGET;
                return true;
            case "QUERY_QUANTITY":
                mappedIntent = NegotiationIntent.QUANTITY_QUERY;
                return true;
            case "SOCIAL":
                mappedIntent = NegotiationIntent.SOCIAL;
                return true;
            case "OFF_TOPIC":
                mappedIntent = NegotiationIntent.OFF_TOPIC;
                return true;
            case "UNKNOWN":
                mappedIntent = NegotiationIntent.UNKNOWN;
                return true;
            default:
                mappedIntent = NegotiationIntent.UNKNOWN;
                return false;
        }
    }
}
