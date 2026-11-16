# Saraswati Chetti — exact current source script

Verbatim Unity strings, not proposed rewrites. CORE means functional negotiation coverage, not a frequency statistic. Editor currently forces Father; classifications describe routes when this character is selected. Source/export IDs are ordinal aliases, not serialized runtime IDs. Shared AUD IDs are documentation-only. Examples use one illustrative state and do not form a guaranteed conversation.

## AUTHORED

### 01 Arrival and repeat visit

#### L1_SAR_001 — CustomerGreeting

Namaskaram, merchant. I am Saraswati Chetti. I need {quantityLabel} of {spiceName} for my shop.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:15`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Namaskaram, merchant. I am Saraswati Chetti. I need 2 Seers (~560g) of pepper for my shop.
- Existing TTS input example: Namaskaram, merchant. I am Saraswati Chetti. I need 2 Seers of pepper for my shop.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Namaskaram, merchant. I am Saraswati Chetti. I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " for my shop."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_002 — CustomerGreeting

Saraswati Chetti. I am looking for {quantityLabel} of {spiceName}; what is your price?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:16`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Saraswati Chetti. I am looking for 2 Seers (~560g) of pepper; what is your price?
- Existing TTS input example: Saraswati Chetti. I am looking for 2 Seers of pepper; what is your price?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Saraswati Chetti. I am looking for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "; what is your price?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_003 — CustomerGreeting

Good day. My customers ask for {spiceName}, and I need {quantityLabel}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:17`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. My customers ask for pepper, and I need 2 Seers (~560g).
- Existing TTS input example: Good day. My customers ask for pepper, and I need 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. My customers ask for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ", and I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_004 — CustomerGreeting

I have come for {quantityLabel} of {spiceName}. Let us see if the price suits the goods.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:18`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ChatManager.StartSession -> GetGreeting(isRepeatCustomer=false); initial ExpectOfferPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have come for 2 Seers (~560g) of pepper. Let us see if the price suits the goods.
- Existing TTS input example: I have come for 2 Seers of pepper. Let us see if the price suits the goods.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have come for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Let us see if the price suits the goods."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_005 — RepeatCustomerGreeting

We meet at your stall again. What have you brought today?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:20`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_006 — RepeatCustomerGreeting

I remember your stall, merchant. Let us discuss today's {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:21`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I remember your stall, merchant. Let us discuss today's pepper.
- Existing TTS input example: I remember your stall, merchant. Let us discuss today's pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I remember your stall, merchant. Let us discuss today's "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_007 — RepeatCustomerGreeting

Back again for {spiceName}. Let us agree on terms for {quantityLabel}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:22`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: Back again for pepper. Let us agree on terms for 2 Seers (~560g).
- Existing TTS input example: Back again for pepper. Let us agree on terms for 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Back again for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Let us agree on terms for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_008 — RepeatCustomerGreeting

Another visit, another set of figures. Shall we begin?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:23`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetGreeting(isRepeatCustomer=true) on an actual repeat customer visit; mid-visit GREETING uses SocialGreeting instead.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 02 Trade information and quantity

#### L1_SAR_009 — AskWhatBuyerWants

I need {quantityLabel} of {spiceName} for my shop.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:25`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I need 2 Seers (~560g) of pepper for my shop.
- Existing TTS input example: I need 2 Seers of pepper for my shop.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " for my shop."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_010 — AskWhatBuyerWants

It is {spiceName} I came to purchase, {quantityLabel} of it.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:26`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: It is pepper I came to purchase, 2 Seers (~560g) of it.
- Existing TTS input example: It is pepper I came to purchase, 2 Seers of it.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "It is "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " I came to purchase, "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of it."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_011 — AskWhatBuyerWants

My customers have been asking for {spiceName}. I seek {quantityLabel}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:27`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: My customers have been asking for pepper. I seek 2 Seers (~560g).
- Existing TTS input example: My customers have been asking for pepper. I seek 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My customers have been asking for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". I seek "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_012 — AskWhatBuyerWants

{quantityLabel} of {spiceName}, provided the quality holds up.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:28`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY; buyer price already introduced and asksCurrentOffer=false. Before price introduction use AskSellerPrice.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: 2 Seers (~560g) of pepper, provided the quality holds up.
- Existing TTS input example: 2 Seers of pepper, provided the quality holds up.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ", provided the quality holds up."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_013 — AskBuyerBudget

My offer stands at {currentBuyerOffer} {currency} for this lot.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:30`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer stands at 234 varahas for this lot.
- Existing TTS input example: My offer stands at 234 varahas for this lot.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer stands at "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for this lot."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_014 — AskBuyerBudget

I have weighed the quantity. {currentBuyerOffer} {currency} is my figure.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:31`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have weighed the quantity. 234 varahas is my figure.
- Existing TTS input example: I have weighed the quantity. 234 varahas is my figure.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have weighed the quantity. "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is my figure."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_015 — AskBuyerBudget

For the {quantityLabel} of {spiceName} in your measure, my figure is {currentBuyerOffer} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:32`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName, currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: For the 2 Seers (~560g) of pepper in your measure, my figure is 234 varahas.
- Existing TTS input example: For the 2 Seers of pepper in your measure, my figure is 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " in your measure, my figure is "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_016 — AskBuyerBudget

At present I can justify {currentBuyerOffer} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:33`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUERY_BUYER_BUDGET; PRICE_QUERY without a reason-only query; ITEM_QUERY asking current offer; CONTINUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: At present I can justify 234 varahas.
- Existing TTS input example: At present I can justify 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "At present I can justify "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_017 — AskQuantity

I require {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:35`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I require 2 Seers (~560g) of pepper.
- Existing TTS input example: I require 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I require "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_018 — AskQuantity

The measure I asked for is {quantityLabel}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:36`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: The measure I asked for is 2 Seers (~560g).
- Existing TTS input example: The measure I asked for is 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The measure I asked for is "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_019 — AskQuantity

Set aside {quantityLabel} of {spiceName}; let our figures be exact.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:37`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Set aside 2 Seers (~560g) of pepper; let our figures be exact.
- Existing TTS input example: Set aside 2 Seers of pepper; let our figures be exact.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Set aside "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "; let our figures be exact."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_020 — AskQuantity

{quantityLabel}, merchant. The quantity matters as much as the price.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:38`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_QUERY; answers committed trade quantity.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: 2 Seers (~560g), merchant. The quantity matters as much as the price.
- Existing TTS input example: 2 Seers , merchant. The quantity matters as much as the price.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": ", merchant. The quantity matters as much as the price."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_021 — AskSellerPrice

You know my quantity. What price do you ask for it?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:40`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_022 — AskSellerPrice

Let me hear your price for {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:41`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Let me hear your price for 2 Seers (~560g) of pepper.
- Existing TTS input example: Let me hear your price for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let me hear your price for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_023 — AskSellerPrice

The measure is clear; the price is yours to name.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:42`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_024 — AskSellerPrice

I have told you what I need. What is your asking price?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:43`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. ITEM_QUERY whose brain nextExpectedReplyState is ExpectOfferPrice (no current-offer request and no introduced price). Not all PRICE_QUERY inputs.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_025 — QuantityChanged

Then we are discussing {quantityLabel} of {spiceName}. Let us price that quantity.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:45`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then we are discussing 2 Seers (~560g) of pepper. Let us price that quantity.
- Existing TTS input example: Then we are discussing 2 Seers of pepper. Let us price that quantity.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then we are discussing "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Let us price that quantity."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_026 — QuantityChanged

I have the new measure: {quantityLabel}. What is your price for it?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:46`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have the new measure: 2 Seers (~560g). What is your price for it?
- Existing TTS input example: I have the new measure: 2 Seers . What is your price for it?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have the new measure: "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": ". What is your price for it?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_027 — QuantityChanged

Very well, {quantityLabel} of {spiceName}. That changes what we must weigh.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:47`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Very well, 2 Seers (~560g) of pepper. That changes what we must weigh.
- Existing TTS input example: Very well, 2 Seers of pepper. That changes what we must weigh.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Very well, "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". That changes what we must weigh."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_028 — QuantityChanged

The quantity is now {quantityLabel}; I will judge the price against that.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:48`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. QUANTITY_CHANGE with result.commitsQuantity=true; accepted terms cannot be changed. Quantity+price may instead counter/accept.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: The quantity is now 2 Seers (~560g); I will judge the price against that.
- Existing TTS input example: The quantity is now 2 Seers ; I will judge the price against that.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The quantity is now "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "; I will judge the price against that."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_070 — OfferReason

I must resell this lot in my shop. At {currentBuyerOffer} {currency}, the measure still makes sense.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:101`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I must resell this lot in my shop. At 234 varahas, the measure still makes sense.
- Existing TTS input example: I must resell this lot in my shop. At 234 varahas, the measure still makes sense.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I must resell this lot in my shop. At "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", the measure still makes sense."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_071 — OfferReason

I judge the quality and what my customers will pay. That gives me {currentBuyerOffer} {currency}.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:102`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I judge the quality and what my customers will pay. That gives me 234 varahas.
- Existing TTS input example: I judge the quality and what my customers will pay. That gives me 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I judge the quality and what my customers will pay. That gives me "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_072 — OfferReason

My offer of {currentBuyerOffer} {currency} accounts for the quantity and the work of selling it.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:103`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer of 234 varahas accounts for the quantity and the work of selling it.
- Existing TTS input example: My offer of 234 varahas accounts for the quantity and the work of selling it.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer of "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " accounts for the quantity and the work of selling it."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_073 — OfferReason

There is a second sale after this one. That is why I offer {currentBuyerOffer} {currency}.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:104`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. PRICE_QUERY+asksReason&&!asksItem OR QUERY_BUYER_BUDGET+asksReason; explicit pressure questions route here for this character (unlike Lakshmamma).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: There is a second sale after this one. That is why I offer 234 varahas.
- Existing TTS input example: There is a second sale after this one. That is why I offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "There is a second sale after this one. That is why I offer "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_074 — AskName

Saraswati Chetti. I keep a shop here and buy what my customers ask for.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:106`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_075 — AskName

My name is Saraswati Chetti. I prefer to know the person behind a price.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:107`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_076 — AskName

Saraswati Chetti, merchant. You may remember the name when we strike a bargain.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:108`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_077 — AskName

I am Saraswati Chetti. Now, tell me how you judge this lot.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:109`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; word name or exact normalized who are [you], after weather check.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_078 — AskOrigin

My shop is here in Vijayanagara. I know its buyers better than its distant roads.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:111`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_079 — AskOrigin

I do business in Vijayanagara's markets; that is the part of my story I can tell you.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:112`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_080 — AskOrigin

Vijayanagara is where I keep my shop, merchant.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:113`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_081 — AskOrigin

I sell here in Vijayanagara. My customers teach me plenty about this market.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:114`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; where/origin/from, after weather/name/king/temple checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 03 Negotiation and rejection

#### L1_SAR_029 — SellerPriceTooHigh

{offeredPrice} {currency} is beyond what I can justify for this measure. My offer is {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:50`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency, counterPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 varahas is beyond what I can justify for this measure. My offer is 234 varahas.
- Existing TTS input example: 300 varahas is beyond what I can justify for this measure. My offer is 234 varahas.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is beyond what I can justify for this measure. My offer is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_030 — SellerPriceTooHigh

That price asks my customers to bear too much. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:51`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: That price asks my customers to bear too much. I remain at 234 varahas.
- Existing TTS input example: That price asks my customers to bear too much. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That price asks my customers to bear too much. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_031 — SellerPriceTooHigh

Tell me what warrants {offeredPrice} {currency}; I cannot pay it as it stands.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:52`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Tell me what warrants 300 varahas; I cannot pay it as it stands.
- Existing TTS input example: Tell me what warrants 300 varahas; I cannot pay it as it stands.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Tell me what warrants "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; I cannot pay it as it stands."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_032 — SellerPriceTooHigh

I have counted the measure twice. {offeredPrice} {currency} is still too high.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:53`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have counted the measure twice. 300 varahas is still too high.
- Existing TTS input example: I have counted the measure twice. 300 varahas is still too high.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have counted the measure twice. "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is still too high."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_033 — SellerPriceSlightlyHigh

{offeredPrice} {currency} needs a stronger case. My figure is {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:55`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency, counterPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 varahas needs a stronger case. My figure is 234 varahas.
- Existing TTS input example: 300 varahas needs a stronger case. My figure is 234 varahas.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " needs a stronger case. My figure is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_034 — SellerPriceSlightlyHigh

I can follow your reasoning only as far as {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:56`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can follow your reasoning only as far as 234 varahas.
- Existing TTS input example: I can follow your reasoning only as far as 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can follow your reasoning only as far as "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_035 — SellerPriceSlightlyHigh

The asking price is above what I can justify. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:57`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The asking price is above what I can justify. I remain at 234 varahas.
- Existing TTS input example: The asking price is above what I can justify. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The asking price is above what I can justify. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_036 — SellerPriceSlightlyHigh

The quality may be sound, but {offeredPrice} {currency} is more than I will pay.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:58`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No upward movement; above max by <=max(6,minIncrement*3), or above previous offer within max(5,minIncrement*2).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The quality may be sound, but 300 varahas is more than I will pay.
- Existing TTS input example: The quality may be sound, but 300 varahas is more than I will pay.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The quality may be sound, but "}, {"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is more than I will pay."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_037 — SellerPriceBelowExpected

That is lower than I expected. Are you certain of such a figure?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:60`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_038 — SellerPriceBelowExpected

A low price can be welcome, but a calculating buyer always wonders why.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:61`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_039 — SellerPriceBelowExpected

You ask less than I expected. Is there a reason for so modest a sum?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:62`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_040 — SellerPriceBelowExpected

An unusual figure. I do not simply take a cheap offer without understanding the reason.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:63`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal nonaccepted explicit seller price below previous buyer offer, with no upward buyer movement. Acceptance/failure take priority.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_041 — BuyerCounterFirst

For this {quantityLabel}, I can offer {counterPrice} {currency}. What do you think?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:65`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: For this 2 Seers (~560g), I can offer 234 varahas. What do you think?
- Existing TTS input example: For this 2 Seers , I can offer 234 varahas. What do you think?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For this "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": ", I can offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What do you think?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_042 — BuyerCounterFirst

Let me put {counterPrice} {currency} before you for the lot.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:66`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Let me put 234 varahas before you for the lot.
- Existing TTS input example: Let me put 234 varahas before you for the lot.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let me put "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " before you for the lot."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_043 — BuyerCounterFirst

I have considered the measure. My offer is {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:67`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have considered the measure. My offer is 234 varahas.
- Existing TTS input example: I have considered the measure. My offer is 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have considered the measure. My offer is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_044 — BuyerCounterFirst

We can begin at {counterPrice} {currency}; tell me where your figure differs.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:68`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound<=1. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We can begin at 234 varahas; tell me where your figure differs.
- Existing TTS input example: We can begin at 234 varahas; tell me where your figure differs.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We can begin at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; tell me where your figure differs."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_045 — BuyerCounterMiddle

Your case has merit. I can increase my offer to {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:70`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Your case has merit. I can increase my offer to 234 varahas.
- Existing TTS input example: Your case has merit. I can increase my offer to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Your case has merit. I can increase my offer to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_046 — BuyerCounterMiddle

I have revised the figure to {counterPrice} {currency}. Will that answer?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:71`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have revised the figure to 234 varahas. Will that answer?
- Existing TTS input example: I have revised the figure to 234 varahas. Will that answer?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have revised the figure to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Will that answer?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_047 — BuyerCounterMiddle

The gap is narrower now: I offer {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:72`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The gap is narrower now: I offer 234 varahas.
- Existing TTS input example: The gap is narrower now: I offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The gap is narrower now: I offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_048 — BuyerCounterMiddle

I can move to {counterPrice} {currency} after hearing your argument.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:73`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can move to 234 varahas after hearing your argument.
- Existing TTS input example: I can move to 234 varahas after hearing your argument.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can move to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " after hearing your argument."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_049 — BuyerCounterFinal

I have reconsidered the figures. My offer is now {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:75`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have reconsidered the figures. My offer is now 234 varahas.
- Existing TTS input example: I have reconsidered the figures. My offer is now 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have reconsidered the figures. My offer is now "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_050 — BuyerCounterFinal

For the measure we have discussed, I can move to {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:76`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: For the measure we have discussed, I can move to 234 varahas.
- Existing TTS input example: For the measure we have discussed, I can move to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "For the measure we have discussed, I can move to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_051 — BuyerCounterFinal

You have drawn another step from me: {counterPrice} {currency}. What is your answer?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:77`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You have drawn another step from me: 234 varahas. What is your answer?
- Existing TTS input example: You have drawn another step from me: 234 varahas. What is your answer?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You have drawn another step from me: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What is your answer?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_052 — BuyerCounterFinal

After weighing your point, I offer {counterPrice} {currency} now.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:78`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound>=5. This is a round band, not an irrevocable last-offer guarantee.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: After weighing your point, I offer 234 varahas now.
- Existing TTS input example: After weighing your point, I offer 234 varahas now.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "After weighing your point, I offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " now."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_053 — BuyerHoldsFirm

My figure remains {counterPrice} {currency}; the measure has not changed.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:80`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My figure remains 234 varahas; the measure has not changed.
- Existing TTS input example: My figure remains 234 varahas; the measure has not changed.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My figure remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; the measure has not changed."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_054 — BuyerHoldsFirm

I have heard you, but my offer is still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:81`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have heard you, but my offer is still 234 varahas.
- Existing TTS input example: I have heard you, but my offer is still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have heard you, but my offer is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_055 — BuyerHoldsFirm

Give me a reason to revise it. Until then, {counterPrice} {currency} stands.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:82`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Give me a reason to revise it. Until then, 234 varahas stands.
- Existing TTS input example: Give me a reason to revise it. Until then, 234 varahas stands.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Give me a reason to revise it. Until then, "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " stands."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_056 — BuyerHoldsFirm

The argument has moved; the price has not. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:83`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The argument has moved; the price has not. I remain at 234 varahas.
- Existing TTS input example: The argument has moved; the price has not. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The argument has moved; the price has not. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_057 — BuyerHoldsFirm

I am keeping my offer at {counterPrice} {currency}. What do you propose?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:84`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am keeping my offer at 234 varahas. What do you propose?
- Existing TTS input example: I am keeping my offer at 234 varahas. What do you propose?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am keeping my offer at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What do you propose?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_066 — PlayerRejectedBuyerOffer

My figure does not persuade you. What price would?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:96`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_067 — PlayerRejectedBuyerOffer

You refuse that offer. Give me a figure to consider.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:97`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_068 — PlayerRejectedBuyerOffer

Then tell me where your calculation differs from mine.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:98`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_069 — PlayerRejectedBuyerOffer

We have no agreement yet. State the price you want.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:99`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. REJECT, nonterminal, updatedOffer<=previousOffer. A raising rejection uses the relevant counter band.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_128 — BuyerHoldsFirm

I enjoy a careful bargain. My figure is still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:175`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I enjoy a careful bargain. My figure is still 234 varahas.
- Existing TTS input example: I enjoy a careful bargain. My figure is still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I enjoy a careful bargain. My figure is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_129 — BuyerHoldsFirm

You make a thoughtful case, but I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:176`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You make a thoughtful case, but I remain at 234 varahas.
- Existing TTS input example: You make a thoughtful case, but I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You make a thoughtful case, but I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_130 — BuyerHoldsFirm

We can discuss the figures further. My offer stays at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:177`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We can discuss the figures further. My offer stays at 234 varahas.
- Existing TTS input example: We can discuss the figures further. My offer stays at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We can discuss the figures further. My offer stays at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_131 — BuyerHoldsFirm

I am listening, though the number remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:178`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am listening, though the number remains 234 varahas.
- Existing TTS input example: I am listening, though the number remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am listening, though the number remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_132 — BuyerHoldsFirm

You may yet persuade me; for now I hold at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:179`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Friendly", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You may yet persuade me; for now I hold at 234 varahas.
- Existing TTS input example: You may yet persuade me; for now I hold at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You may yet persuade me; for now I hold at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_133 — BuyerHoldsFirm

My offer remains {counterPrice} {currency}. What is your counter?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:181`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer remains 234 varahas. What is your counter?
- Existing TTS input example: My offer remains 234 varahas. What is your counter?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What is your counter?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_134 — BuyerHoldsFirm

The measure still supports {counterPrice} {currency} in my reckoning.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:182`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: The measure still supports 234 varahas in my reckoning.
- Existing TTS input example: The measure still supports 234 varahas in my reckoning.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The measure still supports "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " in my reckoning."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_135 — BuyerHoldsFirm

I have checked it again: still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:183`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have checked it again: still 234 varahas.
- Existing TTS input example: I have checked it again: still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have checked it again: still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_136 — BuyerHoldsFirm

I have considered it. The offer is still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:184`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have considered it. The offer is still 234 varahas.
- Existing TTS input example: I have considered it. The offer is still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have considered it. The offer is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_137 — BuyerHoldsFirm

That argument does not alter my offer of {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:185`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Normal", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: That argument does not alter my offer of 234 varahas.
- Existing TTS input example: That argument does not alter my offer of 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That argument does not alter my offer of "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_138 — BuyerHoldsFirm

I have checked the measure. My offer stays at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:187`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have checked the measure. My offer stays at 234 varahas.
- Existing TTS input example: I have checked the measure. My offer stays at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have checked the measure. My offer stays at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_139 — BuyerHoldsFirm

Repeating a price does not explain it. I remain at {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:188`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Repeating a price does not explain it. I remain at 234 varahas.
- Existing TTS input example: Repeating a price does not explain it. I remain at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Repeating a price does not explain it. I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_140 — BuyerHoldsFirm

I require a reason to move. Until then, {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:189`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I require a reason to move. Until then, 234 varahas.
- Existing TTS input example: I require a reason to move. Until then, 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I require a reason to move. Until then, "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_141 — BuyerHoldsFirm

No change in my offer: {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:190`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: No change in my offer: 234 varahas.
- Existing TTS input example: No change in my offer: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "No change in my offer: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_142 — BuyerHoldsFirm

I have heard the demand. The figure remains {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:191`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Strict", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have heard the demand. The figure remains 234 varahas.
- Existing TTS input example: I have heard the demand. The figure remains 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have heard the demand. The figure remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_143 — BuyerHoldsFirm

Still {counterPrice} {currency}. Your answer, please.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:193`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Still 234 varahas. Your answer, please.
- Existing TTS input example: Still 234 varahas. Your answer, please.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Your answer, please."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_144 — BuyerHoldsFirm

My offer is unchanged at {counterPrice} {currency}. Be direct.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:194`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer is unchanged at 234 varahas. Be direct.
- Existing TTS input example: My offer is unchanged at 234 varahas. Be direct.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer is unchanged at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Be direct."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_145 — BuyerHoldsFirm

{counterPrice} {currency} remains the figure. We need a decision.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:195`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: 234 varahas remains the figure. We need a decision.
- Existing TTS input example: 234 varahas remains the figure. We need a decision.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " remains the figure. We need a decision."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_146 — BuyerHoldsFirm

I have no revised offer. It is {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:196`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have no revised offer. It is 234 varahas.
- Existing TTS input example: I have no revised offer. It is 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have no revised offer. It is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_147 — BuyerHoldsFirm

I remain at {counterPrice} {currency}. What do you say?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:197`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Impatient", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I remain at 234 varahas. What do you say?
- Existing TTS input example: I remain at 234 varahas. What do you say?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I remain at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". What do you say?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_148 — BuyerHoldsFirm

My patience is short, but my offer is unchanged: {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:199`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My patience is short, but my offer is unchanged: 234 varahas.
- Existing TTS input example: My patience is short, but my offer is unchanged: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My patience is short, but my offer is unchanged: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_149 — BuyerHoldsFirm

Still {counterPrice} {currency}. I need a clear answer soon.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:200`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Still 234 varahas. I need a clear answer soon.
- Existing TTS input example: Still 234 varahas. I need a clear answer soon.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". I need a clear answer soon."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_150 — BuyerHoldsFirm

My figure remains {counterPrice} {currency}; I cannot wait much longer.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:201`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My figure remains 234 varahas; I cannot wait much longer.
- Existing TTS input example: My figure remains 234 varahas; I cannot wait much longer.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My figure remains "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; I cannot wait much longer."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_151 — BuyerHoldsFirm

We know the quantity. The offer is still {counterPrice} {currency}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:202`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We know the quantity. The offer is still 234 varahas.
- Existing TTS input example: We know the quantity. The offer is still 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We know the quantity. The offer is still "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_152 — BuyerHoldsFirm

I am holding at {counterPrice} {currency}. Please decide.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:203`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. No buyer raise on PRICE/COUNTER/BARGAIN/QUANTITY_PRICE/ULTIMATUM after more specific price cases. CONTINUE maps to AskBuyerBudget.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am holding at 234 varahas. Please decide.
- Existing TTS input example: I am holding at 234 varahas. Please decide.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am holding at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please decide."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_153 — SellerPriceTooHigh

{offeredPrice} {currency} is too high. My offer stays at {counterPrice} {currency}; answer soon.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:205`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: offeredPrice, currency, counterPrice; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 varahas is too high. My offer stays at 234 varahas; answer soon.
- Existing TTS input example: 300 varahas is too high. My offer stays at 234 varahas; answer soon.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is too high. My offer stays at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; answer soon."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_154 — SellerPriceTooHigh

I cannot justify that figure, and I have little time left.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:206`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_155 — SellerPriceTooHigh

Your price remains too high. I need a different answer now.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:207`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal nonacceptance with hasSellerPrice, no upward movement, price above maxBuyerPrice by more than max(6,minIncrement*3).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_156 — BuyerCounterMiddle

I can increase to {counterPrice} {currency}. Please answer promptly.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:209`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can increase to 234 varahas. Please answer promptly.
- Existing TTS input example: I can increase to 234 varahas. Please answer promptly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can increase to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please answer promptly."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_157 — BuyerCounterMiddle

My revised offer is {counterPrice} {currency}; time is short.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:210`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My revised offer is 234 varahas; time is short.
- Existing TTS input example: My revised offer is 234 varahas; time is short.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My revised offer is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "; time is short."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_158 — BuyerCounterMiddle

I have moved to {counterPrice} {currency}. Let us decide.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:211`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Nonterminal upward buyer offer; CurrentRound 2-4. REJECT with a raise can route here.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have moved to 234 varahas. Let us decide.
- Existing TTS input example: I have moved to 234 varahas. Let us decide.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have moved to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Let us decide."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 04 Price agreement, goods pending

#### L1_SAR_058 — SellerPriceAccepted

Avunu, the price is agreed: {finalPrice} {currency} for {quantityLabel} of {spiceName}. Please bring the goods.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:86`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Avunu, the price is agreed: 234 varahas for 2 Seers (~560g) of pepper. Please bring the goods.
- Existing TTS input example: Avunu, the price is agreed: 234 varahas for 2 Seers of pepper. Please bring the goods.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Avunu, the price is agreed: "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Please bring the goods."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_059 — SellerPriceAccepted

{finalPrice} {currency} suits the {quantityLabel} of {spiceName}. Bring the goods, please.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:87`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: 234 varahas suits the 2 Seers (~560g) of pepper. Bring the goods, please.
- Existing TTS input example: 234 varahas suits the 2 Seers of pepper. Bring the goods, please.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " suits the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Bring the goods, please."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_060 — SellerPriceAccepted

We have terms: {quantityLabel} of {spiceName} at {finalPrice} {currency}. Please prepare the delivery.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:88`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: quantityLabel, spiceName, finalPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We have terms: 2 Seers (~560g) of pepper at 234 varahas. Please prepare the delivery.
- Existing TTS input example: We have terms: 2 Seers of pepper at 234 varahas. Please prepare the delivery.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We have terms: "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please prepare the delivery."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_061 — SellerPriceAccepted

I accept {finalPrice} {currency} for {quantityLabel} of {spiceName}. The goods are still to come.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:89`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.isAccepted or resolutionAction=ACCEPT, input.intent!=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I accept 234 varahas for 2 Seers (~560g) of pepper. The goods are still to come.
- Existing TTS input example: I accept 234 varahas for 2 Seers of pepper. The goods are still to come.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I accept "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". The goods are still to come."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_062 — PlayerAcceptedDeal

Then we agree on {finalPrice} {currency} for {quantityLabel} of {spiceName}. Bring it over.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:91`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then we agree on 234 varahas for 2 Seers (~560g) of pepper. Bring it over.
- Existing TTS input example: Then we agree on 234 varahas for 2 Seers of pepper. Bring it over.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then we agree on "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Bring it over."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_063 — PlayerAcceptedDeal

The price is settled at {finalPrice} {currency}. Please bring over the {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:92`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: The price is settled at 234 varahas. Please bring over the 2 Seers (~560g) of pepper.
- Existing TTS input example: The price is settled at 234 varahas. Please bring over the 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The price is settled at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please bring over the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_064 — PlayerAcceptedDeal

I accept these terms: {finalPrice} {currency}, {quantityLabel} of {spiceName}. Please measure it out.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:93`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I accept these terms: 234 varahas, 2 Seers (~560g) of pepper. Please measure it out.
- Existing TTS input example: I accept these terms: 234 varahas, 2 Seers of pepper. Please measure it out.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I accept these terms: "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": ". Please measure it out."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_065 — PlayerAcceptedDeal

Our figures agree at {finalPrice} {currency}. Please bring the {quantityLabel} of {spiceName}.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:94`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Accepted result with input.intent=ACCEPT. Frozen terms; fulfillment still pending.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: finalPrice, currency, quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Our figures agree at 234 varahas. Please bring the 2 Seers (~560g) of pepper.
- Existing TTS input example: Our figures agree at 234 varahas. Please bring the 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Our figures agree at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Please bring the "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 05 Social, history and conduct

#### L1_SAR_082 — WeatherQuestion

Dry storage matters more to my spice than a bright sky.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:116`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_083 — WeatherQuestion

Rain changes a market day quickly. The measure and price still need attention.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:117`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_084 — WeatherQuestion

I watch the weather, but I watch the condition of my stock more closely.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:118`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; weather/rain/sun/sunshine. First keyword check; no weather simulation read.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_085 — HistoryKing

Krishnadevaraya is the king. I know my customers better than court affairs.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:120`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_086 — HistoryKing

Our ruler is Krishnadevaraya. What reaches my shop is news from the bazaar.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:121`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_087 — HistoryKing

Krishnadevaraya rules here. My daily calculations are rather smaller than his.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:122`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; king/krishnadevaraya after weather/name.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_088 — HistoryTemple

Virupaksha's temple stands by a busy bazaar. Follow the traders and you will find your way.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:124`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_089 — HistoryTemple

You mean Virupaksha's temple. Its market brings many buyers past these stalls.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:125`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_090 — HistoryTemple

The bazaar before Virupaksha's temple is hard to miss when business is lively.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:126`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; temple/virupaksha after weather/name/king.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_091 — HistoryQuestion

A city is easier to understand through what its people buy and sell.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:128`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_092 — HistoryQuestion

Vijayanagara draws many kinds of trade. My own knowledge begins at the shop counter.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:129`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_093 — HistoryQuestion

I can tell you what sells in this bazaar. For older histories, ask someone who keeps them.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:130`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_094 — HistoryQuestion

Look at the market and you will see how far goods travel to reach this city.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:131`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. SOCIAL/GENERAL_DIALOGUE; history/city/hampi/vijayanagara/market/bazaar, after the earlier keyword checks.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_095 — SocialGreeting

Namaskaram. A clear answer is as welcome as a courteous one.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:133`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_096 — SocialGreeting

Good to speak with you. Tell me what you think of this lot.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:134`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_097 — SocialGreeting

Courtesy makes bargaining easier to hear, even when the prices differ.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:135`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_098 — SocialGreeting

You have my attention, merchant. I am listening.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:136`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_099 — OffTopic

That is beyond what I know. Shall we return to the {spiceName}?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:138`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: That is beyond what I know. Shall we return to the pepper?
- Existing TTS input example: That is beyond what I know. Shall we return to the pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That is beyond what I know. Shall we return to the "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_100 — OffTopic

An interesting turn, though it does not answer the price of this lot.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:139`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_101 — OffTopic

I can speak to the measure before us. What did you want to know about it?

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:140`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_102 — OffTopic

We may discuss that another time. For now, I am buying {spiceName}.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:141`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal OFF_TOPIC; repeated out-of-world talk can instead resolve to TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: We may discuss that another time. For now, I am buying pepper.
- Existing TTS input example: We may discuss that another time. For now, I am buying pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We may discuss that another time. For now, I am buying "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_103 — HostileWarning

I will consider a price, not an insult. Speak plainly and respectfully.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:143`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_104 — HostileWarning

There is no need for that tone. State your objection to the offer.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:144`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_105 — HostileWarning

A careful measure deserves a careful word. Try again without the insult.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:145`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Nonterminal HOSTILE; terminal hostility instead resolves TransactionFailure.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_159 — SocialGreeting

A courteous word is welcome. I do need to return to my shop soon.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:213`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_160 — SocialGreeting

Thank you. Now please give me your answer on the trade.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:214`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_161 — SocialGreeting

We can speak more another day. I need a decision now.

- Classification: **REACHABLE_OPTIONAL** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:215`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. Mid-visit GREETING or remaining SOCIAL/GENERAL_DIALOGUE.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 06 Clarification and precise continuation

#### L1_SAR_106 — UnclearSpeech

Konchem more detail, please: {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:147`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: Konchem more detail, please: The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: Konchem more detail, please: The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Konchem more detail, please: "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_107 — UnclearSpeech

One point is uncertain. {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:148`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: One point is uncertain. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: One point is uncertain. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "One point is uncertain. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_108 — UnclearSpeech

Let us be precise. {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:149`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: Let us be precise. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: Let us be precise. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Let us be precise. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_109 — UnclearSpeech

I need to hear the exact terms. {ruleReply}

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:150`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. result.requiresClarification first priority, or CLARIFICATION/UNKNOWN intent; preserves precise {ruleReply}.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: I need to hear the exact terms. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: I need to hear the exact terms. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I need to hear the exact terms. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_110 — Unknown

{ruleReply}

- Classification: **REACHABLE_OPTIONAL** / CONDITIONAL_RESIDUAL
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:151`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. MapScenario residual outcome; e.g. nonfinal ACCEPT suspicious-price guard without a price on the input. Not equivalent to input UNKNOWN (which maps UnclearSpeech).
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{ruleReply}"}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

### 07 Waiting, completion and departure

#### L1_SAR_111 — TransactionSuccess

The goods are received and the measure is right. Thank you, merchant.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:153`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_112 — TransactionSuccess

The delivery matches our terms. A careful trade; you have my thanks.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:154`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_113 — TransactionSuccess

Well measured and delivered. I am pleased with this purchase.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:155`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_114 — TransactionSuccess

I have the {spiceName} now. You kept your word, and I appreciate it.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:156`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. GetTransactionSuccess after accepted physical delivery AND trade.SettlementClaimed; never merely at acceptance.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have the pepper now. You kept your word, and I appreciate it.
- Existing TTS input example: I have the pepper now. You kept your word, and I appreciate it.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have the "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " now. You kept your word, and I appreciate it."}]
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_115 — TransactionFailure

We have not found terms. I will try another stall.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:158`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_116 — TransactionFailure

The figures do not meet. I am leaving without a purchase.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:159`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_117 — TransactionFailure

No bargain today. I will return to my shop.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:160`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_118 — TransactionFailure

I cannot take this lot on those terms. Good day, merchant.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:161`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_119 — TimePressure

I must return to my shop soon. Give me a clear answer.

- Classification: **UNREACHABLE_OR_DEFERRED** / UNROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:163`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_120 — TimePressure

We have discussed the measure. Can we decide on the price?

- Classification: **UNREACHABLE_OR_DEFERRED** / UNROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:164`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_121 — TimePressure

My customers will not wait all day; let us settle the question.

- Classification: **UNREACHABLE_OR_DEFERRED** / UNROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:165`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. No current MapScenario/MapAuthoredScenario/GetIdleReminder route. Authored but not selected by normal Level 1 path.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_122 — IdleReminderFirst

Merchant? I am waiting for your figure.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:167`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_123 — IdleReminderFirst

Take a moment, then tell me what you think of my offer.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:168`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_124 — IdleReminderFirst

Have you considered the measure? I am ready for your answer.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:169`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle first stage -> GetIdleReminder(stage=1,terminal=false). Only actual waiting time counts.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_125 — IdleReminderSecond

I need an answer now; my shop is waiting.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:171`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_126 — IdleReminderSecond

We cannot bargain by silence. Do you have a price?

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:172`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_127 — IdleReminderSecond

Please give me your decision. I have other buying to do.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:173`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. Marketplace idle second stage -> GetIdleReminder(stage=2,terminal=false); reminder completion preserves elapsed idle time.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Any", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_162 — TransactionFailure

My time is spent. I will seek the goods elsewhere.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:217`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_163 — TransactionFailure

We have no bargain, and I must return to my shop.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:218`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

#### L1_SAR_164 — TransactionFailure

I cannot wait longer on these terms. I am leaving.

- Classification: **CORE** / ROUTED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/SiddharthChettiDialogue.cs:219`
- Trigger/evidence: Normal local negotiation/lifecycle presentation route. CORE describes functional coverage, not measured frequency. walkedAway/WALK_AWAY (dismissal, rejection limits, pressure, hostility, round limit), or terminal idle timeout.
- Restrictions: {"mood_bucket": "Any", "patience_bucket": "Low", "reputation_bucket": null, "desperation_bucket": null, "round_bucket": null}
- Placeholders: none; type: FULL_CLIP
- Note: Existing export ordinal; no runtime line ID is serialized in this C# table. Match exact character/template/scenario/restrictions.

## BRAIN_FALLBACK

### 02 Trade information and quantity

#### AUD_SAR_C63A5FE7E70477 — AskBuyerBudget

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

#### AUD_SAR_9AC21C5FCF67DB — AskSellerPrice

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

#### AUD_SAR_08A6999373A1A1 — AskBuyerBudget

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

#### AUD_SAR_A56530B7C3D97A — AskBuyerBudget

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

#### AUD_SAR_189D44784A9EF1 — QuantityChanged

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

#### AUD_SAR_DDFBAD2470E8E5 — QuantityChanged

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

#### AUD_SAR_9CD31497DD9020 — AskBuyerBudget

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

#### AUD_SAR_C4CDD001485665 — OfferReason

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

#### AUD_SAR_17CD21551CCF79 — AskBuyerBudget

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

#### AUD_SAR_D3484AE0E45CE7 — AskWhatBuyerWants

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

#### AUD_SAR_709F2F473439DE — AskWhatBuyerWants

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

#### AUD_SAR_C83BA8FA61D43B — AskQuantity

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

#### AUD_SAR_83015BA9D16215 — AskQuantity

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

#### AUD_SAR_5D32B694688E69 — AskOrigin

I have travelled far from {buyerOrigin} to this bazaar. Now, what is your price for the {spiceDescriptor}?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1000`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Origin helper Friendly.
- Restrictions: {}
- Placeholders: buyerOrigin, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I have travelled far from Local Vijayanagara Retail Shop Owner to this bazaar. Now, what is your price for the black pepper?
- Existing TTS input example: I have travelled far from Local Vijayanagara Retail Shop Owner to this bazaar. Now, what is your price for the black pepper?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I have travelled far from "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": " to this bazaar. Now, what is your price for the "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "?"}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_2827BFF6D67948 — AskOrigin

I come from {buyerOrigin}, but the road is less important than the trade.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1001`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Origin helper not Friendly.
- Restrictions: {}
- Placeholders: buyerOrigin; type: DYNAMIC_TEMPLATE
- Raw Unity example: I come from Local Vijayanagara Retail Shop Owner, but the road is less important than the trade.
- Existing TTS input example: I come from Local Vijayanagara Retail Shop Owner, but the road is less important than the trade.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I come from "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ", but the road is less important than the trade."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_F06756EC52D27F — AskName

I am {buyerName}. Let us return to the matter of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1005`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Name helper.
- Restrictions: {}
- Placeholders: buyerName, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am Saraswati Chetti. Let us return to the matter of black pepper.
- Existing TTS input example: I am Saraswati Chetti. Let us return to the matter of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ". Let us return to the matter of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 03 Negotiation and rejection

#### AUD_SAR_075B5AF44A29A0 — COUNTER_OR_HOLD

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

#### AUD_SAR_A3DBE0A9B44DBC — COUNTER_OR_HOLD

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

#### AUD_SAR_86287139B6D934 — PlayerRejectedBuyerOffer

Then we are too far apart, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:213`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. REJECT does not raise offer; hard -> too far apart, soft -> name a better price.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_08EDE5E41CA7DE — PlayerRejectedBuyerOffer

If that price does not suit you, name a better one.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:213`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. REJECT does not raise offer; hard -> too far apart, soft -> name a better price.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_74E8E5C14FFFFB — COUNTER_OR_HOLD

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

#### AUD_SAR_0D993A3B0F2316 — BuyerHoldsFirm

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

#### AUD_SAR_5444A33783C835 — COUNTER_OR_HOLD

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

#### AUD_SAR_32CD55BB9046A0 — COUNTER_OR_HOLD

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

#### AUD_SAR_A90EC2D785816A — SellerPriceBelowExpected

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

#### AUD_SAR_70D7BBF02A0D7D — SellerPriceBelowExpected

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

#### AUD_SAR_64CE47AE1AD26B — COUNTER_OR_HOLD

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

#### AUD_SAR_2CFB9D3772F850 — COUNTER_OR_HOLD

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

#### AUD_SAR_D46EB55E93C1CE — BuyerHoldsFirm

I can improve a little, but not much.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:433`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. BARGAIN fails to raise offer.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_A46A5680D97648 — COUNTER_OR_HOLD

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

#### AUD_SAR_19F4DD41830DE2 — COUNTER_OR_HOLD

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

#### AUD_SAR_71A0A6FB482F44 — COUNTER_OR_HOLD

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

#### AUD_SAR_05F0CCD5C5C181 — COUNTER_OR_HOLD

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

#### AUD_SAR_23A964DEF93E0B — COUNTER_OR_HOLD

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

#### AUD_SAR_54D489724E7990 — COUNTER_OR_HOLD

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

#### AUD_SAR_37FCBD8C4FA025 — COUNTER_OR_HOLD

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

#### AUD_SAR_20F1F3BE77A233 — COUNTER_OR_HOLD

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

#### AUD_SAR_A65DC6F88ED41F — COUNTER_OR_HOLD

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

#### AUD_SAR_82B079B5FFBD13 — SellerPriceAccepted

Our bargain is already set. Bring the agreed goods to complete the trade.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:47`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. AcceptedTerms already bound; brain returns finished ACCEPT; ordinarily authored agreement replaces this.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_43A5EEC613A15A — PlayerAcceptedDeal

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

#### AUD_SAR_98C3546B16B711 — SellerPriceAccepted

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

#### AUD_SAR_6982E6BDC3A064 — SellerPriceAccepted

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

#### AUD_SAR_E80439342ECFCE — SocialGreeting

Good day. I am {buyerName} of {buyerOrigin}. I am here for {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:956`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetGreetingLine Strict.
- Restrictions: {}
- Placeholders: buyerName, buyerOrigin, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I am Saraswati Chetti of Local Vijayanagara Retail Shop Owner. I am here for 2 Seers (~560g) of black pepper.
- Existing TTS input example: Good day. I am Saraswati Chetti of Local Vijayanagara Retail Shop Owner. I am here for 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ". I am here for "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_7F6EA122E7C378 — SocialGreeting

I am {buyerName} of {buyerOrigin}. I need {quantity} of {spiceDescriptor} quickly.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:960`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetGreetingLine Impatient.
- Restrictions: {}
- Placeholders: buyerName, buyerOrigin, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am Saraswati Chetti of Local Vijayanagara Retail Shop Owner. I need 2 Seers (~560g) of black pepper quickly.
- Existing TTS input example: I am Saraswati Chetti of Local Vijayanagara Retail Shop Owner. I need 2 Seers of black pepper quickly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ". I need "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": " quickly."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_0D33775EF08194 — SocialGreeting

Greetings. I am {buyerName} of {buyerOrigin}, and I seek {quantity} of {spiceDescriptor}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:962`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other greeting personality.
- Restrictions: {}
- Placeholders: buyerName, buyerOrigin, quantity, spiceDescriptor; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings. I am Saraswati Chetti of Local Vijayanagara Retail Shop Owner, and I seek 2 Seers (~560g) of black pepper.
- Existing TTS input example: Greetings. I am Saraswati Chetti of Local Vijayanagara Retail Shop Owner, and I seek 2 Seers of black pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{buyerOrigin}"}, {"kind": "LITERAL", "text": ", and I seek "}, {"kind": "VALUE", "text": "{quantity}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceDescriptor}"}, {"kind": "LITERAL", "text": "."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_0912CBDA7FB632 — WeatherQuestion

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

#### AUD_SAR_2BA7CC0D81122E — WeatherQuestion

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

#### AUD_SAR_F0C84A74979BFB — HistoryQuestion

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

#### AUD_SAR_5CF99A8F9A79A8 — SocialGreeting

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

#### AUD_SAR_944E9D58CFCE6B — OffTopic

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

#### AUD_SAR_2EFD8EC9168C45 — OffTopic

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

#### AUD_SAR_0392AE223D9105 — OffTopic

Enough. Speak of the trade, or I leave.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1024`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. OFF_TOPIC third count.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_4F974863293736 — HostileWarning

Speak with respect in this bazaar, or I will take my business elsewhere.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1031`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE first count.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_0DEDB6EA889365 — HostileWarning

You try my patience. One more insult and this trade ends.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1035`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE second count.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_C022E0E574B8A6 — HostileWarning

Mind your tongue, merchant. This is your last warning.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1037`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE third count only if frustration remains <0.75; earlier walk-away gates otherwise apply.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

### 06 Clarification and precise continuation

#### AUD_SAR_C910E41DB9D2FD — UnclearSpeech

Speak plainly, merchant.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:88`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Null trade or input; result.requiresClarification=true.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_EC6365C608C06C — Unknown

That price is suspiciously low. I will not agree to it.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:228`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ACCEPT hasSellerPrice and lastSellerPrice <0.3*targetMarketValue; nonfinal refusal.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_C82A8D4A0BD5AD — UnclearSpeech

State your final price clearly, merchant.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:255`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ULTIMATUM without seller price; requiresClarification=true.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_D39608439B85DA — UnclearSpeech

I did not catch your price. Say it plainly.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:308`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. PRICE/QUANTITY_PRICE missing valid price; requiresClarification=true.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_B7B550A000BBB7 — UnclearSpeech

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

#### AUD_SAR_9B26B06E9E028A — UnclearSpeech

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

#### AUD_SAR_A1AF5BDA228EDB — UnclearSpeech

Our bargain is already set. Bring the goods I asked for to complete the trade.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:735`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. parseReason=FulfillmentExpected, after UnresolvedTerms check.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_6FEF5636769CA4 — UnclearSpeech

The market is noisy. I heard nothing clearly. Say your price again.

- Classification: **FALLBACK_ONLY** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:740`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. parseReason=EmptyTranscript.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_9ADFCE035F6F42 — UnclearSpeech

The market is busy and your words were unclear. Say it again plainly.

- Classification: **REACHABLE_OPTIONAL** / NESTED_REACHABLE
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:745`
- Trigger/evidence: Optional topic, unusual terms or a guarded residual response; source route exists subject to stated conditions. parseReason=UnrecognizedSpeech.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_0E1FE8AE090C01 — UnclearSpeech

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

#### AUD_SAR_086B46D00420A9 — UnclearSpeech

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

#### AUD_SAR_EC6B9FE215122E — UnclearSpeech

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

#### AUD_SAR_3C9CBA39600499 — UnclearSpeech

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

#### AUD_SAR_BC42B344E7EB9B — UnclearSpeech

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

#### AUD_SAR_E8C5FF986005DD — UnclearSpeech

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

#### AUD_SAR_CC4A465EFF3043 — UnclearSpeech

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

#### AUD_SAR_09BDAE294199F1 — TransactionFailure

Very well. I will take my business elsewhere.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:122`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit DISMISS.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_5E988AD70877BD — TransactionFailure

I am done with such talk. We shall not trade today.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:172`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. OFF_TOPIC reaches outOfWorldCount>=4.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_5C8919BF40CB66 — TransactionFailure

Speak with respect in this bazaar, or we are finished.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:183`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. HOSTILE count>=4, or count>=3 with frustration>=0.75.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_4F1F1EB8D3A4AC — TransactionFailure

Very well. We shall not trade today.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:200`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit hard REJECT; early terminal return before counter computation.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_CB146E1EFA4F55 — TransactionFailure

Then we shall not trade today.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:267`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Ultimatum above max and ShouldWalkAwayFromHighPrice.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording. Additional source unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1086; Pressure walk-away Strict (same wording also ultimatum).

#### AUD_SAR_52C8B03A413EF6 — TransactionFailure

That price is too suspicious. I will take my leave.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:326`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit price per gram <35% target market value per gram, lowPriceCount>=3.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_CE5C34F9285932 — TransactionFailure

Your demand is too high. Then we shall not trade.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:340`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Explicit price above upper walk-away range and pressure exhausted.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_E9DF1BA83C22BF — TransactionFailure

{buyerName} will not linger longer. We trade another day.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1082`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Pressure walk-away Impatient.
- Restrictions: {}
- Placeholders: buyerName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Saraswati Chetti will not linger longer. We trade another day.
- Existing TTS input example: Saraswati Chetti will not linger longer. We trade another day.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " will not linger longer. We trade another day."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

#### AUD_SAR_DB510B713E4082 — TransactionFailure

Very well. {buyerName} will return another day.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/RuleBasedNPCBrain.cs:1088`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Other pressure walk-away.
- Restrictions: {}
- Placeholders: buyerName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Very well. Saraswati Chetti will return another day.
- Existing TTS input example: Very well. Saraswati Chetti will return another day.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Very well. "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": " will return another day."}]
- Note: Normally replaces or nests under authored presentation; do not treat every brain literal as another routinely heard recording.

## GENERIC_BYPASSED

### 01 Arrival and repeat visit

#### AUD_SAR_78854D4925FC7F — CustomerGreeting

Greetings, merchant. I seek {quantityLabel} of {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:168`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings, merchant. I seek 2 Seers (~560g) of pepper.
- Existing TTS input example: Greetings, merchant. I seek 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings, merchant. I seek "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_C0EC64291C1C66 — CustomerGreeting

Good day. I have come for {quantityLabel} of {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:169`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I have come for 2 Seers (~560g) of pepper.
- Existing TTS input example: Good day. I have come for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I have come for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_300CDCF55B955B — RepeatCustomerGreeting

We have already met, merchant. Let us return to the bargain.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:173`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 02 Trade information and quantity

#### AUD_SAR_ACEC0A16D6A487 — AskWhatBuyerWants

I am here to buy {quantityLabel} of {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:177`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: quantityLabel, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am here to buy 2 Seers (~560g) of pepper.
- Existing TTS input example: I am here to buy 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am here to buy "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_78512EA74DD6D7 — AskWhatBuyerWants

The {spiceName} is what brings me to your stall.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:178`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: The pepper is what brings me to your stall.
- Existing TTS input example: The pepper is what brings me to your stall.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "The "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " is what brings me to your stall."}]
- Note: 

#### AUD_SAR_94F9054AF9A4F9 — AskBuyerBudget

My offer stands at {currentBuyerOffer} {currency} for {quantityLabel}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:182`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency, quantityLabel; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer stands at 234 varahas for 2 Seers (~560g).
- Existing TTS input example: My offer stands at 234 varahas for 2 Seers .
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer stands at "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_AA7605D7C3680C — AskBuyerBudget

I can pay {currentBuyerOffer} {currency} at present.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:183`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I can pay 234 varahas at present.
- Existing TTS input example: I can pay 234 varahas at present.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I can pay "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " at present."}]
- Note: 

#### AUD_SAR_0F28DBA3C9BBD7 — AskQuantity

State the quantity clearly before we settle the price.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:187`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_A73DF37BD9C7BA — AskQuantity

How much {spiceName} do you mean to sell?

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:188`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: How much pepper do you mean to sell?
- Existing TTS input example: How much pepper do you mean to sell?
- Exact assembly pattern: [{"kind": "LITERAL", "text": "How much "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " do you mean to sell?"}]
- Note: 

### 03 Negotiation and rejection

#### AUD_SAR_8E2695605396FC — SellerPriceTooHigh

{offeredPrice} {currency} is far above what I can justify for {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:192`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: offeredPrice, currency, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: 300 varahas is far above what I can justify for pepper.
- Existing TTS input example: 300 varahas is far above what I can justify for pepper.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{offeredPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is far above what I can justify for "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_D89368D8E0904A — SellerPriceTooHigh

That price is too high for me. I cannot follow you there.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:193`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_6B7AEFE78580BF — SellerPriceSlightlyHigh

You ask a little too much, merchant. I can only move toward {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:197`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: You ask a little too much, merchant. I can only move toward 234 varahas.
- Existing TTS input example: You ask a little too much, merchant. I can only move toward 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "You ask a little too much, merchant. I can only move toward "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_1B999DABDFB4F9 — SellerPriceSlightlyHigh

That is above my reach. Let us come nearer to {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:198`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: That is above my reach. Let us come nearer to 234 varahas.
- Existing TTS input example: That is above my reach. Let us come nearer to 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "That is above my reach. Let us come nearer to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_F93B61D925CF68 — SellerPriceBelowExpected

That is below what I expected to hear from you.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:207`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_7D359BD244DA15 — SellerPriceBelowExpected

Your number is lower than I prepared for, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:208`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_A78C027363F371 — BuyerCounterFirst

My first counter is {counterPrice} {currency}. Consider it carefully.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:212`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My first counter is 234 varahas. Consider it carefully.
- Existing TTS input example: My first counter is 234 varahas. Consider it carefully.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My first counter is "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ". Consider it carefully."}]
- Note: 

#### AUD_SAR_8EBC4D458790D6 — BuyerCounterFirst

I begin at {counterPrice} {currency} for this {spiceName}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:213`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency, spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: I begin at 234 varahas for this pepper.
- Existing TTS input example: I begin at 234 varahas for this pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I begin at "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " for this "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_B3758B56B413D6 — BuyerCounterMiddle

We are narrowing the gap. I can offer {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:217`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: We are narrowing the gap. I can offer 234 varahas.
- Existing TTS input example: We are narrowing the gap. I can offer 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "We are narrowing the gap. I can offer "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_1DFFA9674B0C51 — BuyerCounterMiddle

I will come to {counterPrice} {currency}, but not freely.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:218`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I will come to 234 varahas, but not freely.
- Existing TTS input example: I will come to 234 varahas, but not freely.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I will come to "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": ", but not freely."}]
- Note: 

#### AUD_SAR_1A00947B3B4363 — BuyerCounterFinal

This is my last meaningful step: {counterPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:222`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: This is my last meaningful step: 234 varahas.
- Existing TTS input example: This is my last meaningful step: 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "This is my last meaningful step: "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_79EBFEAE13AAE4 — BuyerCounterFinal

I am nearly at my edge. {counterPrice} {currency} is my final counter.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:223`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: counterPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I am nearly at my edge. 234 varahas is my final counter.
- Existing TTS input example: I am nearly at my edge. 234 varahas is my final counter.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I am nearly at my edge. "}, {"kind": "VALUE", "text": "{counterPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": " is my final counter."}]
- Note: 

#### AUD_SAR_E6527F4943CE97 — BuyerHoldsFirm

I hold firm at {currentBuyerOffer} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:227`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: I hold firm at 234 varahas.
- Existing TTS input example: I hold firm at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I hold firm at "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_514B0B56DC45E6 — BuyerHoldsFirm

My offer does not move beyond {currentBuyerOffer} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:228`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: currentBuyerOffer, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: My offer does not move beyond 234 varahas.
- Existing TTS input example: My offer does not move beyond 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "My offer does not move beyond "}, {"kind": "VALUE", "text": "{currentBuyerOffer}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_F73B311DDAD3E1 — PlayerRejectedBuyerOffer

Then you refuse my offer.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:237`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_F4C2CCF5C792C3 — PlayerRejectedBuyerOffer

So be it. You reject the bargain I put forward.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:238`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 04 Price agreement, goods pending

#### AUD_SAR_82C8181D268295 — SellerPriceAccepted

That price is acceptable. We can conclude this bargain.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:202`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_59A08C36DCD1F4 — SellerPriceAccepted

Very well. That sits within reason for me.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:203`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_65C2E0560EE45D — PlayerAcceptedDeal

Then we are agreed at {finalPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:232`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: finalPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Then we are agreed at 234 varahas.
- Existing TTS input example: Then we are agreed at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Then we are agreed at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_6F4729A9091B8C — PlayerAcceptedDeal

Good. We will settle at {finalPrice} {currency}.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:233`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: finalPrice, currency; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good. We will settle at 234 varahas.
- Existing TTS input example: Good. We will settle at 234 varahas.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good. We will settle at "}, {"kind": "VALUE", "text": "{finalPrice}"}, {"kind": "LITERAL", "text": " "}, {"kind": "VALUE", "text": "{currency}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

### 05 Social, history and conduct

#### AUD_SAR_BFDAE4525FA42D — HistoryQuestion

There is time for stories later. First, let us finish this trade.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:242`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_AB4A9850AC700A — HistoryQuestion

History can wait, merchant. Speak of {spiceName} and price.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:243`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: spiceName; type: DYNAMIC_TEMPLATE
- Raw Unity example: History can wait, merchant. Speak of pepper and price.
- Existing TTS input example: History can wait, merchant. Speak of pepper and price.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "History can wait, merchant. Speak of "}, {"kind": "VALUE", "text": "{spiceName}"}, {"kind": "LITERAL", "text": " and price."}]
- Note: 

#### AUD_SAR_C45BA0C6200267 — SocialGreeting

A courteous word is welcome, but trade is why I am here.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:247`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_7A00979E5C7EB7 — SocialGreeting

Greetings to you as well. Now let us return to the bargain.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:248`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_86AD4D4CF98968 — OffTopic

Let us keep to the trade, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:252`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_E39FFA2C211C31 — OffTopic

Speak of price or quantity, not of other matters.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:253`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 06 Clarification and precise continuation

#### AUD_SAR_4C29AD3BB3861D — UnclearSpeech

I did not catch that clearly. {ruleReply}

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:257`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: I did not catch that clearly. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: I did not catch that clearly. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "I did not catch that clearly. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: 

#### AUD_SAR_3B2FD666A89550 — UnclearSpeech

Your meaning is not clear to me. {ruleReply}

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:258`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: Your meaning is not clear to me. The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: Your meaning is not clear to me. The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Your meaning is not clear to me. "}, {"kind": "VALUE", "text": "{ruleReply}"}]
- Note: 

#### AUD_SAR_647B136DC6E1EF — Unknown

{ruleReply}

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:276`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: ruleReply; type: NESTED_DYNAMIC
- Raw Unity example: The market is busy and your words were unclear. Say it again plainly.
- Existing TTS input example: The market is busy and your words were unclear. Say it again plainly.
- Exact assembly pattern: [{"kind": "VALUE", "text": "{ruleReply}"}]
- Note: 

### 07 Waiting, completion and departure

#### AUD_SAR_E63171B3825F12 — TransactionSuccess

The trade is complete and honourably settled.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:262`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_147AC0322ECB81 — TransactionFailure

Then this bargain ends without a sale.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:266`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_E27141A0C342A5 — TransactionFailure

We part without agreement today.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:267`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_B2D28F0AAB8BAF — TimePressure

My patience is spent. Decide now or we are done.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:271`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_733FBAB24A27A8 — TimePressure

Time runs thin, merchant. I cannot linger longer.

- Classification: **UNREACHABLE_OR_DEFERRED** / NOT_ROUTED_FOR_CHARACTER
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueCharacterRegistry.cs:272`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. FindTemplate returns FindRestrictedTemplate directly for known authored saraswati_chetti; generic lookup is bypassed.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

## PRESENTATION_FALLBACK

### 01 Arrival and repeat visit

#### AUD_SAR_B9E96D60759F75 — CustomerGreeting

Greetings, merchant. I am {buyerName}, and I seek {quantityLabel} of {spice}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:335`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: buyerName, quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Greetings, merchant. I am Saraswati Chetti, and I seek 2 Seers (~560g) of pepper.
- Existing TTS input example: Greetings, merchant. I am Saraswati Chetti, and I seek 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Greetings, merchant. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ", and I seek "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_46FC4996E4C59B — CustomerGreeting

Good day. I am {buyerName}. I am here for {quantityLabel} of {spice}.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:337`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: buyerName, quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Good day. I am Saraswati Chetti. I am here for 2 Seers (~560g) of pepper.
- Existing TTS input example: Good day. I am Saraswati Chetti. I am here for 2 Seers of pepper.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Good day. I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ". I am here for "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": "."}]
- Note: 

#### AUD_SAR_C7B6958C60889B — CustomerGreeting

Merchant, I am {buyerName}. I need {quantityLabel} of {spice} quickly.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/LocalTradeSessionGenerator.cs:339`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. ChatManager uses localSession.greetingText only when authored GetGreeting returns empty. Friendly/Strict/Impatient/default switch.
- Restrictions: {}
- Placeholders: buyerName, quantityLabel, spice; type: DYNAMIC_TEMPLATE
- Raw Unity example: Merchant, I am Saraswati Chetti. I need 2 Seers (~560g) of pepper quickly.
- Existing TTS input example: Merchant, I am Saraswati Chetti. I need 2 Seers of pepper quickly.
- Exact assembly pattern: [{"kind": "LITERAL", "text": "Merchant, I am "}, {"kind": "VALUE", "text": "{buyerName}"}, {"kind": "LITERAL", "text": ". I need "}, {"kind": "VALUE", "text": "{quantityLabel}"}, {"kind": "LITERAL", "text": " of "}, {"kind": "VALUE", "text": "{spice}"}, {"kind": "LITERAL", "text": " quickly."}]
- Note: 

#### AUD_SAR_A383DA81A05618 — CustomerGreeting

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

#### AUD_SAR_210C40327571AF — IdleReminderFirst

Well?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:92`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_95679342FFFF15 — IdleReminderFirst

Take your time, but don't keep me waiting.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:93`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_7164CB920051F5 — IdleReminderSecond

I don't have all day. What's your decision?

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:97`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_609E0E5A59C47C — IdleReminderSecond

Other merchants are waiting for my business.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:98`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_4E5911AD4C2F76 — TransactionFailure

Enough. I am leaving.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:102`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_8D61E64EAFF350 — TransactionFailure

You have wasted enough of my time.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/MarketplaceManager.cs:103`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Marketplace passes fallback to ChatManager; authored GetIdleReminder replaces it for Lakshmamma. First/second/final stage respectively.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_CC12A24CB6D02A — TransactionFailure

My patience is spent. I will take my leave.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_USUALLY_MASKED
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:1561`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. TryHandleNegotiationTimeout default finalLine; authored terminal GetIdleReminder replaces it.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

## PROVIDER_FALLBACK

### 01 Arrival and repeat visit

#### AUD_SAR_D81F2B3BD9343A — CustomerGreeting

Greetings, merchant.

- Classification: **UNREACHABLE_OR_DEFERRED** / CONTEXT_ONLY
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:130`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. Greeting context.ruleReply only; greeting templates do not include ruleReply.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 06 Clarification and precise continuation

#### AUD_SAR_CC19E934B5F42C — UnclearSpeech

Speak plainly, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_FALLBACK
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:577`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. Blank brain.replyText fallback for context.ruleReply and ReplacePlaceholders.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

### 07 Waiting, completion and departure

#### AUD_SAR_4AF37DE80A4004 — TransactionSuccess

Thank you, merchant.

- Classification: **FALLBACK_ONLY** / DEFENSIVE_FALLBACK
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs:73`
- Trigger/evidence: Precise nested/defensive source or provider exception fallback; normally masked where an authored response exists. GetPresentation fallback after settlement, if no authored match or exception.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

## UI_ONLY_RECOVERY

### 06 Clarification and precise continuation

#### AUD_SAR_DBAFFCF412DB88 — UnclearSpeech

The market is too noisy, could you repeat that?

- Classification: **UNREACHABLE_OR_DEFERRED** / TEXT_ONLY_NOT_SPOKEN
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:554`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. Null trade in local turn or error in legacy OnNPCReply; assigned npcText only, not sent to PresentNpcSubtitleAndTts.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 

#### AUD_SAR_15A1EF806B11DD — UnclearSpeech

The market is too noisy, could you repeat that?

- Classification: **UNREACHABLE_OR_DEFERRED** / TEXT_ONLY_NOT_SPOKEN
- Source: `unity/StorytellingVR/Assets/_Project/Scripts/Level1/ChatManager.cs:894`
- Trigger/evidence: No normal route for this character, preempted branch, or text-only UI; excluded from the recording requirement. Null trade in local turn or error in legacy OnNPCReply; assigned npcText only, not sent to PresentNpcSubtitleAndTts.
- Restrictions: {}
- Placeholders: none; type: FULL_CLIP
- Note: 
