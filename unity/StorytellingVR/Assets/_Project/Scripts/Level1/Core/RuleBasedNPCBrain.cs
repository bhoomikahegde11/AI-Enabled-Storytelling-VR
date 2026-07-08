using System;
using UnityEngine;

public class RuleBasedNPCBrainResult
{
    public string replyText;
    public int updatedOffer;
    public int resolvedPrice;
    public int resolvedQuantityGrams;
    public float trust;
    public float frustration;
    public int outOfWorldCount;
    public string resolutionAction = "OFFER";
    public bool isFinished;
    public bool isAccepted;
    public bool walkedAway;
}

public class RuleBasedNPCBrain
{
    public RuleBasedNPCBrainResult GenerateReply(
        string playerText,
        NegotiationInput input,
        LocalTradeState trade,
        int roundCount,
        int patience)
    {
        EnsureTradeInitialized(trade, patience);

        string buyerName = trade != null ? trade.buyerName : "Buyer";
        string buyerOrigin = trade != null ? trade.buyerOrigin : "Merchant";
        string personality = trade != null ? trade.buyerPersonality : string.Empty;
        string spice = trade != null ? trade.spiceDisplayName.ToLowerInvariant() : "spice";
        string quantity = trade != null ? trade.quantityLabel : "this lot";
        int currentOffer = trade != null ? trade.npcOffer : 0;
        int marketValue = trade != null ? trade.marketValue : 0;
        int sellerPrice = input != null && input.hasSellerPrice ? input.sellerPrice : -1;
        int quantityGrams = input != null && input.hasQuantity ? input.quantityGrams : (trade != null ? trade.quantityGrams : 0);
        string spiceDescriptor = GetSpiceDescriptor(trade != null ? trade.spiceKey : string.Empty, spice);
        float marketPerGram = trade != null && trade.quantityGrams > 0 ? (float)trade.marketValue / trade.quantityGrams : 0f;
        float targetMarketValue = quantityGrams > 0 ? marketPerGram * quantityGrams : marketValue;

        RuleBasedNPCBrainResult result = new RuleBasedNPCBrainResult
        {
            updatedOffer = currentOffer,
            resolvedPrice = currentOffer,
            resolvedQuantityGrams = trade != null ? trade.quantityGrams : 0
        };

        if (trade == null || input == null)
        {
            result.replyText = "Speak plainly, merchant.";
            return result;
        }

        UpdateMemory(trade, input);
        if (input.hasQuantity && input.quantityGrams > 0)
        {
            result.resolvedQuantityGrams = input.quantityGrams;
        }

        if (trade.buyerPatience <= 0 || trade.buyerFrustration >= GetWalkAwayThreshold(personality))
        {
            string reason = trade.buyerPatience <= 0 ? "buyer patience exhausted" : "buyer frustration threshold reached";
            LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "WALK_AWAY", reason);
            return WalkAway(result, trade, GetWalkAwayLine(buyerName, personality), "WALK_AWAY");
        }

        if (input.needsClarification)
        {
            result.replyText = GetClarificationLine(input, trade, spiceDescriptor, quantity, currentOffer);
            return SyncState(result, trade);
        }

        switch (input.intent)
        {
            case NegotiationIntent.GREETING:
                result.replyText = GetGreetingLine(buyerName, buyerOrigin, personality, quantity, spiceDescriptor);
                return SyncState(result, trade);

            case NegotiationIntent.ITEM_QUERY:
                result.replyText = GetItemQueryLine(trade, spiceDescriptor);
                return SyncState(result, trade);

            case NegotiationIntent.QUANTITY_QUERY:
                result.replyText = GetQuantityQueryLine(trade, quantity, spiceDescriptor);
                return SyncState(result, trade);

            case NegotiationIntent.QUERY_BUYER_BUDGET:
                trade.priceIntroduced = true;
                trade.budgetRevealed = true;
                result.replyText = $"I can offer {currentOffer} varahas for {quantity} of {spiceDescriptor}. That is fair for today's bazaar.";
                return SyncState(result, trade);

            case NegotiationIntent.SOCIAL:
            case NegotiationIntent.GENERAL_DIALOGUE:
                result.replyText = GetSocialLine(input.normalizedText, trade, buyerName, buyerOrigin, personality, spiceDescriptor);
                return SyncState(result, trade);

            case NegotiationIntent.OFF_TOPIC:
                trade.outOfWorldCount++;
                AdjustEmotion(trade, 0.18f, -0.08f);
                if (trade.outOfWorldCount >= 4)
                {
                    return WalkAway(result, trade, "I am done with such talk. We shall not trade today.", "WALK_AWAY");
                }
                result.replyText = GetOutOfWorldLine(trade, spiceDescriptor);
                return SyncState(result, trade);

            case NegotiationIntent.HOSTILE:
                trade.hostileCount++;
                AdjustEmotion(trade, 0.28f, -0.12f);
                if (trade.hostileCount >= 4 || (trade.hostileCount >= 3 && trade.buyerFrustration >= 0.75f))
                {
                    return WalkAway(result, trade, "Speak with respect in this bazaar, or we are finished.", "WALK_AWAY");
                }
                result.replyText = GetHostileLine(trade);
                return SyncState(result, trade);

            case NegotiationIntent.CONTINUE:
                AdjustEmotion(trade, -0.01f, 0.03f);
                result.replyText = $"My offer stands at {currentOffer} varahas for {quantity} of {spiceDescriptor}.";
                return SyncState(result, trade);

            case NegotiationIntent.REJECT:
                if (input.hasHardRejection)
                {
                    trade.hardRejectCount++;
                    trade.rejectionCount++;
                    AdjustEmotion(trade, 0.14f, -0.06f);
                    if (trade.rejectionCount >= GetMaxRejections(personality, trade))
                    {
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "WALK_AWAY", "hard rejection exhausted buyer patience");
                        return WalkAway(result, trade, GetWalkAwayLine(buyerName, personality), "WALK_AWAY");
                    }
                }
                else
                {
                    AdjustEmotion(trade, 0.06f, -0.02f);
                }

                int rejectionStep = Mathf.Max(GetMinimumVisibleMovement(currentOffer), Mathf.RoundToInt(currentOffer * 0.05f));
                result.updatedOffer = Mathf.Min(currentOffer + rejectionStep, trade.maxBuyerPrice);
                result.replyText = result.updatedOffer > currentOffer
                    ? (input.hasHardRejection
                        ? $"Then hear my better offer: {result.updatedOffer} varahas for {quantity} of {spiceDescriptor}."
                        : $"If not that, I can improve it to {result.updatedOffer} varahas for {quantity} of {spiceDescriptor}.")
                    : (input.hasHardRejection ? "Then we are too far apart, merchant." : "If that price does not suit you, name a better one.");
                LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, result.updatedOffer > currentOffer ? "COUNTER" : "REJECT", input.hasHardRejection ? "hard rejection prompted counter" : "soft rejection prompted counter", result.updatedOffer - currentOffer);
                return SyncState(result, trade);

            case NegotiationIntent.ACCEPT:
                if (!CanFinalizeAcceptance(input, trade))
                {
                    result.replyText = GetClarificationLine(input, trade, spiceDescriptor, quantity, currentOffer);
                    return SyncState(result, trade);
                }

                if (input.hasSellerPrice && trade.lastSellerPrice > 0 && trade.lastSellerPrice < targetMarketValue * 0.3f)
                {
                    result.replyText = "That price is suspiciously low. I will not agree to it.";
                    return SyncState(result, trade);
                }

                result.resolvedPrice = ResolveAcceptedPrice(input, trade, currentOffer);
                result.replyText = input.hasSellerPrice && result.resolvedPrice == trade.lastSellerPrice
                    ? $"Agreed. I will pay {result.resolvedPrice} varahas for the {spiceDescriptor}."
                    : $"Agreed. {quantity} of {spiceDescriptor} for {result.resolvedPrice} varahas.";
                result.isFinished = true;
                result.isAccepted = true;
                result.resolutionAction = "ACCEPT";
                AdjustEmotion(trade, -0.1f, 0.1f);
                return SyncState(result, trade);

            case NegotiationIntent.QUANTITY_CHANGE:
                AdjustEmotion(trade, -0.01f, 0.03f);
                result.replyText = quantityGrams < trade.quantityGrams
                    ? $"For the smaller quantity of {trade.quantityLabel}, my offer remains {currentOffer} varahas."
                    : $"For {trade.quantityLabel} of {spiceDescriptor}, I can still begin at {currentOffer} varahas.";
                return SyncState(result, trade);

            case NegotiationIntent.ULTIMATUM:
                if (!input.hasSellerPrice)
                {
                    result.replyText = "State your final price clearly, merchant.";
                    return SyncState(result, trade);
                }

                trade.actualPriceOfferCount++;
                trade.sellerMinPrice = sellerPrice;
                AdjustEmotion(trade, 0.1f, -0.05f);
                if (sellerPrice > trade.maxBuyerPrice)
                {
                    if (ShouldWalkAwayFromHighPrice(trade, sellerPrice, roundCount))
                    {
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "WALK_AWAY", "ultimatum above buyer max");
                        return WalkAway(result, trade, "Then we shall not trade today.", "WALK_AWAY");
                    }

                    int stretchCounter = MoveTowardTarget(trade, trade.maxBuyerPrice, roundCount);
                    result.updatedOffer = Mathf.Max(currentOffer, stretchCounter);
                    result.replyText = $"{sellerPrice} is beyond what I can pay. My strongest offer is {result.updatedOffer} varahas.";
                    LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, result.updatedOffer > currentOffer ? "COUNTER" : "HOLD", "ultimatum above buyer max, stretch counter", result.updatedOffer - currentOffer);
                    return SyncState(result, trade);
                }

                if (ShouldHoldPosition(trade, sellerPrice, roundCount))
                {
                    result.replyText = $"This is my limit. I can hold at {currentOffer} varahas.";
                    LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "HOLD", "ultimatum hit buyer limit");
                    return SyncState(result, trade);
                }

                result.updatedOffer = MoveTowardTarget(trade, sellerPrice, roundCount);
                result.replyText = $"You press hard, merchant. I can move to {result.updatedOffer} varahas, but not beyond that.";
                LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, "COUNTER", "ultimatum counter", result.updatedOffer - currentOffer);
                return SyncState(result, trade);

            case NegotiationIntent.COUNTER:
                if (input.hasSellerPrice)
                {
                    trade.actualPriceOfferCount++;
                    trade.counterCount++;
                    AdjustEmotion(trade, 0.08f, 0f);
                    if (ShouldWalkAwayForNegotiationPressure(trade, input, roundCount))
                    {
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "WALK_AWAY", "price-offer pressure exceeded buyer limits");
                        return WalkAway(result, trade, GetWalkAwayLine(buyerName, personality), "WALK_AWAY");
                    }

                    trade.lastSellerPrice = sellerPrice;
                    if (sellerPrice > trade.maxBuyerPrice && !ShouldWalkAwayFromHighPrice(trade, sellerPrice, roundCount))
                    {
                        result.updatedOffer = MoveTowardTarget(trade, trade.maxBuyerPrice, roundCount);
                        result.replyText = GetCounterLine(trade, sellerPrice, result.updatedOffer, spiceDescriptor);
                        LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, "COUNTER", "counter above buyer max, stretch toward limit", result.updatedOffer - currentOffer);
                        return SyncState(result, trade);
                    }

                    if (ShouldHoldPosition(trade, sellerPrice, roundCount))
                    {
                        result.replyText = $"I cannot move beyond {currentOffer} varahas.";
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "HOLD", "counter-offer gap still too high");
                        return SyncState(result, trade);
                    }

                    result.updatedOffer = MoveTowardTarget(trade, sellerPrice, roundCount);
                    result.replyText = $"We are getting closer. I can offer {result.updatedOffer} varahas.";
                    LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, "COUNTER", "counter-offer accepted for negotiation", result.updatedOffer - currentOffer);
                    return SyncState(result, trade);
                }

                trade.softBargainCount++;
                int fallbackCounterStep = ComputeIncrement(trade, trade.lastSellerPrice > 0 ? trade.lastSellerPrice : trade.maxBuyerPrice, roundCount);
                result.updatedOffer = Mathf.Min(currentOffer + fallbackCounterStep, trade.maxBuyerPrice);
                result.replyText = $"I can increase my offer slightly to {result.updatedOffer} varahas.";
                LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, "COUNTER", "counter language without explicit price", fallbackCounterStep);
                return SyncState(result, trade);

            case NegotiationIntent.PRICE:
            case NegotiationIntent.QUANTITY_PRICE:
                if (!input.hasSellerPrice)
                {
                    result.replyText = "I did not catch your price. Say it plainly.";
                    return SyncState(result, trade);
                }

                trade.actualPriceOfferCount++;
                trade.priceIntroduced = true;
                trade.lastSellerPrice = sellerPrice;

                float sellerPerGram = quantityGrams > 0 ? sellerPrice / Mathf.Max(1f, quantityGrams) : 0f;
                float marketPerGramForTarget = quantityGrams > 0 ? targetMarketValue / Mathf.Max(1f, quantityGrams) : 0f;

                if (sellerPerGram > 0f && marketPerGramForTarget > 0f && sellerPerGram < marketPerGramForTarget * 0.35f)
                {
                    trade.lowPriceCount++;
                    AdjustEmotion(trade, 0.05f + (0.04f * Mathf.Min(trade.lowPriceCount - 1, 2)), -0.06f);
                    if (trade.lowPriceCount >= 3)
                    {
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "WALK_AWAY", "seller price suspiciously low");
                        return WalkAway(result, trade, "That price is too suspicious. I will take my leave.", "WALK_AWAY");
                    }
                    result.replyText = $"That is far too low for honest trade. My offer remains {currentOffer} varahas.";
                    LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "HOLD", "seller price below honest trade floor");
                    return SyncState(result, trade);
                }

                if (sellerPerGram > 0f && marketPerGramForTarget > 0f && sellerPerGram > marketPerGramForTarget * 1.8f)
                {
                    trade.tooExpensiveCount++;
                    AdjustEmotion(trade, 0.2f, -0.2f);
                    if (ShouldWalkAwayFromHighPrice(trade, sellerPrice, roundCount) || input.intent == NegotiationIntent.ULTIMATUM)
                    {
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "WALK_AWAY", "seller price far above market range");
                        return WalkAway(result, trade, "Your demand is too high. Then we shall not trade.", "WALK_AWAY");
                    }
                    int strongCounter = MoveTowardTarget(trade, trade.maxBuyerPrice, roundCount);
                    result.updatedOffer = Mathf.Max(currentOffer, strongCounter);
                    result.replyText = GetCounterLine(trade, sellerPrice, result.updatedOffer, spiceDescriptor);
                    LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, result.updatedOffer > currentOffer ? "COUNTER" : "HOLD", "seller price high but still negotiable", result.updatedOffer - currentOffer);
                    return SyncState(result, trade);
                }

                if (sellerPrice == currentOffer)
                {
                    result.resolvedPrice = sellerPrice;
                    result.resolvedQuantityGrams = quantityGrams;
                    result.replyText = $"Agreed. {trade.quantityLabel} of {spiceDescriptor} for {sellerPrice} varahas.";
                    result.isFinished = true;
                    result.isAccepted = true;
                    result.resolutionAction = "ACCEPT";
                    AdjustEmotion(trade, -0.08f, 0.1f);
                    LogNegotiation(trade, input, currentOffer, sellerPrice, roundCount, "ACCEPT", "seller exactly matched current buyer offer");
                    return SyncState(result, trade);
                }

                if (sellerPrice < currentOffer)
                {
                    if (CanAcceptNow(trade, roundCount) && CanFinalizeAcceptance(input, trade))
                    {
                        result.resolvedPrice = sellerPrice;
                        result.resolvedQuantityGrams = quantityGrams;
                        result.replyText = $"Agreed. {trade.quantityLabel} of {spiceDescriptor} for {sellerPrice} varahas.";
                        result.isFinished = true;
                        result.isAccepted = true;
                        result.resolutionAction = "ACCEPT";
                        AdjustEmotion(trade, -0.08f, 0.1f);
                        LogNegotiation(trade, input, currentOffer, sellerPrice, roundCount, "ACCEPT", "seller met current buyer offer");
                        return SyncState(result, trade);
                    }

                    result.replyText = $"Your price meets my offer, yet I will hold at {currentOffer} varahas.";
                    LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "HOLD", "offer matched but acceptance gate not satisfied");
                    return SyncState(result, trade);
                }

                if (sellerPrice <= GetAcceptanceThreshold(trade, roundCount) &&
                    sellerPrice - currentOffer <= GetNearAcceptGap(trade, roundCount) &&
                    CanFinalizeAcceptance(input, trade))
                {
                    result.resolvedPrice = sellerPrice;
                    result.resolvedQuantityGrams = quantityGrams;
                    result.replyText = $"We are close enough. Agreed at {sellerPrice} varahas.";
                    result.isFinished = true;
                    result.isAccepted = true;
                    result.resolutionAction = "ACCEPT";
                    AdjustEmotion(trade, -0.05f, 0.08f);
                    LogNegotiation(trade, input, currentOffer, sellerPrice, roundCount, "ACCEPT", "seller near buyer acceptance threshold");
                    return SyncState(result, trade);
                }

                if (sellerPrice > trade.maxBuyerPrice)
                {
                    trade.repeatedRejectedPrice = trade.lastPlayerPrice == sellerPrice ? trade.repeatedRejectedPrice + 1 : 1;
                    trade.repeatedOverMaxCount = trade.repeatedRejectedPrice;
                    trade.lastPlayerPrice = sellerPrice;
                    AdjustEmotion(trade, 0.1f, -0.06f);

                    if (trade.repeatedRejectedPrice >= 2)
                    {
                        result.replyText = $"I have already refused {sellerPrice}. Do not test my patience.";
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "REJECT", "repeated same over-max seller price");
                        return SyncState(result, trade);
                    }

                    if (ShouldHoldPosition(trade, sellerPrice, roundCount) && currentOffer >= Mathf.RoundToInt(trade.maxBuyerPrice * 0.9f))
                    {
                        result.replyText = $"{sellerPrice} is too high, merchant. I can only hold at {currentOffer} varahas.";
                        LogNegotiation(trade, input, currentOffer, currentOffer, roundCount, "HOLD", "seller price above max and buyer near ceiling");
                        return SyncState(result, trade);
                    }
                }

                result.updatedOffer = MoveTowardTarget(trade, sellerPrice, roundCount);
                result.replyText = GetCounterLine(trade, sellerPrice, result.updatedOffer, spiceDescriptor);
                LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, "COUNTER", "price offer moved buyer upward", result.updatedOffer - currentOffer);
                return SyncState(result, trade);

            case NegotiationIntent.BARGAIN:
                trade.repeatedBargains++;
                trade.softBargainCount++;
                AdjustEmotion(trade, 0.04f, 0.01f);
                int bargainTarget = trade.lastSellerPrice > 0 ? Mathf.Min(trade.lastSellerPrice, trade.maxBuyerPrice) : trade.maxBuyerPrice;
                int bargainStep = ComputeBargainIncrement(trade, currentOffer, bargainTarget, roundCount);
                result.updatedOffer = Mathf.Min(currentOffer + bargainStep, trade.maxBuyerPrice);
                result.replyText = result.updatedOffer > currentOffer
                    ? GetBargainLine(trade, result.updatedOffer, spiceDescriptor)
                    : "I can improve a little, but not much.";
                LogNegotiation(trade, input, currentOffer, result.updatedOffer, roundCount, result.updatedOffer > currentOffer ? "COUNTER" : "HOLD", "soft bargain response", result.updatedOffer - currentOffer);
                return SyncState(result, trade);

            case NegotiationIntent.PRICE_QUERY:
                result.replyText = $"I am offering {currentOffer} varahas for {quantity} of {spiceDescriptor}.";
                return SyncState(result, trade);

            case NegotiationIntent.CLARIFICATION:
            case NegotiationIntent.CONFUSED:
                result.replyText = GetClarificationLine(input, trade, spiceDescriptor, quantity, currentOffer);
                return SyncState(result, trade);

            default:
                AdjustEmotion(trade, 0.04f, -0.02f);
                result.replyText = $"Speak plainly, merchant. My offer stands at {currentOffer} varahas for {quantity} of {spiceDescriptor}.";
                return SyncState(result, trade);
        }
    }

    private static RuleBasedNPCBrainResult SyncState(RuleBasedNPCBrainResult result, LocalTradeState trade)
    {
        result.updatedOffer = Mathf.Max(0, result.updatedOffer);
        result.trust = trade.buyerTrust;
        result.frustration = trade.buyerFrustration;
        result.outOfWorldCount = trade.outOfWorldCount;
        if (result.resolvedPrice <= 0)
        {
            result.resolvedPrice = result.updatedOffer;
        }
        if (result.resolvedQuantityGrams <= 0)
        {
            result.resolvedQuantityGrams = trade.quantityGrams;
        }
        return result;
    }

    private static RuleBasedNPCBrainResult WalkAway(RuleBasedNPCBrainResult result, LocalTradeState trade, string text, string action)
    {
        result.replyText = text;
        result.walkedAway = true;
        result.isFinished = true;
        result.resolutionAction = action;
        return SyncState(result, trade);
    }

    private static void EnsureTradeInitialized(LocalTradeState trade, int patience)
    {
        if (trade == null)
        {
            return;
        }

        if (trade.maxBuyerPrice <= 0)
        {
            trade.maxBuyerPrice = Mathf.Max(trade.npcOffer, Mathf.RoundToInt(trade.marketValue * GetMaxMultiplier(trade.buyerPersonality)));
        }
        if (trade.buyerPatience <= 0)
        {
            trade.buyerPatience = Mathf.Max(1, patience);
        }
        else
        {
            trade.buyerPatience = Mathf.Min(trade.buyerPatience, Mathf.Max(0, patience));
        }
        if (trade.buyerTrust <= 0f)
        {
            trade.buyerTrust = GetStartingTrust(trade.buyerPersonality);
        }
        if (trade.buyerFrustration <= 0f)
        {
            trade.buyerFrustration = GetStartingFrustration(trade.buyerPersonality);
        }
        if (trade.buyerDesperation <= 0f)
        {
            trade.buyerDesperation = GetDesperation(trade.buyerPersonality);
        }
        trade.minIncrement = Mathf.Max(2, Mathf.RoundToInt(trade.marketValue * 0.02f));

        if (trade.referencePrice > 0)
        {
            float anchored = (0.7f * trade.npcOffer) + (0.3f * trade.referencePrice);
            trade.npcOffer = Mathf.Min(trade.maxBuyerPrice, Mathf.Max(trade.npcOffer, Mathf.RoundToInt(anchored)));
        }
    }

    private static void UpdateMemory(LocalTradeState trade, NegotiationInput input)
    {
        if (trade == null || input == null)
        {
            return;
        }

        trade.lastNormalizedPlayerInput = input.normalizedText;
        trade.lastIntentName = input.intent.ToString();
        if (input.hasSellerPrice)
        {
            trade.lastSellerPrice = input.sellerPrice;
        }
    }

    private static int ResolveAcceptedPrice(NegotiationInput input, LocalTradeState trade, int currentOffer)
    {
        if (input != null && input.hasSellerPrice)
        {
            return input.sellerPrice;
        }

        if (trade != null && trade.npcOffer > 0)
        {
            return trade.npcOffer;
        }

        if (trade != null && trade.previousNpcOffer > 0)
        {
            return trade.previousNpcOffer;
        }

        return currentOffer;
    }

    private static int MoveTowardTarget(LocalTradeState trade, int sellerPrice, int roundCount)
    {
        if (trade == null)
        {
            return 0;
        }

        if (ShouldHoldPosition(trade, sellerPrice, roundCount))
        {
            return trade.npcOffer;
        }

        int target = Mathf.Min(sellerPrice, trade.maxBuyerPrice);
        int increment = ComputeIncrement(trade, target, roundCount);
        int moved = trade.npcOffer + increment;
        moved = Mathf.Min(moved, target);
        return Mathf.Clamp(moved, trade.npcOffer, trade.maxBuyerPrice);
    }

    private static int ComputeIncrement(LocalTradeState trade, int targetPrice, int roundCount)
    {
        int gap = Mathf.Max(0, targetPrice - trade.npcOffer);
        if (gap <= 0)
        {
            return 0;
        }

        float concessionPercent = GetConcessionPercent(trade, roundCount);
        float quantityMultiplier = GetQuantityMultiplier(trade);
        float trustMultiplier = 0.95f + (trade.buyerTrust * 0.08f);
        float frustrationMultiplier = 1f - (trade.buyerFrustration * 0.08f);
        float raw = gap * concessionPercent * quantityMultiplier * trustMultiplier * frustrationMultiplier;
        int increment = Mathf.Max(GetMinimumVisibleMovement(trade.npcOffer), Mathf.RoundToInt(raw));
        increment = Mathf.Max(increment, Mathf.Min(gap, GetMinimumVisibleMovement(trade.npcOffer)));
        return Mathf.Min(increment, Mathf.Max(GetMinimumVisibleMovement(trade.npcOffer), Mathf.RoundToInt(gap * 0.45f)));
    }

    private static int ComputeBargainIncrement(LocalTradeState trade, int currentOffer, int targetPrice, int roundCount)
    {
        int gap = Mathf.Max(0, targetPrice - currentOffer);
        if (gap <= 0)
        {
            return 0;
        }

        int baseIncrement = ComputeIncrement(trade, targetPrice, roundCount);
        int softBargainFloor = Mathf.Max(GetMinimumVisibleMovement(currentOffer), Mathf.CeilToInt(gap * 0.18f));
        if (trade.softBargainCount <= 1)
        {
            softBargainFloor = Mathf.Max(softBargainFloor, Mathf.CeilToInt(gap * 0.22f));
        }

        if (IsStrict(trade.buyerPersonality))
        {
            softBargainFloor = Mathf.Max(GetMinimumVisibleMovement(currentOffer), Mathf.CeilToInt(softBargainFloor * 0.85f));
        }
        else if (IsFriendly(trade.buyerPersonality))
        {
            softBargainFloor = Mathf.CeilToInt(softBargainFloor * 1.1f);
        }

        return Mathf.Min(gap, Mathf.Max(baseIncrement, softBargainFloor));
    }

    private static bool ShouldHoldPosition(LocalTradeState trade, int targetPrice, int roundCount)
    {
        if (trade == null || targetPrice <= trade.npcOffer)
        {
            return false;
        }

        float gapRatio = (targetPrice - trade.npcOffer) / Mathf.Max(1f, trade.npcOffer);
        bool nearCeiling = trade.npcOffer >= Mathf.RoundToInt(trade.maxBuyerPrice * 0.9f);
        bool repeatedHighPrice = trade.repeatedRejectedPrice >= 1 || trade.tooExpensiveCount >= 2;
        bool lateNegotiation = roundCount >= 4 || trade.counterCount >= 2;

        if (trade.buyerFrustration >= 0.78f)
        {
            return nearCeiling;
        }

        if (nearCeiling)
        {
            return gapRatio >= 0.08f;
        }

        if (repeatedHighPrice && gapRatio >= 0.25f)
        {
            return true;
        }

        if (IsStrict(trade.buyerPersonality) && lateNegotiation && gapRatio >= 0.6f)
        {
            return true;
        }

        return lateNegotiation && gapRatio >= 1f;
    }

    private static bool CanAcceptNow(LocalTradeState trade, int roundCount)
    {
        if (trade == null)
        {
            return false;
        }

        if (trade.hostileCount >= 2)
        {
            return false;
        }

        if (!trade.priceIntroduced)
        {
            return false;
        }

        if (trade.sellerMinPrice > 0 && trade.npcOffer < trade.sellerMinPrice)
        {
            return false;
        }

        if (trade.npcOffer < GetAcceptanceThreshold(trade, roundCount))
        {
            return roundCount >= 3;
        }

        return true;
    }

    private static bool CanFinalizeAcceptance(NegotiationInput input, LocalTradeState trade)
    {
        if (input == null)
        {
            return false;
        }

        if (input.parseConfidence != ParseConfidence.High)
        {
            return false;
        }

        if (input.expectedReplyState == ExpectedReplyState.ExpectQuantity || input.expectedReplyState == ExpectedReplyState.ExpectOfferPrice)
        {
            return false;
        }

        return trade != null && trade.npcOffer > 0;
    }

    private static string GetClarificationLine(NegotiationInput input, LocalTradeState trade, string spiceDescriptor, string quantity, int currentOffer)
    {
        if (input != null)
        {
            if (input.parseReason == ParseReason.FulfillmentExpected)
            {
                return "Our bargain is already set. Bring the goods I asked for to complete the trade.";
            }

            if (input.parseReason == ParseReason.MissingQuantity || input.expectedReplyState == ExpectedReplyState.ExpectQuantity)
            {
                return $"Tell me the quantity plainly, merchant. How much of the {spiceDescriptor} do you mean?";
            }

            if (input.parseReason == ParseReason.StateBlockedAccept || input.parseReason == ParseReason.MissingPrice || input.expectedReplyState == ExpectedReplyState.ExpectOfferPrice)
            {
                return $"State your price clearly, merchant. What do you ask for the {quantity} of {spiceDescriptor}?";
            }

            if (input.parseReason == ParseReason.AmbiguousAcceptOrCounter || input.expectedReplyState == ExpectedReplyState.ExpectAcceptOrCounter)
            {
                return $"Speak plainly. Do you accept my offer of {currentOffer} varahas, or do you want a different price?";
            }

            if (input.intent == NegotiationIntent.CONFUSED || input.parseReason == ParseReason.ConfusedPlayer)
            {
                return $"I will say it again. My offer is {currentOffer} varahas for {quantity} of {spiceDescriptor}.";
            }
        }

        return $"I did not understand you. State your price clearly for the {quantity} of {spiceDescriptor}.";
    }

    private static float GetQuantityMultiplier(LocalTradeState trade)
    {
        if (trade.quantityGrams > 0 && trade.quantityGrams < 200)
        {
            return 0.75f;
        }
        if (trade.quantityGrams > 1000)
        {
            return 1.12f;
        }
        return 1f;
    }

    private static int GetNearAcceptGap(LocalTradeState trade, int roundCount)
    {
        float roundMultiplier = roundCount <= 1 ? 0.75f : roundCount == 2 ? 1f : 1.35f;
        return Mathf.Max(GetMinimumVisibleMovement(trade.npcOffer), Mathf.RoundToInt(trade.minIncrement * roundMultiplier));
    }

    private static int GetMinimumVisibleMovement(int currentOffer)
    {
        return currentOffer >= 100 ? 5 : currentOffer >= 30 ? 2 : 1;
    }

    private static float GetConcessionPercent(LocalTradeState trade, int roundCount)
    {
        bool laterRound = roundCount >= 2;
        if (IsFriendly(trade.buyerPersonality))
        {
            return laterRound ? 0.4f : 0.3f;
        }
        if (IsStrict(trade.buyerPersonality))
        {
            return laterRound ? 0.2f : 0.12f;
        }
        if (IsImpatient(trade.buyerPersonality))
        {
            return laterRound ? 0.38f : 0.28f;
        }
        return laterRound ? 0.3f : 0.2f;
    }

    private static int GetAcceptanceThreshold(LocalTradeState trade, int roundCount)
    {
        float thresholdPercent = roundCount <= 1
            ? (IsFriendly(trade.buyerPersonality) ? 0.97f : IsStrict(trade.buyerPersonality) ? 1f : 0.99f)
            : roundCount == 2
                ? (IsFriendly(trade.buyerPersonality) ? 0.93f : IsStrict(trade.buyerPersonality) ? 0.97f : 0.95f)
                : (IsFriendly(trade.buyerPersonality) ? 0.88f : IsStrict(trade.buyerPersonality) ? 0.93f : 0.9f);

        return Mathf.RoundToInt(trade.maxBuyerPrice * thresholdPercent);
    }

    private static bool ShouldWalkAwayFromHighPrice(LocalTradeState trade, int sellerPrice, int roundCount)
    {
        if (trade == null)
        {
            return false;
        }

        if (sellerPrice <= trade.maxBuyerPrice)
        {
            return false;
        }

        bool repeatedTooHigh = trade.tooExpensiveCount >= 2 || trade.repeatedRejectedPrice >= 2;
        bool exhausted = trade.buyerPatience <= 1 || trade.buyerFrustration >= 0.82f || roundCount >= 5;
        bool farBeyondLimit = sellerPrice >= Mathf.RoundToInt(trade.maxBuyerPrice * 1.35f);
        return repeatedTooHigh || (farBeyondLimit && exhausted);
    }

    private static bool ShouldWalkAwayForNegotiationPressure(LocalTradeState trade, NegotiationInput input, int roundCount)
    {
        if (trade == null)
        {
            return false;
        }

        if (input != null && input.hasHardRejection)
        {
            return true;
        }

        bool enoughActualPriceOffers = trade.actualPriceOfferCount >= 3;
        bool finalRound = roundCount >= 5;
        bool exhausted = trade.buyerPatience <= 0 || trade.buyerFrustration >= 0.86f;
        bool repeatedOverMaxPressure = trade.repeatedOverMaxCount >= 2 || trade.tooExpensiveCount >= 2;
        return repeatedOverMaxPressure || (finalRound && enoughActualPriceOffers && exhausted);
    }

    private static void LogNegotiation(LocalTradeState trade, NegotiationInput input, int previousBuyerPrice, int newBuyerCounter, int roundCount, string decision, string reason = "", int concessionStep = -1)
    {
        int playerOffer = input != null && input.hasSellerPrice ? input.sellerPrice : -1;
        int buyerMaxOffer = trade != null ? trade.maxBuyerPrice : 0;
        int marketValue = trade != null ? trade.marketValue : 0;
        int playerProfitIfAccepted = newBuyerCounter - marketValue;
        float profitMargin = marketValue > 0 ? (float)playerProfitIfAccepted / marketValue : 0f;
        int offerGap = playerOffer > 0 ? playerOffer - previousBuyerPrice : 0;
        int resolvedConcessionStep = concessionStep >= 0 ? concessionStep : Mathf.Max(0, newBuyerCounter - previousBuyerPrice);
        Level1DebugForceAccept.LogTrade("[NEGOTIATION] decision=" + decision +
                                        " | round=" + roundCount +
                                        " | marketValue=" + marketValue +
                                        " | playerProfitIfAccepted=" + playerProfitIfAccepted +
                                        " | sellerPrice=" + playerOffer +
                                        " | previousBuyerOffer=" + previousBuyerPrice +
                                        " | newBuyerOffer=" + newBuyerCounter +
                                        " | buyerMaxOffer=" + buyerMaxOffer +
                                        " | offerGap=" + offerGap +
                                        " | concessionStep=" + resolvedConcessionStep +
                                        " | actualPriceOfferCount=" + (trade != null ? trade.actualPriceOfferCount : 0) +
                                        " | softBargainCount=" + (trade != null ? trade.softBargainCount : 0) +
                                        " | hardRejectCount=" + (trade != null ? trade.hardRejectCount : 0) +
                                        " | repeatedOverMaxCount=" + (trade != null ? trade.repeatedOverMaxCount : 0) +
                                        " | profitMargin=" + profitMargin.ToString("0.00") +
                                        " | personality=" + (trade != null ? trade.buyerPersonality : string.Empty) +
                                        " | walkAwayReason=" + (decision == "WALK_AWAY" ? (string.IsNullOrWhiteSpace(reason) ? "unspecified" : reason) : "n/a") +
                                        (string.IsNullOrWhiteSpace(reason) ? string.Empty : " | reason=" + reason));
    }

    private static void AdjustEmotion(LocalTradeState trade, float frustrationDelta, float trustDelta)
    {
        trade.buyerFrustration = Mathf.Clamp01(trade.buyerFrustration + frustrationDelta);
        trade.buyerTrust = Mathf.Clamp01(trade.buyerTrust + trustDelta);
        if (frustrationDelta > 0f)
        {
            trade.buyerPatience = Mathf.Max(0, trade.buyerPatience - 1);
        }
    }

    private static int GetMaxRejections(string personality, LocalTradeState trade)
    {
        int maxRejections = trade.buyerPatience < 4 ? 1 : 2;
        if (IsStrict(personality))
        {
            maxRejections = 1;
        }
        else if (IsFriendly(personality))
        {
            maxRejections++;
        }
        return maxRejections;
    }

    private static int GetMaxCounters(string personality, LocalTradeState trade)
    {
        int maxCounters = trade.buyerPatience < 3 ? 1 : trade.buyerPatience < 6 ? 2 : 3;
        if (IsStrict(personality))
        {
            maxCounters = Mathf.Max(1, maxCounters - 1);
        }
        else if (IsFriendly(personality))
        {
            maxCounters += 1;
        }
        if (trade.buyerFrustration >= 0.7f)
        {
            maxCounters = Mathf.Max(1, maxCounters - 1);
        }
        return maxCounters;
    }

    private static float GetWalkAwayThreshold(string personality)
    {
        if (IsStrict(personality))
        {
            return 0.85f;
        }
        if (IsFriendly(personality))
        {
            return 0.98f;
        }
        return 0.93f;
    }

    private static string GetGreetingLine(string buyerName, string buyerOrigin, string personality, string quantity, string spiceDescriptor)
    {
        if (IsStrict(personality))
        {
            return $"Good day. I am {buyerName} of {buyerOrigin}. I am here for {quantity} of {spiceDescriptor}.";
        }
        if (IsImpatient(personality))
        {
            return $"I am {buyerName} of {buyerOrigin}. I need {quantity} of {spiceDescriptor} quickly.";
        }
        return $"Greetings. I am {buyerName} of {buyerOrigin}, and I seek {quantity} of {spiceDescriptor}.";
    }

    private static string GetItemQueryLine(LocalTradeState trade, string spiceDescriptor)
    {
        if (trade.repeatedItemQueries > 0)
        {
            trade.repeatedItemQueries++;
            return $"I have already named it. I am here for your {spiceDescriptor}.";
        }

        trade.repeatedItemQueries = 1;
        return $"I am here for your {spiceDescriptor} today, merchant.";
    }

    private static string GetQuantityQueryLine(LocalTradeState trade, string quantity, string spiceDescriptor)
    {
        if (trade.repeatedQuantityQueries > 0)
        {
            trade.repeatedQuantityQueries++;
            return $"The quantity has not changed. I still seek {quantity} of {spiceDescriptor}.";
        }

        trade.repeatedQuantityQueries = 1;
        return $"I seek {quantity} of {spiceDescriptor}, no more.";
    }

    private static string GetSocialLine(string normalizedText, LocalTradeState trade, string buyerName, string buyerOrigin, string personality, string spiceDescriptor)
    {
        if (normalizedText.Contains("weather") || normalizedText.Contains("rain") || normalizedText.Contains("sun"))
        {
            return IsStrict(personality)
                ? $"The skies matter less than the trade. Let us speak of the {spiceDescriptor}."
                : $"The Hampi air is fair for trade, but I am here for {spiceDescriptor}.";
        }
        if (normalizedText.Contains("where") || normalizedText.Contains("origin") || normalizedText.Contains("from"))
        {
            return IsFriendly(personality)
                ? $"I have travelled far from {buyerOrigin} to this bazaar. Now, what is your price for the {spiceDescriptor}?"
                : $"I come from {buyerOrigin}, but the road is less important than the trade.";
        }
        if (normalizedText.Contains("name"))
        {
            return $"I am {buyerName}. Let us return to the matter of {spiceDescriptor}.";
        }
        if (normalizedText.Contains("king") || normalizedText.Contains("temple"))
        {
            return $"Hampi has many wonders, but I came for {spiceDescriptor}. Name your price.";
        }
        return $"Pleasantries can wait, merchant. What is your offer for the {spiceDescriptor}?";
    }

    private static string GetOutOfWorldLine(LocalTradeState trade, string spiceDescriptor)
    {
        if (trade.outOfWorldCount == 1)
        {
            return $"Speak of our trade, not of strange wonders. What is your price for the {spiceDescriptor}?";
        }
        if (trade.outOfWorldCount == 2)
        {
            return $"This is a bazaar, not a hall of riddles. Return to the price of the {spiceDescriptor}.";
        }
        return $"Enough. Speak of the trade, or I leave.";
    }

    private static string GetHostileLine(LocalTradeState trade)
    {
        if (trade.hostileCount == 1)
        {
            return "Speak with respect in this bazaar, or I will take my business elsewhere.";
        }
        if (trade.hostileCount == 2)
        {
            return "You try my patience. One more insult and this trade ends.";
        }
        return "Mind your tongue, merchant. This is your last warning.";
    }

    private static string GetCounterLine(LocalTradeState trade, int sellerPrice, int counterOffer, string spiceDescriptor)
    {
        if (sellerPrice > trade.maxBuyerPrice)
        {
            return $"{sellerPrice} is too steep, merchant. I can raise my offer to {counterOffer} varahas, but not beyond that.";
        }
        if (IsStrict(trade.buyerPersonality))
        {
            return $"Not {sellerPrice}, merchant. I can move only slightly, to {counterOffer} varahas.";
        }
        if (IsFriendly(trade.buyerPersonality))
        {
            return $"{sellerPrice} is high, but you bargain well. I can increase my offer to {counterOffer} varahas for the {spiceDescriptor}.";
        }
        if (IsImpatient(trade.buyerPersonality))
        {
            return $"Let us finish this quickly. Not {sellerPrice}, but I can make it {counterOffer} varahas.";
        }
        return $"We are drawing closer. Not {sellerPrice}, but I can offer {counterOffer} varahas for the {spiceDescriptor}.";
    }

    private static string GetBargainLine(LocalTradeState trade, int nextOffer, string spiceDescriptor)
    {
        if (trade.repeatedBargains > 1)
        {
            return $"I have heard you already. My best movement now is {nextOffer} varahas for the {spiceDescriptor}.";
        }
        if (IsStrict(trade.buyerPersonality))
        {
            return $"This is already fair. I can move only to {nextOffer} varahas.";
        }
        if (IsFriendly(trade.buyerPersonality))
        {
            return $"You bargain well, merchant. I can increase my offer to {nextOffer} varahas.";
        }
        return $"I can raise my offer to {nextOffer} varahas for the {spiceDescriptor}.";
    }

    private static string GetWalkAwayLine(string buyerName, string personality)
    {
        if (IsImpatient(personality))
        {
            return $"{buyerName} will not linger longer. We trade another day.";
        }
        if (IsStrict(personality))
        {
            return "Then we shall not trade today.";
        }
        return $"Very well. {buyerName} will return another day.";
    }

    private static string GetSpiceDescriptor(string spiceKey, string fallback)
    {
        switch ((spiceKey ?? string.Empty).ToLowerInvariant())
        {
            case "pepper":
                return "black pepper";
            case "clove":
                return "clove";
            case "cinnamon":
                return "cinnamon bark";
            case "cardamom":
                return "green cardamom";
            default:
                return fallback;
        }
    }

    private static float GetMaxMultiplier(string personality)
    {
        if (IsStrict(personality)) return 1.18f;
        if (IsFriendly(personality)) return 1.42f;
        if (IsImpatient(personality)) return 1.3f;
        return 1.28f;
    }

    private static float GetStartingTrust(string personality)
    {
        if (IsFriendly(personality)) return 0.65f;
        if (IsStrict(personality)) return 0.35f;
        if (IsImpatient(personality)) return 0.42f;
        return 0.5f;
    }

    private static float GetStartingFrustration(string personality)
    {
        if (IsFriendly(personality)) return 0.05f;
        if (IsStrict(personality)) return 0.12f;
        if (IsImpatient(personality)) return 0.16f;
        return 0.08f;
    }

    private static float GetDesperation(string personality)
    {
        if (IsFriendly(personality)) return 0.55f;
        if (IsStrict(personality)) return 0.4f;
        if (IsImpatient(personality)) return 0.72f;
        return 0.5f;
    }

    private static bool IsFriendly(string personality)
    {
        return string.Equals(personality, "Friendly", System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsStrict(string personality)
    {
        return string.Equals(personality, "Strict", System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsImpatient(string personality)
    {
        return string.Equals(personality, "Impatient", System.StringComparison.OrdinalIgnoreCase);
    }
}
