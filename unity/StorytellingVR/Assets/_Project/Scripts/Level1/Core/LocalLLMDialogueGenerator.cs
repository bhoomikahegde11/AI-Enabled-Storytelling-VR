using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LlamaCppUnity;
using UnityEngine;

public class LocalLLMDialogueGenerator
{
#if UNITY_EDITOR
    private const int TimeoutMs = 12000;
#else
    private const int TimeoutMs = 6000;
#endif

    private static readonly string[] ImmersionBreakers =
    {
        "rupee", "rupees", "dollar", "dollars", "bitcoin", "crypto", "bank", "credit card",
        "internet", "phone", "computer", "download", "upload"
    };

    public class GenerationResult
    {
        public string rawOutput;
        public string cleanedOutput;
        public string finalLine;
        public string fallbackReason;
        public string validationFailureReason;
        public bool validationPassed;
        public float elapsedSeconds;
    }

    public float TimeoutSeconds => TimeoutMs / 1000f;

    public Task<GenerationResult> BeginGenerate(
        int turnId,
        string playerRawText,
        NegotiationInput input,
        LocalTradeState trade,
        RuleBasedNPCBrainResult brainResult)
    {
        Debug.Log("[LLM-GEN] Started turn: " + turnId);
        Debug.Log("[LLM-GEN] Model path: " + LocalLLMInterpreter.SharedModelPath);
        Llama llama = LocalLLMInterpreter.GetSharedLlama();
        if (llama == null)
        {
            Debug.Log("[LLM-GEN] Fallback reason: model missing");
            return Task.FromResult(new GenerationResult { fallbackReason = "model missing" });
        }

        string prompt = BuildPrompt(playerRawText, input, trade, brainResult);
        Debug.Log("[LLM-GEN] Prompt: " + prompt);
        return Task.Run(() => GenerateBlocking(llama, prompt, brainResult != null ? brainResult.replyText : string.Empty));
    }

    private static GenerationResult GenerateBlocking(Llama llama, string prompt, string fallbackLine)
    {
        GenerationResult result = new GenerationResult();
        try
        {
            DateTime startedAt = DateTime.UtcNow;
            lock (LocalLLMInterpreter.SyncRoot)
            {
                result.rawOutput = llama.Run(
                    prompt,
                    maxTokens: 48,
                    temperature: 0.2f,
                    topP: 0.9f,
                    topK: 20,
                    repeatPenalty: 1.05f,
                    stop: new List<string> { "\n", "<|im_end|>", "<|endoftext|>" });
            }

            result.elapsedSeconds = (float)(DateTime.UtcNow - startedAt).TotalSeconds;
            result.cleanedOutput = CleanOutput(result.rawOutput);

            if ((result.elapsedSeconds * 1000f) > TimeoutMs)
            {
                result.fallbackReason = "timeout";
                return result;
            }

            if (string.IsNullOrWhiteSpace(result.cleanedOutput))
            {
                result.fallbackReason = "empty output";
                return result;
            }

            if (!IsValidRewrite(result.cleanedOutput, fallbackLine, out string validationFailureReason))
            {
                result.fallbackReason = "validation failed";
                result.validationFailureReason = validationFailureReason;
                return result;
            }

            result.validationPassed = true;
            result.finalLine = result.cleanedOutput;
            return result;
        }
        catch (Exception ex)
        {
            result.fallbackReason = ex.Message;
            return result;
        }
    }

    private static string BuildPrompt(string playerRawText, NegotiationInput input, LocalTradeState trade, RuleBasedNPCBrainResult brainResult)
    {
        string spice = trade != null ? trade.spiceDisplayName : "spice";
        string quantity = trade != null ? trade.quantityLabel : "unknown";
        string buyerName = trade != null ? trade.buyerName : "buyer";
        string mood = trade != null ? trade.buyerPersonality : "neutral";
        int currentOffer = trade != null ? trade.npcOffer : 0;
        int sellerPrice = input != null && input.hasSellerPrice ? input.sellerPrice : -1;
        int quantityGrams = input != null && input.hasQuantity ? input.quantityGrams : 0;
        string cleanedText = input != null ? input.normalizedText : playerRawText;

        return
            "<|im_start|>system\n" +
            "You are rewriting the NPC's already-decided reply.\n" +
            "Do not negotiate.\n" +
            "Do not change any decision, price, quantity, acceptance, rejection, or counter.\n" +
            "Preserve all numbers exactly.\n" +
            "Do not add a new offer.\n" +
            "Do not say the NPC offers a price unless the rule reply is already a counter-offer.\n" +
            "Speak as a human buyer in a 16th-century Hampi Bazaar.\n" +
            "Natural, slightly historical, not Shakespearean.\n" +
            "Output only one short spoken line, max 22 words, max 2 sentences.\n" +
            "No markdown. No JSON. No explanations. No role labels.\n" +
            "Avoid flat lines like '" + buyerName + " offers 250 varahas.'\n" +
            "<|im_end|>\n" +
            "<|im_start|>user\n" +
            "Player raw text: " + playerRawText + "\n" +
            "Player cleaned text: " + cleanedText + "\n" +
            "Detected intent: " + (input != null ? input.intent.ToString() : "UNKNOWN") + "\n" +
            "Dialogue action already decided by rules: " + GetDialogueAction(input, brainResult) + "\n" +
            "Safe rule-based reply to rewrite: " + (brainResult != null ? brainResult.replyText : string.Empty) + "\n" +
            "Spice: " + spice + "\n" +
            "Quantity label: " + quantity + "\n" +
            "Quantity grams: " + quantityGrams + "\n" +
            "Seller offered price: " + sellerPrice + "\n" +
            "Current buyer offer: " + currentOffer + "\n" +
            "Rule counter/final price: " + (brainResult != null ? brainResult.updatedOffer : 0) + "\n" +
            "Buyer name: " + buyerName + "\n" +
            "Buyer mood: " + mood + "\n" +
            "Rewrite the safe reply so it sounds more natural while preserving the exact meaning and all numbers.\n" +
            "<|im_end|>\n" +
            "<|im_start|>assistant\n";
    }

    private static string CleanOutput(string rawOutput)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
        {
            return string.Empty;
        }

        string text = rawOutput.Trim();
        text = Regex.Replace(text, @"^[""'`]+|[""'`]+$", string.Empty);
        text = Regex.Replace(text, @"^(assistant|npc|buyer)\s*:\s*", string.Empty, RegexOptions.IgnoreCase);
        text = Regex.Replace(text, @"\s+", " ").Trim();

        int newline = text.IndexOf('\n');
        if (newline >= 0)
        {
            text = text.Substring(0, newline).Trim();
        }

        string[] words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length > 22)
        {
            text = string.Join(" ", words, 0, 22).Trim();
        }

        return text;
    }

    private static bool IsValidRewrite(string rewritten, string fallbackLine, out string reason)
    {
        reason = string.Empty;
        if (string.IsNullOrWhiteSpace(rewritten))
        {
            reason = "empty cleaned output";
            return false;
        }

        string lower = rewritten.ToLowerInvariant();
        if (lower.Contains("assistant:") || lower.Contains("system:") || lower.Contains("prompt:") || lower.Contains("rewrite:"))
        {
            reason = "role or prompt leakage";
            return false;
        }

        foreach (string breaker in ImmersionBreakers)
        {
            if (Regex.IsMatch(lower, @"\b" + Regex.Escape(breaker) + @"\b"))
            {
                reason = "immersion breaker: " + breaker;
                return false;
            }
        }

        HashSet<string> fallbackNumbers = ExtractNumbers(fallbackLine);
        HashSet<string> rewrittenNumbers = ExtractNumbers(rewritten);
        if (fallbackNumbers.Count > 0 && !fallbackNumbers.SetEquals(rewrittenNumbers))
        {
            reason = "numbers changed";
            return false;
        }

        return true;
    }

    private static string GetDialogueAction(NegotiationInput input, RuleBasedNPCBrainResult brainResult)
    {
        if (brainResult != null)
        {
            if (brainResult.isAccepted || string.Equals(brainResult.resolutionAction, "ACCEPT", StringComparison.OrdinalIgnoreCase))
            {
                return "ACCEPT";
            }
            if (brainResult.walkedAway || string.Equals(brainResult.resolutionAction, "WALK_AWAY", StringComparison.OrdinalIgnoreCase))
            {
                return "REJECT";
            }
        }

        switch (input != null ? input.intent : NegotiationIntent.UNKNOWN)
        {
            case NegotiationIntent.QUANTITY_QUERY:
                return "ASK_QUANTITY";
            case NegotiationIntent.PRICE_QUERY:
            case NegotiationIntent.QUERY_BUYER_BUDGET:
                return "ASK_PRICE";
            case NegotiationIntent.SOCIAL:
            case NegotiationIntent.GENERAL_DIALOGUE:
            case NegotiationIntent.GREETING:
                return "SOCIAL";
            case NegotiationIntent.OFF_TOPIC:
            case NegotiationIntent.HOSTILE:
                return "OFF_TOPIC";
            default:
                return "COUNTER";
        }
    }

    private static HashSet<string> ExtractNumbers(string text)
    {
        HashSet<string> values = new HashSet<string>();
        if (string.IsNullOrWhiteSpace(text))
        {
            return values;
        }

        MatchCollection matches = Regex.Matches(text, @"\b\d+\b");
        foreach (Match match in matches)
        {
            values.Add(match.Value);
        }

        return values;
    }
}
