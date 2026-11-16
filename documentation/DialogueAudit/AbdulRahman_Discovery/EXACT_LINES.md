# Abdul Rahman — exact current source script

Verbatim Unity strings, not proposed rewrites. CORE means functional negotiation coverage, not a frequency statistic. Editor currently forces Father; classifications describe routes when this character is selected. Source/export IDs are ordinal aliases, not serialized runtime IDs. Shared AUD IDs are documentation-only. Examples use one illustrative state and do not form a guaranteed conversation.

## AUTHORED

### 01 Arrival and repeat visit

#### L1_ABD_001 — CustomerGreeting

Marhaban, merchant. I am Abdul Rahman. I wish to buy {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:14`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Marhaban, merchant. I am Abdul Rahman. I wish to buy 2 Seers (~560g) of pepper.
- Existing TTS input example: Marhaban, merchant. I am Abdul Rahman. I wish to buy 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Marhaban, merchant. I am Abdul Rahman. I wish to buy "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_002 — CustomerGreeting

Good day. I am Abdul Rahman, looking for {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:15`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I am Abdul Rahman, looking for 2 Seers (~560g) of pepper.
- Existing TTS input example: Good day. I am Abdul Rahman, looking for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I am Abdul Rahman, looking for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_003 — CustomerGreeting

I have come with the caravan to buy {quantityLabel} of {spiceName}. What is your price?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:16`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have come with the caravan to buy 2 Seers (~560g) of pepper. What is your price?
- Existing TTS input example: I have come with the caravan to buy 2 Seers of pepper. What is your price?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have come with the caravan to buy "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". What is your price?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_004 — CustomerGreeting

Abdul Rahman. I hope your {spiceName} is as inviting as your stall.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:17`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Abdul Rahman. I hope your pepper is as inviting as your stall.
- Existing TTS input example: Abdul Rahman. I hope your pepper is as inviting as your stall.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Abdul Rahman. I hope your "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " is as inviting as your stall."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_005 — RepeatCustomerGreeting

I know your stall now. What do you have for me today?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:19`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_006 — RepeatCustomerGreeting

We meet again, merchant. Let us give this lot its own fair hearing.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:20`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_007 — RepeatCustomerGreeting

A familiar face makes the bargaining easier. What is your price for {spiceName}?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:21`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: A familiar face makes the bargaining easier. What is your price for pepper?
- Existing TTS input example: A familiar face makes the bargaining easier. What is your price for pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "A familiar face makes the bargaining easier. What is your price for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_008 — RepeatCustomerGreeting

Back to your stall for {spiceName}. Shall we begin afresh?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:22`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Back to your stall for pepper. Shall we begin afresh?
- Existing TTS input example: Back to your stall for pepper. Shall we begin afresh?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Back to your stall for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Shall we begin afresh?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 02 Trade information and quantity

#### L1_ABD_009 — AskWhatBuyerWants

I need {quantityLabel} of {spiceName} for caravan trade.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:24`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I need 2 Seers (~560g) of pepper for caravan trade.
- Existing TTS input example: I need 2 Seers of pepper for caravan trade.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " for caravan trade."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_010 — AskWhatBuyerWants

{spiceName}, please, and {quantityLabel} if the measure is sound.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:25`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: pepper, please, and 2 Seers (~560g) if the measure is sound.
- Existing TTS input example: pepper, please, and 2 Seers if the measure is sound.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ", please, and "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " if the measure is sound."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_011 — AskWhatBuyerWants

The lot I came to discuss is {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:26`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: The lot I came to discuss is 2 Seers (~560g) of pepper.
- Existing TTS input example: The lot I came to discuss is 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The lot I came to discuss is "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_012 — AskWhatBuyerWants

I am buying {spiceName}. Let us settle terms for {quantityLabel}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:27`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am buying pepper. Let us settle terms for 2 Seers (~560g).
- Existing TTS input example: I am buying pepper. Let us settle terms for 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am buying "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Let us settle terms for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_013 — AskBuyerBudget

My offer for this lot is {currentBuyerOffer} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:29`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer for this lot is 234 varahas.
- Existing TTS input example: My offer for this lot is 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer for this lot is "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_014 — AskBuyerBudget

I can offer {currentBuyerOffer} {currency} for the {quantityLabel}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:30`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can offer 234 varahas for the 2 Seers (~560g).
- Existing TTS input example: I can offer 234 varahas for the 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can offer "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_015 — AskBuyerBudget

For the goods before us, I have put forward {currentBuyerOffer} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:31`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: For the goods before us, I have put forward 234 varahas.
- Existing TTS input example: For the goods before us, I have put forward 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For the goods before us, I have put forward "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_016 — AskBuyerBudget

You have my current figure: {currentBuyerOffer} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:32`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You have my current figure: 234 varahas.
- Existing TTS input example: You have my current figure: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You have my current figure: "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_017 — AskQuantity

I asked for {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:34`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I asked for 2 Seers (~560g) of pepper.
- Existing TTS input example: I asked for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I asked for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_018 — AskQuantity

The measure is {quantityLabel}, merchant.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:35`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: The measure is 2 Seers (~560g), merchant.
- Existing TTS input example: The measure is 2 Seers , merchant.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The measure is "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": ", merchant."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_019 — AskQuantity

For this purchase, I need {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:36`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: For this purchase, I need 2 Seers (~560g) of pepper.
- Existing TTS input example: For this purchase, I need 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For this purchase, I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_020 — AskQuantity

{quantityLabel} of {spiceName}; that is the lot we are pricing.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:37`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: 2 Seers (~560g) of pepper; that is the lot we are pricing.
- Existing TTS input example: 2 Seers of pepper; that is the lot we are pricing.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "; that is the lot we are pricing."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_021 — AskSellerPrice

I have named the lot. What price do you ask?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:39`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_022 — AskSellerPrice

What is your price for {quantityLabel} of {spiceName}?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:40`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: What is your price for 2 Seers (~560g) of pepper?
- Existing TTS input example: What is your price for 2 Seers of pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "What is your price for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_023 — AskSellerPrice

Let me hear your asking figure, merchant.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:41`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_024 — AskSellerPrice

You know what I wish to buy. Tell me what it will cost.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:42`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_025 — QuantityChanged

Then we are speaking of {quantityLabel} of {spiceName} now. Let us price that lot.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:44`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then we are speaking of 2 Seers (~560g) of pepper now. Let us price that lot.
- Existing TTS input example: Then we are speaking of 2 Seers of pepper now. Let us price that lot.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then we are speaking of "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " now. Let us price that lot."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_026 — QuantityChanged

I understand the new measure: {quantityLabel}. What is your price for it?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:45`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: I understand the new measure: 2 Seers (~560g). What is your price for it?
- Existing TTS input example: I understand the new measure: 2 Seers . What is your price for it?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I understand the new measure: "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": ". What is your price for it?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_027 — QuantityChanged

Very well, {quantityLabel} of {spiceName}. We can bargain on that basis.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:46`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Very well, 2 Seers (~560g) of pepper. We can bargain on that basis.
- Existing TTS input example: Very well, 2 Seers of pepper. We can bargain on that basis.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Very well, "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". We can bargain on that basis."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_028 — QuantityChanged

The quantity has changed to {quantityLabel}; let us keep our figures clear.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:47`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: The quantity has changed to 2 Seers (~560g); let us keep our figures clear.
- Existing TTS input example: The quantity has changed to 2 Seers ; let us keep our figures clear.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The quantity has changed to "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "; let us keep our figures clear."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_070 — OfferReason

I must trade the goods onward. At {currentBuyerOffer} {currency}, there is room for us both.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:100`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I must trade the goods onward. At 234 varahas, there is room for us both.
- Existing TTS input example: I must trade the goods onward. At 234 varahas, there is room for us both.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I must trade the goods onward. At "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", there is room for us both."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_071 — OfferReason

I have considered the measure and the cost of carrying it. That gives me {currentBuyerOffer} {currency}.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:101`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have considered the measure and the cost of carrying it. That gives me 234 varahas.
- Existing TTS input example: I have considered the measure and the cost of carrying it. That gives me 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have considered the measure and the cost of carrying it. That gives me "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_072 — OfferReason

A fair purchase must survive its next sale. That is why I offer {currentBuyerOffer} {currency}.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:102`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: A fair purchase must survive its next sale. That is why I offer 234 varahas.
- Existing TTS input example: A fair purchase must survive its next sale. That is why I offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "A fair purchase must survive its next sale. That is why I offer "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_073 — OfferReason

I want to return to a seller I can trust. {currentBuyerOffer} {currency} is my present offer.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:103`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I want to return to a seller I can trust. 234 varahas is my present offer.
- Existing TTS input example: I want to return to a seller I can trust. 234 varahas is my present offer.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I want to return to a seller I can trust. "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is my present offer."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_074 — AskName

Abdul Rahman. I trade with a caravan and prefer to learn the name of a good seller.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:105`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_075 — AskName

My name is Abdul Rahman, merchant. What may I call you?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:106`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_076 — AskName

Abdul Rahman. Names are worth remembering when bargains are fair.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:107`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_077 — AskName

You may call me Abdul Rahman. I hope we can speak plainly to one another.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:108`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_078 — AskOrigin

I trade with an Arab caravan; our travels have brought me to Vijayanagara.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:110`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_079 — AskOrigin

My work is caravan trade. Vijayanagara is where I am buying today.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:111`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_080 — AskOrigin

I came with a caravan of Arab traders, merchant. The road has brought us here.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:112`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_081 — AskOrigin

I travel with a caravan of traders. For now, our business has brought us to Vijayanagara.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:113`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 03 Negotiation and rejection

#### L1_ABD_029 — SellerPriceTooHigh

{offeredPrice} {currency} is beyond what I can pay for this lot. My offer remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:49`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency, counterPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 varahas is beyond what I can pay for this lot. My offer remains 234 varahas.
- Existing TTS input example: 300 varahas is beyond what I can pay for this lot. My offer remains 234 varahas.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is beyond what I can pay for this lot. My offer remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_030 — SellerPriceTooHigh

I respect your goods, but that asking price leaves me no room to trade onward.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:50`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_031 — SellerPriceTooHigh

We may still talk, though {offeredPrice} {currency} is more than I can justify.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:51`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We may still talk, though 300 varahas is more than I can justify.
- Existing TTS input example: We may still talk, though 300 varahas is more than I can justify.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We may still talk, though "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is more than I can justify."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_032 — SellerPriceTooHigh

I cannot follow that figure. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:52`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I cannot follow that figure. I remain at 234 varahas.
- Existing TTS input example: I cannot follow that figure. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I cannot follow that figure. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_033 — SellerPriceSlightlyHigh

Your asking figure is above mine. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:54`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Your asking figure is above mine. I remain at 234 varahas.
- Existing TTS input example: Your asking figure is above mine. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Your asking figure is above mine. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_034 — SellerPriceSlightlyHigh

There may be a bargain here, but {offeredPrice} {currency} does not work for me.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:55`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: There may be a bargain here, but 300 varahas does not work for me.
- Existing TTS input example: There may be a bargain here, but 300 varahas does not work for me.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "There may be a bargain here, but "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " does not work for me."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_035 — SellerPriceSlightlyHigh

I hear your price. My offer is still {counterPrice} {currency}; can you make a better case?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:56`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I hear your price. My offer is still 234 varahas; can you make a better case?
- Existing TTS input example: I hear your price. My offer is still 234 varahas; can you make a better case?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I hear your price. My offer is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; can you make a better case?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_036 — SellerPriceSlightlyHigh

I would like us both to trade well. At present I can offer {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:57`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I would like us both to trade well. At present I can offer 234 varahas.
- Existing TTS input example: I would like us both to trade well. At present I can offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I would like us both to trade well. At present I can offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_037 — SellerPriceBelowExpected

That price is unexpectedly low. Are you certain of that figure?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:59`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_038 — SellerPriceBelowExpected

You ask less than I expected; such a cheap offer makes a merchant cautious.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:60`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_039 — SellerPriceBelowExpected

A low figure catches my ear, but I must wonder why you part with it so cheaply.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:61`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_040 — SellerPriceBelowExpected

That is below my expectation. Did you truly intend to name so small a sum?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:62`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_041 — BuyerCounterFirst

Let me offer {counterPrice} {currency} for the {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:64`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Let me offer 234 varahas for the 2 Seers (~560g) of pepper.
- Existing TTS input example: Let me offer 234 varahas for the 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let me offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_042 — BuyerCounterFirst

I can begin at {counterPrice} {currency}. Tell me how that sits with you.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:65`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can begin at 234 varahas. Tell me how that sits with you.
- Existing TTS input example: I can begin at 234 varahas. Tell me how that sits with you.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can begin at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Tell me how that sits with you."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_043 — BuyerCounterFirst

For this lot, I put {counterPrice} {currency} before you.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:66`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: For this lot, I put 234 varahas before you.
- Existing TTS input example: For this lot, I put 234 varahas before you.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For this lot, I put "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " before you."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_044 — BuyerCounterFirst

Perhaps {counterPrice} {currency} gives us a good place to begin.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:67`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Perhaps 234 varahas gives us a good place to begin.
- Existing TTS input example: Perhaps 234 varahas gives us a good place to begin.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Perhaps "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " gives us a good place to begin."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_045 — BuyerCounterMiddle

I have heard your reasoning. I can raise my offer to {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:69`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have heard your reasoning. I can raise my offer to 234 varahas.
- Existing TTS input example: I have heard your reasoning. I can raise my offer to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have heard your reasoning. I can raise my offer to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_046 — BuyerCounterMiddle

Let me move to {counterPrice} {currency}; I hope that brings us nearer.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:70`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Let me move to 234 varahas; I hope that brings us nearer.
- Existing TTS input example: Let me move to 234 varahas; I hope that brings us nearer.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let me move to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; I hope that brings us nearer."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_047 — BuyerCounterMiddle

You have persuaded me to offer {counterPrice} {currency} now.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:71`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You have persuaded me to offer 234 varahas now.
- Existing TTS input example: You have persuaded me to offer 234 varahas now.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You have persuaded me to offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " now."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_048 — BuyerCounterMiddle

For the sake of a fair trade, my revised figure is {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:72`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: For the sake of a fair trade, my revised figure is 234 varahas.
- Existing TTS input example: For the sake of a fair trade, my revised figure is 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For the sake of a fair trade, my revised figure is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_049 — BuyerCounterFinal

After hearing you out, I can move to {counterPrice} {currency}. What do you say?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:74`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: After hearing you out, I can move to 234 varahas. What do you say?
- Existing TTS input example: After hearing you out, I can move to 234 varahas. What do you say?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "After hearing you out, I can move to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What do you say?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_050 — BuyerCounterFinal

My offer is now {counterPrice} {currency}; we have both given this thought.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:75`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer is now 234 varahas; we have both given this thought.
- Existing TTS input example: My offer is now 234 varahas; we have both given this thought.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer is now "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; we have both given this thought."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_051 — BuyerCounterFinal

I can raise it to {counterPrice} {currency}. Is there room for agreement?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:76`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can raise it to 234 varahas. Is there room for agreement?
- Existing TTS input example: I can raise it to 234 varahas. Is there room for agreement?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can raise it to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Is there room for agreement?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_052 — BuyerCounterFinal

Let us see whether {counterPrice} {currency} brings our figures together.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:77`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Let us see whether 234 varahas brings our figures together.
- Existing TTS input example: Let us see whether 234 varahas brings our figures together.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let us see whether "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " brings our figures together."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_053 — BuyerHoldsFirm

I value a continuing acquaintance, but my offer remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:79`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I value a continuing acquaintance, but my offer remains 234 varahas.
- Existing TTS input example: I value a continuing acquaintance, but my offer remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I value a continuing acquaintance, but my offer remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_054 — BuyerHoldsFirm

I have listened. I am still at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:80`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have listened. I am still at 234 varahas.
- Existing TTS input example: I have listened. I am still at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have listened. I am still at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_055 — BuyerHoldsFirm

My figure is unchanged: {counterPrice} {currency}. Tell me what I am missing.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:81`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My figure is unchanged: 234 varahas. Tell me what I am missing.
- Existing TTS input example: My figure is unchanged: 234 varahas. Tell me what I am missing.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My figure is unchanged: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Tell me what I am missing."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_056 — BuyerHoldsFirm

Goodwill does not alter the cost of this lot. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:82`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Goodwill does not alter the cost of this lot. I remain at 234 varahas.
- Existing TTS input example: Goodwill does not alter the cost of this lot. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Goodwill does not alter the cost of this lot. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_057 — BuyerHoldsFirm

I would rather agree than argue, but my offer stays at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:83`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I would rather agree than argue, but my offer stays at 234 varahas.
- Existing TTS input example: I would rather agree than argue, but my offer stays at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I would rather agree than argue, but my offer stays at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_066 — PlayerRejectedBuyerOffer

My offer does not suit you. What figure would?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:95`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_067 — PlayerRejectedBuyerOffer

You have refused that price. Tell me what you would ask instead.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:96`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_068 — PlayerRejectedBuyerOffer

Then we differ on the number. Give me one I can consider.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:97`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_069 — PlayerRejectedBuyerOffer

I hear your refusal. Shall we consider another price?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:98`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_128 — BuyerHoldsFirm

I enjoy our talk, though my offer is still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:174`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I enjoy our talk, though my offer is still 234 varahas.
- Existing TTS input example: I enjoy our talk, though my offer is still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I enjoy our talk, though my offer is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_129 — BuyerHoldsFirm

You make good company. My figure remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:175`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You make good company. My figure remains 234 varahas.
- Existing TTS input example: You make good company. My figure remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You make good company. My figure remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_130 — BuyerHoldsFirm

I hope we trade together, but the offer stays at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:176`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I hope we trade together, but the offer stays at 234 varahas.
- Existing TTS input example: I hope we trade together, but the offer stays at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I hope we trade together, but the offer stays at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_131 — BuyerHoldsFirm

We can disagree amiably. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:177`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We can disagree amiably. I remain at 234 varahas.
- Existing TTS input example: We can disagree amiably. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We can disagree amiably. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_132 — BuyerHoldsFirm

I am listening, friend; I have not moved from {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:178`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am listening, friend; I have not moved from 234 varahas.
- Existing TTS input example: I am listening, friend; I have not moved from 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am listening, friend; I have not moved from "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_133 — BuyerHoldsFirm

I remain at {counterPrice} {currency}. What price do you suggest?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:180`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I remain at 234 varahas. What price do you suggest?
- Existing TTS input example: I remain at 234 varahas. What price do you suggest?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What price do you suggest?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_134 — BuyerHoldsFirm

My offer has not changed: {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:181`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer has not changed: 234 varahas.
- Existing TTS input example: My offer has not changed: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer has not changed: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_135 — BuyerHoldsFirm

I have considered it and still offer {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:182`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have considered it and still offer 234 varahas.
- Existing TTS input example: I have considered it and still offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have considered it and still offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_136 — BuyerHoldsFirm

The figure before you remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:183`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The figure before you remains 234 varahas.
- Existing TTS input example: The figure before you remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The figure before you remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_137 — BuyerHoldsFirm

I would hear another reason. For now, {counterPrice} {currency} stands.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:184`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I would hear another reason. For now, 234 varahas stands.
- Existing TTS input example: I would hear another reason. For now, 234 varahas stands.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I would hear another reason. For now, "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " stands."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_138 — BuyerHoldsFirm

I have listened carefully. My offer remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:186`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have listened carefully. My offer remains 234 varahas.
- Existing TTS input example: I have listened carefully. My offer remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have listened carefully. My offer remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_139 — BuyerHoldsFirm

Respectfully, I see no reason to revise {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:187`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Respectfully, I see no reason to revise 234 varahas.
- Existing TTS input example: Respectfully, I see no reason to revise 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Respectfully, I see no reason to revise "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_140 — BuyerHoldsFirm

I must keep to {counterPrice} {currency}. Present another argument if you have one.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:188`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I must keep to 234 varahas. Present another argument if you have one.
- Existing TTS input example: I must keep to 234 varahas. Present another argument if you have one.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I must keep to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Present another argument if you have one."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_141 — BuyerHoldsFirm

My position is unchanged at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:189`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My position is unchanged at 234 varahas.
- Existing TTS input example: My position is unchanged at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My position is unchanged at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_142 — BuyerHoldsFirm

You have my answer: {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:190`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You have my answer: 234 varahas.
- Existing TTS input example: You have my answer: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You have my answer: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_143 — BuyerHoldsFirm

My figure is still {counterPrice} {currency}; may we decide?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:192`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My figure is still 234 varahas; may we decide?
- Existing TTS input example: My figure is still 234 varahas; may we decide?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My figure is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; may we decide?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_144 — BuyerHoldsFirm

The offer remains {counterPrice} {currency}. Shall we decide?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:193`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The offer remains 234 varahas. Shall we decide?
- Existing TTS input example: The offer remains 234 varahas. Shall we decide?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The offer remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Shall we decide?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_145 — BuyerHoldsFirm

I have not raised it. {counterPrice} {currency} stands.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:194`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have not raised it. 234 varahas stands.
- Existing TTS input example: I have not raised it. 234 varahas stands.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have not raised it. "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " stands."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_146 — BuyerHoldsFirm

{counterPrice} {currency}, unchanged. Please be direct.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:195`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: 234 varahas, unchanged. Please be direct.
- Existing TTS input example: 234 varahas, unchanged. Please be direct.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", unchanged. Please be direct."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_147 — BuyerHoldsFirm

My figure is {counterPrice} {currency}. I need your reply.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:196`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My figure is 234 varahas. I need your reply.
- Existing TTS input example: My figure is 234 varahas. I need your reply.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My figure is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". I need your reply."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_148 — BuyerHoldsFirm

My time is short; my offer is still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:198`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My time is short; my offer is still 234 varahas.
- Existing TTS input example: My time is short; my offer is still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My time is short; my offer is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_149 — BuyerHoldsFirm

I remain at {counterPrice} {currency}. Please decide soon.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:199`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I remain at 234 varahas. Please decide soon.
- Existing TTS input example: I remain at 234 varahas. Please decide soon.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please decide soon."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_150 — BuyerHoldsFirm

The figure has not changed: {counterPrice} {currency}. I cannot wait much longer.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:200`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The figure has not changed: 234 varahas. I cannot wait much longer.
- Existing TTS input example: The figure has not changed: 234 varahas. I cannot wait much longer.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The figure has not changed: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". I cannot wait much longer."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_151 — BuyerHoldsFirm

We can continue if you answer promptly. I offer {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:201`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We can continue if you answer promptly. I offer 234 varahas.
- Existing TTS input example: We can continue if you answer promptly. I offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We can continue if you answer promptly. I offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_152 — BuyerHoldsFirm

I am holding at {counterPrice} {currency}. Do you wish to counter?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:202`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am holding at 234 varahas. Do you wish to counter?
- Existing TTS input example: I am holding at 234 varahas. Do you wish to counter?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am holding at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Do you wish to counter?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_153 — SellerPriceTooHigh

I cannot pay {offeredPrice} {currency}. My offer remains {counterPrice} {currency}, and time is short.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:204`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency, counterPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: I cannot pay 300 varahas. My offer remains 234 varahas, and time is short.
- Existing TTS input example: I cannot pay 300 varahas. My offer remains 234 varahas, and time is short.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I cannot pay "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". My offer remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", and time is short."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_154 — SellerPriceTooHigh

That demand is too high. I need a different figure soon.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:205`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_155 — SellerPriceTooHigh

I have little time left; {offeredPrice} {currency} is still beyond me.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:206`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have little time left; 300 varahas is still beyond me.
- Existing TTS input example: I have little time left; 300 varahas is still beyond me.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have little time left; "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is still beyond me."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_156 — BuyerCounterMiddle

I can raise my offer to {counterPrice} {currency}. Please answer soon.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:208`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can raise my offer to 234 varahas. Please answer soon.
- Existing TTS input example: I can raise my offer to 234 varahas. Please answer soon.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can raise my offer to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please answer soon."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_157 — BuyerCounterMiddle

My revised figure is {counterPrice} {currency}. I cannot linger.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:209`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My revised figure is 234 varahas. I cannot linger.
- Existing TTS input example: My revised figure is 234 varahas. I cannot linger.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My revised figure is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". I cannot linger."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_158 — BuyerCounterMiddle

I have moved to {counterPrice} {currency}. Shall we decide?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:210`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have moved to 234 varahas. Shall we decide?
- Existing TTS input example: I have moved to 234 varahas. Shall we decide?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have moved to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Shall we decide?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 04 Price agreement, goods pending

#### L1_ABD_058 — SellerPriceAccepted

We agree on {finalPrice} {currency} for {quantityLabel} of {spiceName}. Please bring the goods.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:85`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: We agree on 234 varahas for 2 Seers (~560g) of pepper. Please bring the goods.
- Existing TTS input example: We agree on 234 varahas for 2 Seers of pepper. Please bring the goods.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We agree on "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Please bring the goods."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_059 — SellerPriceAccepted

That price works for me: {finalPrice} {currency} for {quantityLabel} of {spiceName}. Please prepare the goods.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:86`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: That price works for me: 234 varahas for 2 Seers (~560g) of pepper. Please prepare the goods.
- Existing TTS input example: That price works for me: 234 varahas for 2 Seers of pepper. Please prepare the goods.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That price works for me: "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Please prepare the goods."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_060 — SellerPriceAccepted

I accept the terms, {finalPrice} {currency} and {quantityLabel} of {spiceName}. The goods are still to come.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:87`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I accept the terms, 234 varahas and 2 Seers (~560g) of pepper. The goods are still to come.
- Existing TTS input example: I accept the terms, 234 varahas and 2 Seers of pepper. The goods are still to come.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I accept the terms, "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " and "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". The goods are still to come."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_061 — SellerPriceAccepted

We have a bargain at {finalPrice} {currency}. Bring the {quantityLabel} of {spiceName}, please.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:88`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: We have a bargain at 234 varahas. Bring the 2 Seers (~560g) of pepper, please.
- Existing TTS input example: We have a bargain at 234 varahas. Bring the 2 Seers of pepper, please.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We have a bargain at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Bring the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ", please."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_062 — PlayerAcceptedDeal

Then we are agreed at {finalPrice} {currency} for {quantityLabel} of {spiceName}. Bring the lot over.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:90`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then we are agreed at 234 varahas for 2 Seers (~560g) of pepper. Bring the lot over.
- Existing TTS input example: Then we are agreed at 234 varahas for 2 Seers of pepper. Bring the lot over.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then we are agreed at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Bring the lot over."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_063 — PlayerAcceptedDeal

Our price is settled: {finalPrice} {currency} for {quantityLabel} of {spiceName}. I await the goods.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:91`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Our price is settled: 234 varahas for 2 Seers (~560g) of pepper. I await the goods.
- Existing TTS input example: Our price is settled: 234 varahas for 2 Seers of pepper. I await the goods.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Our price is settled: "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". I await the goods."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_064 — PlayerAcceptedDeal

I accept those terms. {finalPrice} {currency}, {quantityLabel} of {spiceName}; please weigh it out.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:92`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I accept those terms. 234 varahas, 2 Seers (~560g) of pepper; please weigh it out.
- Existing TTS input example: I accept those terms. 234 varahas, 2 Seers of pepper; please weigh it out.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I accept those terms. "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "; please weigh it out."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_065 — PlayerAcceptedDeal

A fair understanding: {quantityLabel} of {spiceName} at {finalPrice} {currency}. Please bring it here.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:93`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName, finalPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: A fair understanding: 2 Seers (~560g) of pepper at 234 varahas. Please bring it here.
- Existing TTS input example: A fair understanding: 2 Seers of pepper at 234 varahas. Please bring it here.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "A fair understanding: "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please bring it here."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 05 Social, history and conduct

#### L1_ABD_082 — WeatherQuestion

A clear sky is welcome on a trading day. It does not settle a price for us.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:115`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_083 — WeatherQuestion

Rain teaches a trader to protect the goods before praising the bargain.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:116`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_084 — WeatherQuestion

The weather may change before evening. Let us judge the {spiceName} while it is here.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:117`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: The weather may change before evening. Let us judge the pepper while it is here.
- Existing TTS input example: The weather may change before evening. Let us judge the pepper while it is here.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The weather may change before evening. Let us judge the "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " while it is here."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_085 — HistoryKing

Krishnadevaraya rules here. I know the market better than I know his court.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:119`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_086 — HistoryKing

The king is Krishnadevaraya. Ask the courtiers about policy; I can speak about trade.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:120`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_087 — HistoryKing

Krishnadevaraya is the ruler, merchant. The bazaar has its own lively conversations.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:121`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_088 — HistoryTemple

That is Virupaksha's temple. The bazaar nearby is familiar to anyone who trades here.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:123`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_089 — HistoryTemple

You mean Virupaksha's temple. Follow the market toward it and you will see it.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:124`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_090 — HistoryTemple

Virupaksha's temple stands by a busy bazaar. Traders know the way well.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:125`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_091 — HistoryQuestion

Vijayanagara gathers traders with many stories. I can tell you what I have seen in its market.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:127`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_092 — HistoryQuestion

A city reveals itself in its markets, though no single trader knows every street.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:128`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_093 — HistoryQuestion

I have learned something of this city by listening to the people who sell here.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:129`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_094 — HistoryQuestion

Ask me about trading in Vijayanagara; I would rather not pretend to know its whole history.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:130`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_095 — SocialGreeting

Marhaban. It is good to exchange a word before exchanging prices.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:132`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_096 — SocialGreeting

You are courteous, merchant. That makes even a difficult bargain easier to discuss.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:133`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_097 — SocialGreeting

A pleasant conversation is welcome. We can return to the lot when you are ready.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:134`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_098 — SocialGreeting

I am glad to talk. What is on your mind?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:135`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_099 — OffTopic

That lies beyond what I know. Shall we return to the {spiceName}?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:137`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: That lies beyond what I know. Shall we return to the pepper?
- Existing TTS input example: That lies beyond what I know. Shall we return to the pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That lies beyond what I know. Shall we return to the "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_100 — OffTopic

You have surprised me with that question. The lot before us is more familiar ground.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:138`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_101 — OffTopic

I cannot give you a useful answer to that. I can answer for my offer.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:139`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_102 — OffTopic

Another conversation, perhaps. Today I came to discuss {spiceName}.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:140`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Another conversation, perhaps. Today I came to discuss pepper.
- Existing TTS input example: Another conversation, perhaps. Today I came to discuss pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Another conversation, perhaps. Today I came to discuss "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_103 — HostileWarning

I have shown you courtesy. I ask the same in return.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:142`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_104 — HostileWarning

I will discuss the price, merchant. I will not trade insults.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:143`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_105 — HostileWarning

Speak to me as a fellow trader, and we may still do business.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:144`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_159 — SocialGreeting

I appreciate the courtesy. I do need to finish this purchase soon.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:212`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_160 — SocialGreeting

Thank you for the kind word. What is your answer on the trade?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:213`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_161 — SocialGreeting

I would talk longer another day. Today my time is short.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:214`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_165 — SocialGreeting

Your courtesy is heard. Let us keep our terms equally clear.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:220`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_166 — SocialGreeting

I appreciate the greeting. Now tell me what you mean to offer.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:221`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_167 — SocialGreeting

I will speak plainly, merchant. Please do the same.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:222`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_168 — SocialGreeting

Thank you. I need to settle this purchase soon.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:224`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_169 — SocialGreeting

I hear you. Now, where do we stand on the trade?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:225`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_170 — SocialGreeting

We can speak at length later. I need your answer now.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:226`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 06 Clarification and precise continuation

#### L1_ABD_106 — UnclearSpeech

Help me understand one point: {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:146`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: Help me understand one point: The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: Help me understand one point: The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Help me understand one point: "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_107 — UnclearSpeech

I would rather ask than assume. {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:147`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: I would rather ask than assume. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: I would rather ask than assume. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I would rather ask than assume. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_108 — UnclearSpeech

Before I answer, let us make this clear. {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:148`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: Before I answer, let us make this clear. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: Before I answer, let us make this clear. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Before I answer, let us make this clear. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_109 — UnclearSpeech

I missed the precise terms. {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:149`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: I missed the precise terms. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: I missed the precise terms. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I missed the precise terms. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_110 — Unknown

{ruleReply}

- Classification: **REACHABLE_OPTIONAL** / CONDITIONAL_RESIDUAL
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:150`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. MapScenario residual outcome; e.g. nonfinal ACCEPT suspicious-price guard without a price on the input. Not equivalent to input UNKNOWN (which maps UnclearSpeech).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 07 Waiting, completion and departure

#### L1_ABD_111 — TransactionSuccess

Shukran, merchant. The goods are here, and you have kept your word.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:152`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_112 — TransactionSuccess

The {spiceName} is delivered as agreed. Thank you for a fair exchange.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:153`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: The pepper is delivered as agreed. Thank you for a fair exchange.
- Existing TTS input example: The pepper is delivered as agreed. Thank you for a fair exchange.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " is delivered as agreed. Thank you for a fair exchange."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_113 — TransactionSuccess

Goods received, and my thanks. I would be glad to meet you at this stall again.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:154`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_114 — TransactionSuccess

You dealt openly and delivered the lot. I will remember that.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:155`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_115 — TransactionFailure

We have no bargain today. I will take my business to another stall.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:157`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_116 — TransactionFailure

Our prices do not meet. I will leave the lot with you.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:158`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_117 — TransactionFailure

I must go without a purchase. I wish you better trade ahead.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:159`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_118 — TransactionFailure

No agreement this time. I will return to the caravan.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:160`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_119 — TimePressure

I have other buying to do. Can you give me an answer soon?

- Classification: **UNREACHABLE_OR_DEFERRED** / UNROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:162`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_120 — TimePressure

We have spoken enough to decide on a figure, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / UNROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:163`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_121 — TimePressure

My time in this market is limited. Where do you stand?

- Classification: **UNREACHABLE_OR_DEFERRED** / UNROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:164`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_122 — IdleReminderFirst

Merchant? I am still here, if you have a figure for me.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:166`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_123 — IdleReminderFirst

Take a moment. I would like to hear your answer.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:167`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_124 — IdleReminderFirst

Shall we continue with the {spiceName}?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:168`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Shall we continue with the pepper?
- Existing TTS input example: Shall we continue with the pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Shall we continue with the "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_125 — IdleReminderSecond

I have waited a while. Are we to continue this bargain?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:170`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_126 — IdleReminderSecond

Please tell me where you stand; I have other business today.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:171`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_127 — IdleReminderSecond

Silence will not bring our figures together. What is your answer?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:172`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_162 — TransactionFailure

Our figures do not meet, and I must leave now.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:216`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_163 — TransactionFailure

I have no more time for this lot. I will try another stall.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:217`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_ABD_164 — TransactionFailure

No bargain today. I must return to my companions.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/AbdulRahmanDialogue.cs:218`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

## BRAIN_FALLBACK

### 02 Trade information and quantity

#### AUD_ABD_C63A5FE7E70477 — AskBuyerBudget

You ask well. I seek {quantity} of {spiceDescriptor}, and I am offering {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:135`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ITEM_QUERY with asksCurrentOffer.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor, currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: You ask well. I seek 2 Seers (~560g) of black pepper, and I am offering 234 varahas.
- Existing TTS input example: You ask well. I seek 2 Seers of black pepper, and I am offering 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You ask well. I seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ", and I am offering "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_9AC21C5FCF67DB — AskSellerPrice

I seek {quantity} of {spiceDescriptor}. Tell me your price, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:140`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ITEM_QUERY before price introduced.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I seek 2 Seers (~560g) of black pepper. Tell me your price, merchant.
- Existing TTS input example: I seek 2 Seers of black pepper. Tell me your price, merchant.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ". Tell me your price, merchant."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_08A6999373A1A1 — AskBuyerBudget

I can offer {currentOffer} varahas for {quantity} of {spiceDescriptor}. That is fair for today's bazaar.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:156`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. QUERY_BUYER_BUDGET.
- Restrictions: {}
- Placeholders: currentOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can offer 234 varahas for 2 Seers (~560g) of black pepper. That is fair for today's bazaar.
- Existing TTS input example: I can offer 234 varahas for 2 Seers of black pepper. That is fair for today's bazaar.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can offer "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ". That is fair for today's bazaar."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_A56530B7C3D97A — AskBuyerBudget

My offer stands at {currentOffer} varahas for {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:190`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. CONTINUE.
- Restrictions: {}
- Placeholders: currentOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer stands at 234 varahas for 2 Seers (~560g) of black pepper.
- Existing TTS input example: My offer stands at 234 varahas for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer stands at "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_189D44784A9EF1 — QuantityChanged

For the smaller quantity of {quantity}, my offer remains {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:246`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. QUANTITY_CHANGE smaller than old quantity.
- Restrictions: {}
- Placeholders: quantity, currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: For the smaller quantity of 2 Seers (~560g), my offer remains 234 varahas.
- Existing TTS input example: For the smaller quantity of 2 Seers , my offer remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For the smaller quantity of "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": ", my offer remains "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_DDFBAD2470E8E5 — QuantityChanged

For {quantity} of {spiceDescriptor}, I can still begin at {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:247`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other QUANTITY_CHANGE.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor, currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: For 2 Seers (~560g) of black pepper, I can still begin at 234 varahas.
- Existing TTS input example: For 2 Seers of black pepper, I can still begin at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ", I can still begin at "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_9CD31497DD9020 — AskBuyerBudget

I seek {quantity} of {spiceDescriptor}, and I am offering {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:440`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. PRICE_QUERY asksItem.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor, currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: I seek 2 Seers (~560g) of black pepper, and I am offering 234 varahas.
- Existing TTS input example: I seek 2 Seers of black pepper, and I am offering 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ", and I am offering "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_C4CDD001485665 — OfferReason

My offer is {currentOffer} varahas because I must still carry the {spiceDescriptor} onward and sell at a profit.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:444`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. PRICE_QUERY asksReason but not asksItem.
- Restrictions: {}
- Placeholders: currentOffer, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer is 234 varahas because I must still carry the black pepper onward and sell at a profit.
- Existing TTS input example: My offer is 234 varahas because I must still carry the black pepper onward and sell at a profit.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer is "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas because I must still carry the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " onward and sell at a profit."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_17CD21551CCF79 — AskBuyerBudget

I am offering {currentOffer} varahas for {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:448`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other PRICE_QUERY.
- Restrictions: {}
- Placeholders: currentOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am offering 234 varahas for 2 Seers (~560g) of black pepper.
- Existing TTS input example: I am offering 234 varahas for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am offering "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_D3484AE0E45CE7 — AskWhatBuyerWants

I have already named it. I am here for your {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:970`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Repeated ITEM_QUERY helper.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have already named it. I am here for your black pepper.
- Existing TTS input example: I have already named it. I am here for your black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have already named it. I am here for your "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_709F2F473439DE — AskWhatBuyerWants

I am here for your {spiceDescriptor} today, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:974`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. First ITEM_QUERY helper.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am here for your black pepper today, merchant.
- Existing TTS input example: I am here for your black pepper today, merchant.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am here for your "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " today, merchant."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_C83BA8FA61D43B — AskQuantity

The quantity has not changed. I still seek {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:982`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Repeated QUANTITY_QUERY.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: The quantity has not changed. I still seek 2 Seers (~560g) of black pepper.
- Existing TTS input example: The quantity has not changed. I still seek 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The quantity has not changed. I still seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_83015BA9D16215 — AskQuantity

I seek {quantity} of {spiceDescriptor}, no more.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:986`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. First QUANTITY_QUERY.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I seek 2 Seers (~560g) of black pepper, no more.
- Existing TTS input example: I seek 2 Seers of black pepper, no more.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ", no more."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_5D32B694688E69 — AskOrigin

I have travelled far from {buyerOrigin} to this bazaar. Now, what is your price for the {spiceDescriptor}?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1000`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Origin helper Friendly.
- Restrictions: {}
- Placeholders: buyerOrigin, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have travelled far from Arab Caravan Trader to this bazaar. Now, what is your price for the black pepper?
- Existing TTS input example: I have travelled far from Arab Caravan Trader to this bazaar. Now, what is your price for the black pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have travelled far from "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": " to this bazaar. Now, what is your price for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_2827BFF6D67948 — AskOrigin

I come from {buyerOrigin}, but the road is less important than the trade.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1001`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Origin helper not Friendly.
- Restrictions: {}
- Placeholders: buyerOrigin; type: DYNAMIC_TEMPLATE
- Raw Unity example: I come from Arab Caravan Trader, but the road is less important than the trade.
- Existing TTS input example: I come from Arab Caravan Trader, but the road is less important than the trade.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I come from "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ", but the road is less important than the trade."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_F06756EC52D27F — AskName

I am {buyerName}. Let us return to the matter of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1005`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Name helper.
- Restrictions: {}
- Placeholders: buyerName, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am Abdul Rahman. Let us return to the matter of black pepper.
- Existing TTS input example: I am Abdul Rahman. Let us return to the matter of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ". Let us return to the matter of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 03 Negotiation and rejection

#### AUD_ABD_075B5AF44A29A0 — COUNTER_OR_HOLD

Then hear my better offer: {result.updatedOffer} varahas for {quantity} of {spiceDescriptor}.

- Classification: **UNREACHABLE_OR_DEFERRED** / PREEMPTED_BRANCH
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:211`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. REJECT raises offer and hasHardRejection.
- Restrictions: {}
- Placeholders: result.updatedOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then hear my better offer: 234 varahas for 2 Seers (~560g) of black pepper.
- Existing TTS input example: Then hear my better offer: 234 varahas for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then hear my better offer: "}, {"kind": "VALUE", "text": "{result.updatedOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_A3DBE0A9B44DBC — COUNTER_OR_HOLD

If not that, I can improve it to {result.updatedOffer} varahas for {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:212`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. REJECT raises offer without hard rejection.
- Restrictions: {}
- Placeholders: result.updatedOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: If not that, I can improve it to 234 varahas for 2 Seers (~560g) of black pepper.
- Existing TTS input example: If not that, I can improve it to 234 varahas for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "If not that, I can improve it to "}, {"kind": "VALUE", "text": "{result.updatedOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_86287139B6D934 — PlayerRejectedBuyerOffer

Then we are too far apart, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:213`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. REJECT does not raise offer; hard -> too far apart, soft -> name a better price.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_08EDE5E41CA7DE — PlayerRejectedBuyerOffer

If that price does not suit you, name a better one.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:213`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. REJECT does not raise offer; hard -> too far apart, soft -> name a better price.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_74E8E5C14FFFFB — COUNTER_OR_HOLD

{sellerPrice} is beyond what I can pay. My strongest offer is {result.updatedOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:272`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Ultimatum above max, still negotiable.
- Restrictions: {}
- Placeholders: sellerPrice, result.updatedOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 is beyond what I can pay. My strongest offer is 234 varahas.
- Existing TTS input example: 300 is beyond what I can pay. My strongest offer is 234 varahas.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": " is beyond what I can pay. My strongest offer is "}, {"kind": "VALUE", "text": "{result.updatedOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_0D993A3B0F2316 — BuyerHoldsFirm

This is my limit. I can hold at {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:279`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Ultimatum ShouldHoldPosition.
- Restrictions: {}
- Placeholders: currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: This is my limit. I can hold at 234 varahas.
- Existing TTS input example: This is my limit. I can hold at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "This is my limit. I can hold at "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_5444A33783C835 — COUNTER_OR_HOLD

You press hard, merchant. I can move to {result.updatedOffer} varahas, but not beyond that.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:285`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other valid ultimatum.
- Restrictions: {}
- Placeholders: result.updatedOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: You press hard, merchant. I can move to 234 varahas, but not beyond that.
- Existing TTS input example: You press hard, merchant. I can move to 234 varahas, but not beyond that.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You press hard, merchant. I can move to "}, {"kind": "VALUE", "text": "{result.updatedOffer}"}, {"kind": "LITERAL", "text": " varahas, but not beyond that."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_32CD55BB9046A0 — COUNTER_OR_HOLD

I can increase my offer slightly to {result.updatedOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:298`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. COUNTER without explicit price.
- Restrictions: {}
- Placeholders: result.updatedOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can increase my offer slightly to 234 varahas.
- Existing TTS input example: I can increase my offer slightly to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can increase my offer slightly to "}, {"kind": "VALUE", "text": "{result.updatedOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_A90EC2D785816A — SellerPriceBelowExpected

That is far too low for honest trade. My offer remains {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:328`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit price below honest trade floor; nonterminal.
- Restrictions: {}
- Placeholders: currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: That is far too low for honest trade. My offer remains 234 varahas.
- Existing TTS input example: That is far too low for honest trade. My offer remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That is far too low for honest trade. My offer remains "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_70D7BBF02A0D7D — SellerPriceBelowExpected

Your price meets my offer, yet I will hold at {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:377`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Price below current offer but acceptance gate not satisfied (no acceptance).
- Restrictions: {}
- Placeholders: currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: Your price meets my offer, yet I will hold at 234 varahas.
- Existing TTS input example: Your price meets my offer, yet I will hold at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Your price meets my offer, yet I will hold at "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_64CE47AE1AD26B — COUNTER_OR_HOLD

I have already refused {sellerPrice}. Do not test my patience.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:406`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Repeated identical over-max price; early refusal.
- Restrictions: {}
- Placeholders: sellerPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have already refused 300. Do not test my patience.
- Existing TTS input example: I have already refused 300. Do not test my patience.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have already refused "}, {"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": ". Do not test my patience."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_2CFB9D3772F850 — COUNTER_OR_HOLD

{sellerPrice} is too high, merchant. I can only hold at {currentOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:413`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Above-max seller price with buyer near ceiling.
- Restrictions: {}
- Placeholders: sellerPrice, currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 is too high, merchant. I can only hold at 234 varahas.
- Existing TTS input example: 300 is too high, merchant. I can only hold at 234 varahas.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": " is too high, merchant. I can only hold at "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_D46EB55E93C1CE — BuyerHoldsFirm

I can improve a little, but not much.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:433`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. BARGAIN fails to raise offer.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_A46A5680D97648 — COUNTER_OR_HOLD

{sellerPrice} is too steep, merchant. I can raise my offer to {counterOffer} varahas, but not beyond that.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1044`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetCounterLine sellerPrice>max.
- Restrictions: {}
- Placeholders: sellerPrice, counterOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 is too steep, merchant. I can raise my offer to 234 varahas, but not beyond that.
- Existing TTS input example: 300 is too steep, merchant. I can raise my offer to 234 varahas, but not beyond that.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": " is too steep, merchant. I can raise my offer to "}, {"kind": "VALUE", "text": "{counterOffer}"}, {"kind": "LITERAL", "text": " varahas, but not beyond that."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_19F4DD41830DE2 — COUNTER_OR_HOLD

Not {sellerPrice}, merchant. I can move only slightly, to {counterOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1048`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Counter Strict.
- Restrictions: {}
- Placeholders: sellerPrice, counterOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: Not 300, merchant. I can move only slightly, to 234 varahas.
- Existing TTS input example: Not 300, merchant. I can move only slightly, to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Not "}, {"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": ", merchant. I can move only slightly, to "}, {"kind": "VALUE", "text": "{counterOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_71A0A6FB482F44 — COUNTER_OR_HOLD

{sellerPrice} is high, but you bargain well. I can increase my offer to {counterOffer} varahas for the {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1052`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Counter Friendly.
- Restrictions: {}
- Placeholders: sellerPrice, counterOffer, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 is high, but you bargain well. I can increase my offer to 234 varahas for the black pepper.
- Existing TTS input example: 300 is high, but you bargain well. I can increase my offer to 234 varahas for the black pepper.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": " is high, but you bargain well. I can increase my offer to "}, {"kind": "VALUE", "text": "{counterOffer}"}, {"kind": "LITERAL", "text": " varahas for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_05F0CCD5C5C181 — COUNTER_OR_HOLD

Let us finish this quickly. Not {sellerPrice}, but I can make it {counterOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1056`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Counter Impatient.
- Restrictions: {}
- Placeholders: sellerPrice, counterOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: Let us finish this quickly. Not 300, but I can make it 234 varahas.
- Existing TTS input example: Let us finish this quickly. Not 300, but I can make it 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let us finish this quickly. Not "}, {"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": ", but I can make it "}, {"kind": "VALUE", "text": "{counterOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_23A964DEF93E0B — COUNTER_OR_HOLD

We are drawing closer. Not {sellerPrice}, but I can offer {counterOffer} varahas for the {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1058`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Counter Normal.
- Restrictions: {}
- Placeholders: sellerPrice, counterOffer, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: We are drawing closer. Not 300, but I can offer 234 varahas for the black pepper.
- Existing TTS input example: We are drawing closer. Not 300, but I can offer 234 varahas for the black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We are drawing closer. Not "}, {"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": ", but I can offer "}, {"kind": "VALUE", "text": "{counterOffer}"}, {"kind": "LITERAL", "text": " varahas for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_54D489724E7990 — COUNTER_OR_HOLD

I have heard you already. My best movement now is {nextOffer} varahas for the {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1065`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. BARGAIN repeatedBargains>1.
- Restrictions: {}
- Placeholders: nextOffer, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have heard you already. My best movement now is 234 varahas for the black pepper.
- Existing TTS input example: I have heard you already. My best movement now is 234 varahas for the black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have heard you already. My best movement now is "}, {"kind": "VALUE", "text": "{nextOffer}"}, {"kind": "LITERAL", "text": " varahas for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_37FCBD8C4FA025 — COUNTER_OR_HOLD

This is already fair. I can move only to {nextOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1069`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. BARGAIN Strict.
- Restrictions: {}
- Placeholders: nextOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: This is already fair. I can move only to 234 varahas.
- Existing TTS input example: This is already fair. I can move only to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "This is already fair. I can move only to "}, {"kind": "VALUE", "text": "{nextOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_20F1F3BE77A233 — COUNTER_OR_HOLD

You bargain well, merchant. I can increase my offer to {nextOffer} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1073`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. BARGAIN Friendly.
- Restrictions: {}
- Placeholders: nextOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: You bargain well, merchant. I can increase my offer to 234 varahas.
- Existing TTS input example: You bargain well, merchant. I can increase my offer to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You bargain well, merchant. I can increase my offer to "}, {"kind": "VALUE", "text": "{nextOffer}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_A65DC6F88ED41F — COUNTER_OR_HOLD

I can raise my offer to {nextOffer} varahas for the {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1075`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other BARGAIN.
- Restrictions: {}
- Placeholders: nextOffer, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can raise my offer to 234 varahas for the black pepper.
- Existing TTS input example: I can raise my offer to 234 varahas for the black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can raise my offer to "}, {"kind": "VALUE", "text": "{nextOffer}"}, {"kind": "LITERAL", "text": " varahas for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 04 Price agreement, goods pending

#### AUD_ABD_82B079B5FFBD13 — SellerPriceAccepted

Our bargain is already set. Bring the agreed goods to complete the trade.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:47`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. AcceptedTerms already bound; brain returns finished ACCEPT; ordinarily authored agreement replaces this.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_43A5EEC613A15A — PlayerAcceptedDeal

Agreed. {result.acceptedTerms.QuantityLabel} of {result.acceptedTerms.SpiceName} for {result.acceptedTerms.Price} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:236`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Validated ACCEPT binds input.acceptedOffer snapshot.
- Restrictions: {}
- Placeholders: result.acceptedTerms.QuantityLabel, result.acceptedTerms.SpiceName, result.acceptedTerms.Price; type: DYNAMIC_TEMPLATE
- Raw Unity example: Agreed. 2 Seers (~560g) of Pepper for 234 varahas.
- Existing TTS input example: Agreed. 2 Seers of Pepper for 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Agreed. "}, {"kind": "VALUE", "text": "{result.acceptedTerms.QuantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{result.acceptedTerms.SpiceName}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{result.acceptedTerms.Price}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_98C3546B16B711 — SellerPriceAccepted

Agreed. {quantity} of {spiceDescriptor} for {sellerPrice} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:353`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Seller price exactly equals current offer, after suspicious-price guards.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor, sellerPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Agreed. 2 Seers (~560g) of black pepper for 300 varahas.
- Existing TTS input example: Agreed. 2 Seers of black pepper for 300 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Agreed. "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording. Additional source unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:368; Price below current offer and CanAcceptNow plus valid accepted terms.

#### AUD_ABD_6982E6BDC3A064 — SellerPriceAccepted

We are close enough. Agreed at {sellerPrice} varahas.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:388`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Within acceptance threshold and near-accept gap, with valid terms.
- Restrictions: {}
- Placeholders: sellerPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: We are close enough. Agreed at 300 varahas.
- Existing TTS input example: We are close enough. Agreed at 300 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We are close enough. Agreed at "}, {"kind": "VALUE", "text": "{sellerPrice}"}, {"kind": "LITERAL", "text": " varahas."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 05 Social, history and conduct

#### AUD_ABD_E80439342ECFCE — SocialGreeting

Good day. I am {buyerName} of {buyerOrigin}. I am here for {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:956`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetGreetingLine Strict.
- Restrictions: {}
- Placeholders: buyerName, buyerOrigin, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I am Abdul Rahman of Arab Caravan Trader. I am here for 2 Seers (~560g) of black pepper.
- Existing TTS input example: Good day. I am Abdul Rahman of Arab Caravan Trader. I am here for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ". I am here for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_7F6EA122E7C378 — SocialGreeting

I am {buyerName} of {buyerOrigin}. I need {quantity} of {spiceDescriptor} quickly.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:960`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetGreetingLine Impatient.
- Restrictions: {}
- Placeholders: buyerName, buyerOrigin, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am Abdul Rahman of Arab Caravan Trader. I need 2 Seers (~560g) of black pepper quickly.
- Existing TTS input example: I am Abdul Rahman of Arab Caravan Trader. I need 2 Seers of black pepper quickly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ". I need "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " quickly."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_0D33775EF08194 — SocialGreeting

Greetings. I am {buyerName} of {buyerOrigin}, and I seek {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:962`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other greeting personality.
- Restrictions: {}
- Placeholders: buyerName, buyerOrigin, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings. I am Abdul Rahman of Arab Caravan Trader, and I seek 2 Seers (~560g) of black pepper.
- Existing TTS input example: Greetings. I am Abdul Rahman of Arab Caravan Trader, and I seek 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ", and I seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_0912CBDA7FB632 — WeatherQuestion

The skies matter less than the trade. Let us speak of the {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:994`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Weather/social helper Strict.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: The skies matter less than the trade. Let us speak of the black pepper.
- Existing TTS input example: The skies matter less than the trade. Let us speak of the black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The skies matter less than the trade. Let us speak of the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_2BA7CC0D81122E — WeatherQuestion

The Hampi air is fair for trade, but I am here for {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:995`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Weather/social helper other personality.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: The Hampi air is fair for trade, but I am here for black pepper.
- Existing TTS input example: The Hampi air is fair for trade, but I am here for black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The Hampi air is fair for trade, but I am here for "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_F0C84A74979BFB — HistoryQuestion

Hampi has many wonders, but I came for {spiceDescriptor}. Name your price.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1009`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. King/temple helper.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Hampi has many wonders, but I came for black pepper. Name your price.
- Existing TTS input example: Hampi has many wonders, but I came for black pepper. Name your price.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Hampi has many wonders, but I came for "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": ". Name your price."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_5CF99A8F9A79A8 — SocialGreeting

Pleasantries can wait, merchant. What is your offer for the {spiceDescriptor}?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1011`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other social helper.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Pleasantries can wait, merchant. What is your offer for the black pepper?
- Existing TTS input example: Pleasantries can wait, merchant. What is your offer for the black pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Pleasantries can wait, merchant. What is your offer for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_944E9D58CFCE6B — OffTopic

Speak of our trade, not of strange wonders. What is your price for the {spiceDescriptor}?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1018`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. OFF_TOPIC first count.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Speak of our trade, not of strange wonders. What is your price for the black pepper?
- Existing TTS input example: Speak of our trade, not of strange wonders. What is your price for the black pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Speak of our trade, not of strange wonders. What is your price for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_2EFD8EC9168C45 — OffTopic

This is a bazaar, not a hall of riddles. Return to the price of the {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1022`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. OFF_TOPIC second count.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: This is a bazaar, not a hall of riddles. Return to the price of the black pepper.
- Existing TTS input example: This is a bazaar, not a hall of riddles. Return to the price of the black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "This is a bazaar, not a hall of riddles. Return to the price of the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_0392AE223D9105 — OffTopic

Enough. Speak of the trade, or I leave.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1024`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. OFF_TOPIC third count.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_4F974863293736 — HostileWarning

Speak with respect in this bazaar, or I will take my business elsewhere.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1031`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE first count.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_0DEDB6EA889365 — HostileWarning

You try my patience. One more insult and this trade ends.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1035`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE second count.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_C022E0E574B8A6 — HostileWarning

Mind your tongue, merchant. This is your last warning.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1037`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE third count only if frustration remains <0.75; earlier walk-away gates otherwise apply.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 06 Clarification and precise continuation

#### AUD_ABD_C910E41DB9D2FD — UnclearSpeech

Speak plainly, merchant.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:88`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Null trade or input; result.requiresClarification=true.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_EC6365C608C06C — Unknown

That price is suspiciously low. I will not agree to it.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:228`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ACCEPT hasSellerPrice and lastSellerPrice <0.3*targetMarketValue; nonfinal refusal.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_C82A8D4A0BD5AD — UnclearSpeech

State your final price clearly, merchant.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:255`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ULTIMATUM without seller price; requiresClarification=true.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_D39608439B85DA — UnclearSpeech

I did not catch your price. Say it plainly.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:308`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. PRICE/QUANTITY_PRICE missing valid price; requiresClarification=true.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_B7B550A000BBB7 — UnclearSpeech

Speak plainly, merchant. My offer stands at {currentOffer} varahas for {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:459`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Brain default intent path.
- Restrictions: {}
- Placeholders: currentOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Speak plainly, merchant. My offer stands at 234 varahas for 2 Seers (~560g) of black pepper.
- Existing TTS input example: Speak plainly, merchant. My offer stands at 234 varahas for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Speak plainly, merchant. My offer stands at "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_9B26B06E9E028A — UnclearSpeech

Our current terms are {quantity} of {spiceDescriptor} for {currentOffer} varahas. Do you accept those terms as they stand, or want to change the price or quantity?

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:732`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. clarificationKind=UnresolvedTerms; first clarification priority.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor, currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: Our current terms are 2 Seers (~560g) of black pepper for 234 varahas. Do you accept those terms as they stand, or want to change the price or quantity?
- Existing TTS input example: Our current terms are 2 Seers of black pepper for 234 varahas. Do you accept those terms as they stand, or want to change the price or quantity?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Our current terms are "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas. Do you accept those terms as they stand, or want to change the price or quantity?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_A1AF5BDA228EDB — UnclearSpeech

Our bargain is already set. Bring the goods I asked for to complete the trade.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:735`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. parseReason=FulfillmentExpected, after UnresolvedTerms check.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_6FEF5636769CA4 — UnclearSpeech

The market is noisy. I heard nothing clearly. Say your price again.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:740`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. parseReason=EmptyTranscript.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_9ADFCE035F6F42 — UnclearSpeech

The market is busy and your words were unclear. Say it again plainly.

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:745`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. parseReason=UnrecognizedSpeech.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_0E1FE8AE090C01 — UnclearSpeech

You mentioned {input.referencedPrices[0]} and {input.referencedPrices[input.referencedPrices.Count - 1]}. Which is your final price?

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:750`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. MultipleActionablePrices and >=2 referenced prices.
- Restrictions: {}
- Placeholders: input.referencedPrices[0], input.referencedPrices[input.referencedPrices.Count - 1]; type: DYNAMIC_TEMPLATE
- Raw Unity example: You mentioned 250 and 300. Which is your final price?
- Existing TTS input example: You mentioned 250 and 300. Which is your final price?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You mentioned "}, {"kind": "VALUE", "text": "{input.referencedPrices[0]}"}, {"kind": "LITERAL", "text": " and "}, {"kind": "VALUE", "text": "{input.referencedPrices[input.referencedPrices.Count - 1]}"}, {"kind": "LITERAL", "text": ". Which is your final price?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_086B46D00420A9 — UnclearSpeech

You referred to my earlier offer of {input.referencedPrices[input.referencedPrices.Count - 1]} varahas. Do you want that price again, or are you naming a new one?

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:755`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. HistoricalPriceOnly and >=1 referenced price.
- Restrictions: {}
- Placeholders: input.referencedPrices[input.referencedPrices.Count - 1]; type: DYNAMIC_TEMPLATE
- Raw Unity example: You referred to my earlier offer of 300 varahas. Do you want that price again, or are you naming a new one?
- Existing TTS input example: You referred to my earlier offer of 300 varahas. Do you want that price again, or are you naming a new one?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You referred to my earlier offer of "}, {"kind": "VALUE", "text": "{input.referencedPrices[input.referencedPrices.Count - 1]}"}, {"kind": "LITERAL", "text": " varahas. Do you want that price again, or are you naming a new one?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_EC6B9FE215122E — UnclearSpeech

Tell me the quantity plainly, merchant. How much of the {spiceDescriptor} do you mean?

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:760`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. MissingQuantity or ExpectQuantity.
- Restrictions: {}
- Placeholders: spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Tell me the quantity plainly, merchant. How much of the black pepper do you mean?
- Existing TTS input example: Tell me the quantity plainly, merchant. How much of the black pepper do you mean?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Tell me the quantity plainly, merchant. How much of the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " do you mean?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_3C9CBA39600499 — UnclearSpeech

State your price clearly, merchant. What do you ask for the {quantity} of {spiceDescriptor}?

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:765`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. StateBlockedAccept/MissingPrice or ExpectOfferPrice.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: State your price clearly, merchant. What do you ask for the 2 Seers (~560g) of black pepper?
- Existing TTS input example: State your price clearly, merchant. What do you ask for the 2 Seers of black pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "State your price clearly, merchant. What do you ask for the "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_BC42B344E7EB9B — UnclearSpeech

Speak plainly. Do you accept my offer of {currentOffer} varahas, or do you want a different price?

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:770`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. AmbiguousAcceptOrCounter or ExpectAcceptOrCounter.
- Restrictions: {}
- Placeholders: currentOffer; type: DYNAMIC_TEMPLATE
- Raw Unity example: Speak plainly. Do you accept my offer of 234 varahas, or do you want a different price?
- Existing TTS input example: Speak plainly. Do you accept my offer of 234 varahas, or do you want a different price?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Speak plainly. Do you accept my offer of "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas, or do you want a different price?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_E8C5FF986005DD — UnclearSpeech

I will say it again. My offer is {currentOffer} varahas for {quantity} of {spiceDescriptor}.

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:775`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. CONFUSED/ConfusedPlayer only if earlier state branches did not match.
- Restrictions: {}
- Placeholders: currentOffer, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I will say it again. My offer is 234 varahas for 2 Seers (~560g) of black pepper.
- Existing TTS input example: I will say it again. My offer is 234 varahas for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I will say it again. My offer is "}, {"kind": "VALUE", "text": "{currentOffer}"}, {"kind": "LITERAL", "text": " varahas for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_CC4A465EFF3043 — UnclearSpeech

I did not understand you. State your price clearly for the {quantity} of {spiceDescriptor}.

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:779`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Default clarification branch.
- Restrictions: {}
- Placeholders: quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I did not understand you. State your price clearly for the 2 Seers (~560g) of black pepper.
- Existing TTS input example: I did not understand you. State your price clearly for the 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I did not understand you. State your price clearly for the "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 07 Waiting, completion and departure

#### AUD_ABD_09BDAE294199F1 — TransactionFailure

Very well. I will take my business elsewhere.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:122`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit DISMISS.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_5E988AD70877BD — TransactionFailure

I am done with such talk. We shall not trade today.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:172`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. OFF_TOPIC reaches outOfWorldCount>=4.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_5C8919BF40CB66 — TransactionFailure

Speak with respect in this bazaar, or we are finished.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:183`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE count>=4, or count>=3 with frustration>=0.75.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_4F1F1EB8D3A4AC — TransactionFailure

Very well. We shall not trade today.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:200`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit hard REJECT; early terminal return before counter computation.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_CB146E1EFA4F55 — TransactionFailure

Then we shall not trade today.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:267`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Ultimatum above max and ShouldWalkAwayFromHighPrice.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording. Additional source unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1086; Pressure walk-away Strict (same wording also ultimatum).

#### AUD_ABD_52C8B03A413EF6 — TransactionFailure

That price is too suspicious. I will take my leave.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:326`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit price per gram <35% target market value per gram, lowPriceCount>=3.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_CE5C34F9285932 — TransactionFailure

Your demand is too high. Then we shall not trade.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:340`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit price above upper walk-away range and pressure exhausted.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_E9DF1BA83C22BF — TransactionFailure

{buyerName} will not linger longer. We trade another day.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1082`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Pressure walk-away Impatient.
- Restrictions: {}
- Placeholders: buyerName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Abdul Rahman will not linger longer. We trade another day.
- Existing TTS input example: Abdul Rahman will not linger longer. We trade another day.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " will not linger longer. We trade another day."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_ABD_DB510B713E4082 — TransactionFailure

Very well. {buyerName} will return another day.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1088`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other pressure walk-away.
- Restrictions: {}
- Placeholders: buyerName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Very well. Abdul Rahman will return another day.
- Existing TTS input example: Very well. Abdul Rahman will return another day.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Very well. "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " will return another day."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

## GENERIC_BYPASSED

### 01 Arrival and repeat visit

#### AUD_ABD_78854D4925FC7F — CustomerGreeting

Greetings, merchant. I seek {quantityLabel} of {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:168`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings, merchant. I seek 2 Seers (~560g) of pepper.
- Existing TTS input example: Greetings, merchant. I seek 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings, merchant. I seek "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_C0EC64291C1C66 — CustomerGreeting

Good day. I have come for {quantityLabel} of {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:169`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I have come for 2 Seers (~560g) of pepper.
- Existing TTS input example: Good day. I have come for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I have come for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_300CDCF55B955B — RepeatCustomerGreeting

We have already met, merchant. Let us return to the bargain.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:173`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 02 Trade information and quantity

#### AUD_ABD_ACEC0A16D6A487 — AskWhatBuyerWants

I am here to buy {quantityLabel} of {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:177`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am here to buy 2 Seers (~560g) of pepper.
- Existing TTS input example: I am here to buy 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am here to buy "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_78512EA74DD6D7 — AskWhatBuyerWants

The {spiceName} is what brings me to your stall.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:178`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: The pepper is what brings me to your stall.
- Existing TTS input example: The pepper is what brings me to your stall.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " is what brings me to your stall."}]
- Note: 

#### AUD_ABD_94F9054AF9A4F9 — AskBuyerBudget

My offer stands at {currentBuyerOffer} {currency} for {quantityLabel}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:182`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer stands at 234 varahas for 2 Seers (~560g).
- Existing TTS input example: My offer stands at 234 varahas for 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer stands at "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_AA7605D7C3680C — AskBuyerBudget

I can pay {currentBuyerOffer} {currency} at present.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:183`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can pay 234 varahas at present.
- Existing TTS input example: I can pay 234 varahas at present.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can pay "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " at present."}]
- Note: 

#### AUD_ABD_0F28DBA3C9BBD7 — AskQuantity

State the quantity clearly before we settle the price.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:187`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_A73DF37BD9C7BA — AskQuantity

How much {spiceName} do you mean to sell?

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:188`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: How much pepper do you mean to sell?
- Existing TTS input example: How much pepper do you mean to sell?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "How much "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " do you mean to sell?"}]
- Note: 

### 03 Negotiation and rejection

#### AUD_ABD_8E2695605396FC — SellerPriceTooHigh

{offeredPrice} {currency} is far above what I can justify for {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:192`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: offeredPrice, currency, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 varahas is far above what I can justify for pepper.
- Existing TTS input example: 300 varahas is far above what I can justify for pepper.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is far above what I can justify for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_D89368D8E0904A — SellerPriceTooHigh

That price is too high for me. I cannot follow you there.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:193`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_6B7AEFE78580BF — SellerPriceSlightlyHigh

You ask a little too much, merchant. I can only move toward {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:197`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You ask a little too much, merchant. I can only move toward 234 varahas.
- Existing TTS input example: You ask a little too much, merchant. I can only move toward 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You ask a little too much, merchant. I can only move toward "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_1B999DABDFB4F9 — SellerPriceSlightlyHigh

That is above my reach. Let us come nearer to {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:198`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: That is above my reach. Let us come nearer to 234 varahas.
- Existing TTS input example: That is above my reach. Let us come nearer to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That is above my reach. Let us come nearer to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_F93B61D925CF68 — SellerPriceBelowExpected

That is below what I expected to hear from you.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:207`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_7D359BD244DA15 — SellerPriceBelowExpected

Your number is lower than I prepared for, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:208`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_A78C027363F371 — BuyerCounterFirst

My first counter is {counterPrice} {currency}. Consider it carefully.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:212`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My first counter is 234 varahas. Consider it carefully.
- Existing TTS input example: My first counter is 234 varahas. Consider it carefully.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My first counter is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Consider it carefully."}]
- Note: 

#### AUD_ABD_8EBC4D458790D6 — BuyerCounterFirst

I begin at {counterPrice} {currency} for this {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:213`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I begin at 234 varahas for this pepper.
- Existing TTS input example: I begin at 234 varahas for this pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I begin at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for this "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_B3758B56B413D6 — BuyerCounterMiddle

We are narrowing the gap. I can offer {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:217`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We are narrowing the gap. I can offer 234 varahas.
- Existing TTS input example: We are narrowing the gap. I can offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We are narrowing the gap. I can offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_1DFFA9674B0C51 — BuyerCounterMiddle

I will come to {counterPrice} {currency}, but not freely.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:218`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I will come to 234 varahas, but not freely.
- Existing TTS input example: I will come to 234 varahas, but not freely.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I will come to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", but not freely."}]
- Note: 

#### AUD_ABD_1A00947B3B4363 — BuyerCounterFinal

This is my last meaningful step: {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:222`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: This is my last meaningful step: 234 varahas.
- Existing TTS input example: This is my last meaningful step: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "This is my last meaningful step: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_79EBFEAE13AAE4 — BuyerCounterFinal

I am nearly at my edge. {counterPrice} {currency} is my final counter.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:223`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am nearly at my edge. 234 varahas is my final counter.
- Existing TTS input example: I am nearly at my edge. 234 varahas is my final counter.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am nearly at my edge. "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is my final counter."}]
- Note: 

#### AUD_ABD_E6527F4943CE97 — BuyerHoldsFirm

I hold firm at {currentBuyerOffer} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:227`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I hold firm at 234 varahas.
- Existing TTS input example: I hold firm at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I hold firm at "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_514B0B56DC45E6 — BuyerHoldsFirm

My offer does not move beyond {currentBuyerOffer} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:228`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer does not move beyond 234 varahas.
- Existing TTS input example: My offer does not move beyond 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer does not move beyond "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_F73B311DDAD3E1 — PlayerRejectedBuyerOffer

Then you refuse my offer.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:237`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_F4C2CCF5C792C3 — PlayerRejectedBuyerOffer

So be it. You reject the bargain I put forward.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:238`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 04 Price agreement, goods pending

#### AUD_ABD_82C8181D268295 — SellerPriceAccepted

That price is acceptable. We can conclude this bargain.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:202`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_59A08C36DCD1F4 — SellerPriceAccepted

Very well. That sits within reason for me.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:203`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_65C2E0560EE45D — PlayerAcceptedDeal

Then we are agreed at {finalPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:232`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: finalPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then we are agreed at 234 varahas.
- Existing TTS input example: Then we are agreed at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then we are agreed at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_6F4729A9091B8C — PlayerAcceptedDeal

Good. We will settle at {finalPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:233`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: finalPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good. We will settle at 234 varahas.
- Existing TTS input example: Good. We will settle at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good. We will settle at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

### 05 Social, history and conduct

#### AUD_ABD_BFDAE4525FA42D — HistoryQuestion

There is time for stories later. First, let us finish this trade.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:242`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_AB4A9850AC700A — HistoryQuestion

History can wait, merchant. Speak of {spiceName} and price.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:243`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: History can wait, merchant. Speak of pepper and price.
- Existing TTS input example: History can wait, merchant. Speak of pepper and price.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "History can wait, merchant. Speak of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " and price."}]
- Note: 

#### AUD_ABD_C45BA0C6200267 — SocialGreeting

A courteous word is welcome, but trade is why I am here.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:247`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_7A00979E5C7EB7 — SocialGreeting

Greetings to you as well. Now let us return to the bargain.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:248`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_86AD4D4CF98968 — OffTopic

Let us keep to the trade, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:252`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_E39FFA2C211C31 — OffTopic

Speak of price or quantity, not of other matters.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:253`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 06 Clarification and precise continuation

#### AUD_ABD_4C29AD3BB3861D — UnclearSpeech

I did not catch that clearly. {ruleReply}

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:257`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: I did not catch that clearly. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: I did not catch that clearly. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I did not catch that clearly. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: 

#### AUD_ABD_3B2FD666A89550 — UnclearSpeech

Your meaning is not clear to me. {ruleReply}

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:258`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: Your meaning is not clear to me. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: Your meaning is not clear to me. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Your meaning is not clear to me. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: 

#### AUD_ABD_647B136DC6E1EF — Unknown

{ruleReply}

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:276`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{ruleReply}"}]
- Note: 

### 07 Waiting, completion and departure

#### AUD_ABD_E63171B3825F12 — TransactionSuccess

The trade is complete and honourably settled.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:262`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_147AC0322ECB81 — TransactionFailure

Then this bargain ends without a sale.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:266`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_E27141A0C342A5 — TransactionFailure

We part without agreement today.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:267`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_B2D28F0AAB8BAF — TimePressure

My patience is spent. Decide now or we are done.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:271`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_733FBAB24A27A8 — TimePressure

Time runs thin, merchant. I cannot linger longer.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:272`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored abdul_rahman; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

## PRESENTATION_FALLBACK

### 01 Arrival and repeat visit

#### AUD_ABD_B9E96D60759F75 — CustomerGreeting

Greetings, merchant. I am {buyerName}, and I seek {quantityLabel} of {spice}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:335`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: buyerName, quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings, merchant. I am Abdul Rahman, and I seek 2 Seers (~560g) of pepper.
- Existing TTS input example: Greetings, merchant. I am Abdul Rahman, and I seek 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings, merchant. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ", and I seek "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_46FC4996E4C59B — CustomerGreeting

Good day. I am {buyerName}. I am here for {quantityLabel} of {spice}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:337`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: buyerName, quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I am Abdul Rahman. I am here for 2 Seers (~560g) of pepper.
- Existing TTS input example: Good day. I am Abdul Rahman. I am here for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ". I am here for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_ABD_C7B6958C60889B — CustomerGreeting

Merchant, I am {buyerName}. I need {quantityLabel} of {spice} quickly.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:339`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: buyerName, quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Merchant, I am Abdul Rahman. I need 2 Seers (~560g) of pepper quickly.
- Existing TTS input example: Merchant, I am Abdul Rahman. I need 2 Seers of pepper quickly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Merchant, I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ". I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": " quickly."}]
- Note: 

#### AUD_ABD_A383DA81A05618 — CustomerGreeting

Greetings, merchant. I seek {quantityLabel} of {spice}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:341`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings, merchant. I seek 2 Seers (~560g) of pepper.
- Existing TTS input example: Greetings, merchant. I seek 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings, merchant. I seek "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

### 07 Waiting, completion and departure

#### AUD_ABD_210C40327571AF — IdleReminderFirst

Well?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:92`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_95679342FFFF15 — IdleReminderFirst

Take your time, but don't keep me waiting.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:93`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_7164CB920051F5 — IdleReminderSecond

I don't have all day. What's your decision?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:97`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_609E0E5A59C47C — IdleReminderSecond

Other merchants are waiting for my business.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:98`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_4E5911AD4C2F76 — TransactionFailure

Enough. I am leaving.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:102`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_8D61E64EAFF350 — TransactionFailure

You have wasted enough of my time.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:103`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_CC12A24CB6D02A — TransactionFailure

My patience is spent. I will take my leave.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:1561`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. TryHandleNegotiationTimeout default finalLine; authored terminal GetIdleReminder replaces it.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

## PROVIDER_FALLBACK

### 01 Arrival and repeat visit

#### AUD_ABD_D81F2B3BD9343A — CustomerGreeting

Greetings, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / CONTEXT_ONLY
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:130`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. Greeting context.ruleReply only; greeting templates do not include ruleReply.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 06 Clarification and precise continuation

#### AUD_ABD_CC19E934B5F42C — UnclearSpeech

Speak plainly, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_FALLBACK
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:577`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Blank brain.replyText fallback for context.ruleReply and ReplacePlaceholders.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 07 Waiting, completion and departure

#### AUD_ABD_4AF37DE80A4004 — TransactionSuccess

Thank you, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_FALLBACK
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:73`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetPresentation fallback after settlement, if no authored match or exception.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

## UI_ONLY_RECOVERY

### 06 Clarification and precise continuation

#### AUD_ABD_DBAFFCF412DB88 — UnclearSpeech

The market is too noisy, could you repeat that?

- Classification: **UNREACHABLE_OR_DEFERRED** / TEXT_ONLY_NOT_SPOKEN
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:554`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. Null trade in local turn or error in legacy OnNPCReply; assigned npcText only, not sent to PresentNpcSubtitleAndTts.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_ABD_15A1EF806B11DD — UnclearSpeech

The market is too noisy, could you repeat that?

- Classification: **UNREACHABLE_OR_DEFERRED** / TEXT_ONLY_NOT_SPOKEN
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:894`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. Null trade in local turn or error in legacy OnNPCReply; assigned npcText only, not sent to PresentNpcSubtitleAndTts.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 
