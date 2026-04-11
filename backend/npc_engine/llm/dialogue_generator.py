import random


_LAST_VARIANT_INDEX = {}


def pick_varied(key, options):
    if len(options) == 1:
        return options[0]

    last_index = _LAST_VARIANT_INDEX.get(key)
    available_indexes = [i for i in range(len(options)) if i != last_index]
    choice_index = random.choice(available_indexes)
    _LAST_VARIANT_INDEX[key] = choice_index
    return options[choice_index]


def offer_prefix_for_personality(personality, negotiation_stage, frustration):
    if frustration >= 0.8:
        return ""

    if personality == "Aggressive Trader":
        if negotiation_stage == "FINALIZATION":
            return ""
        return pick_varied("offer_prefix:aggressive", ["Alright... ", "Well... ", "I see... ", "Hmm... ", ""])

    if personality == "Polite Merchant":
        return pick_varied("offer_prefix:polite", ["Well... ", "Hmm... ", "Alright... ", "I see... ", ""])

    return pick_varied("offer_prefix:cautious", ["Hmm... ", "I see... ", "Alright... ", "Well... ", ""])


def merge_prefix_and_line(prefix, line):
    if not prefix:
        return line

    normalized_line = line.lstrip()
    normalized_lower = normalized_line.lower()
    prefix_lower = prefix.strip().lower()

    conflicting_starters = {
        "hmm... ": ["hmm...", "let me think...", "i am not sure..."],
        "let me think... ": ["hmm...", "let me think...", "i am not sure..."],
        "i am not sure... ": ["hmm...", "let me think...", "i am not sure..."],
        "well... ": ["well...", "perhaps...", "if i may..."],
        "perhaps... ": ["well...", "perhaps...", "if i may..."],
        "if i may... ": ["well...", "perhaps...", "if i may..."],
        "fine. ": ["fine.", "enough."],
        "enough. ": ["fine.", "enough."],
        "alright... ": ["alright..."],
        "i see... ": ["i see..."]
    }

    for starter in conflicting_starters.get(prefix_lower, []):
        if normalized_lower.startswith(starter):
            return normalized_line

    return prefix + normalized_line


def format_quantity_grams(quantity_grams):
    if quantity_grams is None:
        return "1kg"
    if quantity_grams >= 1000 and quantity_grams % 1000 == 0:
        return f"{int(quantity_grams / 1000)}kg"
    if quantity_grams >= 1000:
        return f"{round(quantity_grams / 1000, 2)}kg"
    return f"{int(quantity_grams) if float(quantity_grams).is_integer() else quantity_grams} grams"


def offer_lines_for_personality(personality, price, negotiation_stage, frustration, trust):
    if frustration >= 0.8:
        if personality == "Polite Merchant":
            return [
                f"I must remain at {price} varahas.",
                f"I cannot go beyond {price} varahas.",
                f"{price} varahas is my limit."
            ], ""
        if personality == "Cautious Buyer":
            return [
                f"Hmm... I cannot go beyond {price}.",
                f"I think {price} varahas is my limit.",
                f"Perhaps {price} is as far as I can go."
            ], ""
        return [
            f"{price}. Final.",
            f"I said {price}.",
            f"{price} varahas. No more.",
            f"Take {price} or leave it."
        ], ""

    if personality == "Aggressive Trader":
        if negotiation_stage == "OPENING":
            return [
                f"{price}.",
                f"I will give {price}.",
                f"I open at {price}.",
                f"{price} varahas. Start there.",
                f"Too high. I will give {price}.",
                f"You must do better. {price}.",
                f"That is my price: {price}.",
                f"I will go as far as {price}."
            ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
        if negotiation_stage == "FINALIZATION":
            return [
                f"We settle at {price}.",
                f"{price}. That is close enough.",
                "We are settled then.",
                f"I will give {price}. Take it or leave it.",
                f"{price} varahas. We are close.",
                "That should conclude it.",
                f"I can go to {price}, no further.",
                f"{price}. Finish it.",
                "Let us finish this.",
                f"That is my price: {price}.",
                f"I would settle at {price}."
            ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
        return [
            f"We are getting closer. {price}.",
            f"{price}.",
            f"I will give {price}.",
            f"Call it {price}.",
            f"Still high. {price}.",
            f"We are closer. {price}.",
            f"I can go to {price}.",
            f"{price} is fair to me.",
            f"I will go as far as {price}."
        ], offer_prefix_for_personality(personality, negotiation_stage, frustration)

    if personality == "Polite Merchant":
        if negotiation_stage == "OPENING":
            return [
                f"Let us begin at {price}.",
                f"Perhaps I could begin at {price} varahas.",
                f"That seems a little high to me. I could offer {price}.",
                f"If you agree, I would begin with {price} varahas.",
                f"I could offer {price} varahas, if that seems fair.",
                f"Perhaps {price} varahas would be a reasonable start.",
                f"I would settle first at {price} varahas, if you agree."
            ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
        if negotiation_stage == "FINALIZATION":
            return [
                f"Let us settle at {price}.",
                f"We are getting closer. I could offer {price} varahas, if you agree.",
                "We are settled then.",
                f"That is nearer to my expectation. Perhaps {price} varahas?",
                f"If it suits you, I could settle at {price} varahas.",
                "That should conclude it.",
                f"I believe {price} varahas would conclude this fairly.",
                f"Perhaps we may finish this at {price} varahas.",
                "Let us finish this.",
                f"I would settle at {price} varahas.",
                f"{price} is fair to me, if you agree."
            ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
        if trust >= 0.7:
            return [
                f"I could offer {price} varahas, if that suits you.",
                f"With respect, I would settle at {price} varahas.",
                f"Let us say {price}.",
                f"Perhaps {price} varahas would be fair between us.",
                f"If you agree, I could offer {price} varahas.",
                f"I believe {price} varahas would be a fair arrangement.",
                f"{price} is fair to me."
            ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
        return [
            f"Perhaps I could offer {price} varahas.",
            f"I could offer {price} varahas, if you agree.",
            f"Let us say {price}.",
            f"That seems a little high to me. I could offer {price}.",
            f"Perhaps {price} varahas would suit us both.",
            f"If you agree, I would say {price} varahas.",
            f"I would settle at {price}.",
            f"{price} is fair to me."
        ], offer_prefix_for_personality(personality, negotiation_stage, frustration)

    if negotiation_stage == "OPENING":
        return [
            f"Let us begin at {price}.",
            f"Hmm... do you have room for {price}?",
            f"I am not sure about that. Perhaps {price}?",
            f"Maybe I could begin at {price} varahas.",
            f"I think {price} might be fair to start.",
            f"Hmm... perhaps {price} varahas?",
            f"Maybe {price} is a fair place to begin."
        ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
    if negotiation_stage == "FINALIZATION":
        return [
            f"Let us settle at {price}.",
            f"Hmm... we are getting close. Perhaps {price}?",
            "We are settled then.",
            f"I think {price} is nearer to what I expected.",
            f"Maybe we could finish at {price} varahas?",
            "That should conclude it.",
            f"I am almost satisfied. Perhaps {price}?",
            f"Hmm... I think I could go to {price}.",
            "Let us finish this.",
            f"I would settle at {price}, I think."
        ], offer_prefix_for_personality(personality, negotiation_stage, frustration)
    return [
        f"We are getting closer. {price}.",
        f"Hmm... I think I could offer {price}.",
        f"Let us say {price}.",
        f"Maybe {price} varahas?",
        f"I am not sure about that. Perhaps {price}?",
        f"Hmm... we are closer now. I could offer {price}.",
        f"I think {price} might work for me.",
        f"{price} is fair to me.",
        f"I will go as far as {price}."
    ], offer_prefix_for_personality(personality, negotiation_stage, frustration)


def generate_dialogue(decision, personality, item_name):
    if hasattr(decision, "action"):
        action = decision.action
        price = decision.price
        quantity = decision.quantity
        reason = decision.reason
    else:
        action = decision
        price = None
        quantity = None
        reason = None

    reason_prefix = {
        "TOO_HIGH": "That is too high.",
        "TOO_EXPENSIVE": "That is too high.",
        "FAIR": "That seems fair.",
    }.get(reason)

    context = {}
    frustration = 0.0
    trust = 0.5
    negotiation_stage = getattr(decision, "stage", "BARGAINING")
    display_item_name = item_name

    if action == "SET_QUANTITY":
        quantity_text = format_quantity_grams(quantity)
        return pick_varied(
            f"set_quantity:{personality}",
            [
                f"I want {quantity_text} of {item_name}.",
                f"Give me {quantity_text}.",
                f"Perhaps {quantity_text} of {item_name}."
            ]
        )

    if action == "ASK_ITEM":
        ask_item_name = item_name
        if context.get("last_quantity_grams") not in [None, 1000]:
            ask_item_name = display_item_name

        if personality == "Aggressive Trader":
            ask_lines = [
                f"Do you have {ask_item_name} or not?",
                f"Show me your {ask_item_name}.",
                f"Are you selling {ask_item_name}?"
            ]
        elif personality == "Polite Merchant":
            ask_lines = [
                f"Do you happen to have {ask_item_name}?",
                f"May I ask if you are selling {ask_item_name}?",
                f"I am looking for {ask_item_name}."
            ]
        else:
            ask_lines = [
                f"Do you have {ask_item_name}?",
                f"Are you selling {ask_item_name} today?",
                f"I am looking for {ask_item_name}."
            ]
        return pick_varied(f"ask_item:{personality}", ask_lines)

    item_name = display_item_name

    if action == "ACCEPT":
        if personality == "Aggressive Trader":
            accept_lines = [
                f"Alright. {price}. Done.",
                f"Fine. {price}, we're done.",
                f"Take it then. {price}.",
                f"Done. {price} it is.",
                f"We have a deal. {price}."
            ]
        elif personality == "Polite Merchant":
            accept_lines = [
                f"That sounds fair. {price} varahas.",
                f"Very well, we have a deal at {price}.",
                f"Agreed. Let's settle on {price}.",
                f"I can agree to {price} varahas.",
                f"That works for me. {price} varahas."
            ]
        else:
            accept_lines = [
                f"Hmm... alright, that works. {price}.",
                f"I think that's fair. Let's settle it at {price}.",
                f"I suppose we have a deal. {price}.",
                f"Very well... {price} it is.",
                f"Alright then. {price} works."
            ]
        return pick_varied(f"accept:{personality}", accept_lines)

    if action == "OUT_OF_WORLD":
        stage = context.get("stage", "confused")

        if personality == "Aggressive Trader":
            if stage == "annoyed":
                out_of_world_lines = [
                    "I have already told you that such nonsense means nothing to me.",
                    "Enough of these absurdities. Speak of trade.",
                    "You test my patience with this foolish talk.",
                    "I will not indulge this nonsense again.",
                    "These strange words weary me. Return to business."
                ]
            elif stage == "warning":
                out_of_world_lines = [
                    "Enough. Speak of the goods, or I will leave.",
                    "This is your last chance to speak of trade.",
                    "If you continue with this nonsense, I am done here.",
                    "I warn you now: return to the bargain, or I walk away.",
                    "Speak plainly of trade, or this ends now."
                ]
            else:
                out_of_world_lines = [
                    "I have no time for such nonsense.",
                    "Speak sense, not idle absurdities.",
                    "Those foolish words mean nothing to me.",
                    "I care nothing for such strange nonsense.",
                    "Keep your mind on trade, not childish inventions.",
                    "That is useless talk. Speak plainly of the goods.",
                    "I will not waste my patience on such nonsense."
                ]
        elif personality == "Polite Merchant":
            if stage == "annoyed":
                out_of_world_lines = [
                    "I am still unfamiliar with such things, and we are straying from the bargain.",
                    "I must ask again that we return to the matter of trade.",
                    "Those matters remain unknown to me, and I would rather speak of the goods.",
                    "I mean no rudeness, but this is leading us away from business.",
                    "I cannot help with such things, and we should return to the market at hand."
                ]
            elif stage == "warning":
                out_of_world_lines = [
                    "I must insist that we return to trade, or I shall take my leave.",
                    "Forgive me, but if this continues, I cannot remain.",
                    "Let us speak of the goods now, or I must withdraw from this exchange.",
                    "I would rather not leave, yet I will if we abandon the trade entirely.",
                    "Please return to the bargain, or I shall look elsewhere."
                ]
            else:
                out_of_world_lines = [
                    "I am unfamiliar with such things.",
                    "I do not know of such a matter, I confess.",
                    "Those words are strange to me, though I mean no offense.",
                    "I cannot claim any knowledge of such a thing.",
                    "That is beyond my experience, I am afraid.",
                    "I know nothing of such devices, but I would gladly discuss the goods.",
                    "Such a matter is unknown to me, though trade I understand well."
                ]
        else:
            if stage == "annoyed":
                out_of_world_lines = [
                    "I still do not understand... can we please return to the trade?",
                    "That remains unfamiliar, and I would rather speak of the goods.",
                    "I am growing uncertain where this is going. Are we still bargaining?",
                    "I do not follow this talk, and it is making the trade difficult.",
                    "I would prefer we return to the price before I lose interest."
                ]
            elif stage == "warning":
                out_of_world_lines = [
                    "I do not understand this at all. If we are not trading, I will leave.",
                    "Please return to the bargain, or I shall go elsewhere.",
                    "I am too uncertain to continue if we abandon the trade now.",
                    "If this is no longer about the goods, then I should leave.",
                    "I must ask one last time: are we trading or not?"
                ]
            else:
                out_of_world_lines = [
                    "I do not understand... are we still trading?",
                    "That sounds unfamiliar to me. Are we speaking of the goods still?",
                    "I know nothing of that. Shall we return to the bargain?",
                    "I am not certain what that is meant to be.",
                    "That leaves me unsure. Are we still discussing the trade?",
                    "I do not follow such talk. Perhaps we should return to the price.",
                    "That sounds strange to me, and I would rather speak of the goods."
                ]

        out_of_world_transitions = [
            f"Now, what price do you propose for the {item_name}?",
            f"Let us return to the {item_name}. I can offer {price} varahas.",
            f"Still, trade remains. What do you ask for the {item_name}?",
            f"Now then, let us settle the price of the {item_name}.",
            f"Even so, I can offer {price} varahas for the {item_name}.",
            "Let us return to business.",
            "Now, shall we speak of the bargain before us?"
        ]

        out_of_world_line = pick_varied(f"out_of_world:{personality}", out_of_world_lines)
        out_of_world_transition = pick_varied(f"out_of_world_transition:{personality}", out_of_world_transitions)
        return out_of_world_line + " " + out_of_world_transition

    if action == "HOSTILE":
        stage = context.get("stage", "warning")
        terse_hostile = frustration >= 0.75

        if personality == "Aggressive Trader":
            if stage == "stronger":
                hostile_lines = [
                    "That tongue of yours is wearing thin.",
                    "I warned you once. Do not press me again.",
                    "Your manner grows tiresome.",
                    "Enough of this insolence.",
                    "You push this bargain toward ruin."
                ]
            elif stage == "final_warning":
                hostile_lines = [
                    "This is your last warning.",
                    "One more insult and I am gone.",
                    "I will not endure another word like that.",
                    "You stand at the edge of ending this bargain.",
                    "Speak properly now, or we are finished."
                ]
            else:
                hostile_lines = [
                    "Mind your tongue.",
                    "Speak with respect, or we are done.",
                    "I will not be insulted in my own trade.",
                    "Enough. Show some sense.",
                    "Try that again, and I leave.",
                    "Watch your tongue.",
                    "Speak properly or leave.",
                    "I will not tolerate such behavior.",
                    "You are close to losing this bargain.",
                    "Do not test me with that tone."
                ]
            if terse_hostile:
                hostile_lines = [
                    "Enough.",
                    "Watch yourself.",
                    "Speak properly.",
                    "Careful now.",
                    "Hold your tongue."
                ]
        elif personality == "Polite Merchant":
            if stage == "stronger":
                hostile_lines = [
                    "I must insist on a respectful tone.",
                    "We cannot bargain well through insults.",
                    "Please understand that such language is not acceptable.",
                    "I have been patient, but this must stop.",
                    "Let us keep this civil, or not continue at all."
                ]
            elif stage == "final_warning":
                hostile_lines = [
                    "This will be my final warning on the matter.",
                    "If this tone continues, I will leave.",
                    "I will not remain for another insult.",
                    "Please choose civility now, or we end this here.",
                    "We are one harsh word away from ending the bargain."
                ]
            else:
                hostile_lines = [
                    "There is no need for such language.",
                    "I would ask that you speak with courtesy.",
                    "I will continue only if you remain civil.",
                    "Please keep this exchange respectful.",
                    "Such words do not help our bargain.",
                    "Such words are unnecessary.",
                    "Let us remain respectful.",
                    "I would prefer a civil discussion.",
                    "We may continue, but only with courtesy.",
                    "I ask only for a calmer tone."
                ]
            if terse_hostile:
                hostile_lines = [
                    "That is enough.",
                    "Please stop.",
                    "Be respectful.",
                    "No more of that.",
                    "Keep this civil."
                ]
        else:
            if stage == "stronger":
                hostile_lines = [
                    "You are making me uneasy now.",
                    "Please stop speaking that way.",
                    "This tone is becoming difficult to bear.",
                    "I do not wish to hear more insults.",
                    "We should calm this down at once."
                ]
            elif stage == "final_warning":
                hostile_lines = [
                    "I cannot continue if this happens again.",
                    "Please understand, this is my last warning.",
                    "Another insult, and I will leave.",
                    "I do not feel at ease continuing like this.",
                    "We must speak calmly now, or stop entirely."
                ]
            else:
                hostile_lines = [
                    "There is no cause for that tone.",
                    "I would rather keep this civil.",
                    "That is needlessly harsh.",
                    "Please speak more plainly and with restraint.",
                    "I do not care for insults.",
                    "That is unsettling...",
                    "I do not like this tone.",
                    "Perhaps we should proceed calmly.",
                    "You are making this harder than it needs to be.",
                    "I would prefer a quieter exchange."
                ]
            if terse_hostile:
                hostile_lines = [
                    "Please stop.",
                    "I do not like this.",
                    "Enough of that.",
                    "Speak calmly.",
                    "This must stop."
                ]

        if stage == "final_warning" or frustration >= 0.7:
            hostile_transitions = [
                f"Return to the price of the {item_name}, or this ends now.",
                "One more outburst, and I leave.",
                f"If we continue, we speak only of the {item_name}.",
                "Choose your next words carefully.",
                f"Speak of the bargain now, or we are finished."
            ]
        else:
            hostile_transitions = [
                f"Now, return to the price of the {item_name}.",
                f"If we continue, we speak of the {item_name} and nothing else.",
                f"Now then, what is your offer for the {item_name}?",
                "Let us return to trade with some sense.",
                f"We may continue, but only about the bargain before us.",
                f"Set the tone aside and speak of the {item_name}.",
                "If we remain calm, we may still continue."
            ]

        if terse_hostile:
            hostile_transitions = [
                "Back to trade.",
                "Speak of the bargain.",
                "Name your price.",
                "Return to business.",
                "Talk of the goods."
            ]

        hostile_line = pick_varied(f"hostile:{personality}", hostile_lines)
        hostile_transition = pick_varied(f"hostile_transition:{personality}", hostile_transitions)
        return hostile_line + " " + hostile_transition

    if action == "NO_ITEM":
        if personality == "Aggressive Trader":
            no_item_lines = [
                "Then I have no business here.",
                "You should have said that sooner."
            ]
        elif personality == "Polite Merchant":
            no_item_lines = [
                "I understand. Thank you for your time.",
                "Very well, I shall take my leave."
            ]
        else:
            no_item_lines = [
                "I see... then perhaps another time.",
                "That is unfortunate, but I understand."
            ]
        return pick_varied(f"no_item:{personality}", no_item_lines)

    if action == "REJECT":
        rejection_count = context.get("rejection_count", 1)

        if personality == "Aggressive Trader":
            reject_lines = [
                "Then why waste my time?",
                "You refuse without reason?",
                "You turn it down too quickly.",
                "If you reject it, speak plainly about the price."
            ]
        elif personality == "Polite Merchant":
            reject_lines = [
                "I understand, though that is unfortunate.",
                "Very well, perhaps another arrangement?",
                "I see. Then we may need a different price.",
                "That is fair to say, though I had hoped otherwise."
            ]
        else:
            reject_lines = [
                "I see... that gives me pause.",
                "Then we may need to reconsider this trade.",
                "I understand, though it makes the bargain uncertain.",
                "Then perhaps we should approach the price differently."
            ]

        if rejection_count >= 2:
            reject_transitions = [
                "We are close to ending this bargain.",
                "If this continues, I will leave.",
                "I have little patience left for this trade.",
                "Give me a better path forward, or this ends."
            ]
        else:
            reject_transitions = [
                f"I can still offer {price} varahas.",
                f"Then tell me what troubles you about {price} varahas.",
                f"Very well. My offer remains {price} varahas.",
                f"Let us try again. I can offer {price} varahas."
            ]

        reject_line = pick_varied(f"reject:{personality}", reject_lines)
        reject_transition = pick_varied(f"reject_transition:{rejection_count}", reject_transitions)
        return reject_line + " " + reject_transition

    if action == "SOCIAL_RESPONSE":
        social_sub_intent = context.get("social_sub_intent", "GENERAL")

        if social_sub_intent == "GREETING":
            social_lines = [
                "I am well, thank you.",
                "I fare well enough today.",
                "All is well on my side.",
                "Well enough, and ready for trade.",
                "I am in good spirits, thank you.",
                "I have no complaint this day."
            ]
        elif social_sub_intent == "WEATHER":
            social_lines = [
                "The weather has been fair.",
                "The air has been kind today.",
                "It has been a calm day for the market.",
                "The sky has treated us well enough.",
                "The day has been warm and steady.",
                "The weather has favored trade today."
            ]
        elif social_sub_intent == "DAILY_LIFE":
            social_lines = [
                "I have spent the day among stalls and ledgers.",
                "My day has passed in trade and travel.",
                "I have been about the market since morning.",
                "The day has been filled with bargaining and accounts.",
                "I have kept busy with goods, carts, and customers.",
                "Most of my day has gone to trade, as ever."
            ]
        elif social_sub_intent == "CONFUSION":
            social_lines = [
                "I do not follow. Could you clarify?",
                "I do not understand your meaning.",
                "Your words are unclear to me.",
                "I am not certain what you mean.",
                "That is unclear. Speak plainly, if you would.",
                "I do not quite follow your point."
            ]
        else:
            social_lines = [
                "All is well. Let us continue.",
                "A fair enough thought, though trade awaits.",
                "Such talk has its place, though business remains.",
                "A pleasant remark, but we still have trade to settle.",
                "We may speak of such things, yet business comes first.",
                "That is fair to say. Still, we are here to bargain."
            ]

        transition_lines = [
            f"Now, about the {item_name} - I can offer {price} varahas.",
            f"But let us return to trade. I can offer {price} varahas.",
            f"Now then, what price do you suggest?",
            f"Shall we return to business?",
            f"Now, what is your offer for the {item_name}?",
            f"Let us return to the {item_name}. I can offer {price} varahas.",
            f"Still, we should settle the price of the {item_name}.",
            f"Now then, let us finish this bargain over the {item_name}.",
            f"Even so, trade calls. What do you ask for the {item_name}?"
        ]

        social_line = pick_varied(f"social:{social_sub_intent}", social_lines)
        transition_line = pick_varied("social_transition", transition_lines)
        return social_line + " " + transition_line

    if action == "LOW_PRICE":
        stage = context.get("stage", "warning")

        if stage == "suspicious" or frustration >= 0.7:
            lines = [
                f"{price} is already a fair offer. Your price is suspiciously low.",
                "That price does not sound honest.",
                "You press too strangely low a price for this trade.",
                "This bargain is beginning to smell of trickery."
            ]
        else:
            lines = [
                "That price is unusually low.",
                "You ask too little for goods of this worth.",
                "Such a low price gives me pause.",
                "That price does not fit the market."
            ]

        transitions = [
            f"I will remain at {price} varahas.",
            f"If we trade, we trade fairly. I can offer {price} varahas.",
            f"Let us speak honestly of the {item_name}. My offer is {price} varahas.",
            "Name a fairer price, or we will not continue.",
            f"Return to a sensible bargain for the {item_name}."
        ]

        low_price_line = pick_varied(f"low_price:{stage}", lines)
        low_price_transition = pick_varied(f"low_price_transition:{stage}", transitions)
        return low_price_line + " " + low_price_transition

    if action == "WALK_AWAY":
        reason = reason
        if reason == "HOSTILE":
            return pick_varied("walk_away:hostile", [
                "I will not remain where I am spoken to so poorly.",
                "This bargain is over. I will take my leave.",
                "I have had enough of your insults. Farewell."
            ])
        if reason == "NO_ITEM":
            if personality == "Polite Merchant":
                return pick_varied("walk_away:no_item:polite", [
                    "I see, then I shall not trouble you further.",
                    "That is unfortunate, but I understand.",
                    "Very well, then I will take my leave with thanks.",
                    "I understand. Without the goods, there is no reason to trouble you."
                ])
            if personality == "Aggressive Trader":
                return pick_varied("walk_away:no_item:aggressive", [
                    "Then why did I come here?",
                    "You waste my time.",
                    "No goods? Then this visit was pointless.",
                    "If there is nothing to sell, I am done here."
                ])
            return pick_varied("walk_away:no_item:cautious", [
                "I see... then this ends here.",
                "Very well, I will look elsewhere.",
                "I understand. Then I should move on.",
                "Without the item, I think I should leave."
            ])
        if reason == "DISRESPECT":
            return pick_varied("walk_away:disrespect", [
                "You have spoken too poorly for me to continue this bargain.",
                "There will be no trade after such disrespect. Farewell.",
                "I will not conclude a deal in this manner. I am leaving."
            ])
        if reason == "NO_INTEREST":
            if personality == "Aggressive Trader":
                return pick_varied("walk_away:no_interest:aggressive", [
                    "Not worth it. I'm done.",
                    "Forget it.",
                    "Waste of time. I'm leaving.",
                    "We're done here."
                ])
            if personality == "Polite Merchant":
                return pick_varied("walk_away:no_interest:polite", [
                    "I appreciate your time, but I'll pass.",
                    "Thank you, but I'll look elsewhere.",
                    "We can't seem to agree. Farewell.",
                    "I will take my leave, thank you."
                ])
            return pick_varied("walk_away:no_interest:cautious", [
                "Hmm... I don't think this works for me.",
                "I'll think about it and come back later.",
                "Maybe another time...",
                "I think I'll pass for now."
            ])
        if reason == "SUSPICIOUS":
            return pick_varied("walk_away:suspicious", [
                "These prices are too suspicious for honest trade. I am leaving.",
                "I will not bargain where the price sounds false. Farewell.",
                "This no longer feels like fair trade. I will go elsewhere."
            ])
        if frustration >= 0.85:
            return pick_varied("walk_away:warning", [
                "Enough. I will not continue this any longer.",
                "I have had my fill of this. I am leaving.",
                "This has gone too far. I will take my leave."
            ])
        return random.choice([
            "I think I'll look elsewhere.",
            "We can't seem to reach an agreement.",
            "I'm going to pass."
        ])

    proposal_type = context.get("proposal_type")
    
    quantity_proposals = [
        "quantity_change", "bundle_offer", "quantity_reduce", "bundle_adjust", 
        "quantity_expect_more", "bundle_expect_more", "quantity_too_costly",
        "quantity_too_small", "reject_small_quantity"
    ]
    if proposal_type in quantity_proposals:
        if context.get("last_quantity_grams") is not None and context.get("current_quantity_grams") == context.get("last_quantity_grams"):
            lines = [
                "Alright, let's talk price.",
                "Fine, now the price.",
                "That works, what about the price?"
            ]
            if personality == "Aggressive Trader":
                transitions = [f"I give {price}.", f"My price is {price}.", f"Take {price}."]
            elif personality == "Polite Merchant":
                transitions = [f"I'll offer {price}.", f"How does {price} sound?", f"Perhaps {price}?"]
            else:
                transitions = [f"I think I could do {price}.", f"Let's say {price}.", f"I'd offer {price}."]
            return pick_varied("quantity_fixed", lines) + " " + pick_varied(f"quantity_fixed_transition:{personality}", transitions)

    if proposal_type == "price_increase":
        seller_price = context.get("seller_price")
        previous_seller_price = context.get("previous_seller_price")
        if personality == "Aggressive Trader":
            lines = [
                f"You raised it from {previous_seller_price} to {seller_price}. Why?",
                "You raise the price suddenly. No.",
                f"You said {previous_seller_price}, now {seller_price}. I do not chase that."
            ]
            transitions = [
                f"My offer is still {price}.",
                "Keep the price steady, or this ends.",
                f"I will not chase a moving price. {price}."
            ]
        elif personality == "Polite Merchant":
            lines = [
                f"Your price has increased from {previous_seller_price} to {seller_price}. Why?",
                "You raise the price suddenly. That is unusual.",
                f"You asked {previous_seller_price} before, and now {seller_price}. That gives me pause."
            ]
            transitions = [
                f"My offer remains {price} varahas.",
                "If we continue, I would ask that the bargain remain steady.",
                f"I would rather not chase a moving price. I can still offer {price}."
            ]
        else:
            lines = [
                f"Hmm... your price moved from {previous_seller_price} to {seller_price}?",
                "That changed rather suddenly.",
                f"You said {previous_seller_price} before, and now {seller_price}. I am not sure about that."
            ]
            transitions = [
                f"I think my offer should remain {price}.",
                "If we continue, I would prefer a steadier price.",
                f"Hmm... I cannot chase that. I can still offer {price}."
            ]
        return pick_varied("price_increase_line", lines) + " " + pick_varied("price_increase_transition", transitions)

    if proposal_type == "too_expensive_warning":
        seller_price = context.get("seller_price")
        lines = [
            "That's way too high.",
            "That is too much.",
            f"{seller_price}? Not a chance.",
            "You ask too much.",
            "Not worth it.",
            "I can't go that far."
        ]
        transitions = [
            f"My limit is {price}.",
            f"I won't give more than {price}.",
            f"Let's stick to {price}.",
            f"How about {price}?",
            f"I'll give {price}."
        ]
        return pick_varied("too_expensive_warning_line", lines) + " " + pick_varied("too_expensive_warning_transition", transitions)

    if proposal_type == "hold_position":
        if personality == "Aggressive Trader":
            lines = [
                "I won't move.",
                "Too high.",
                "Do better.",
                "No.",
                "I'm staying here."
            ]
            transitions = [
                f"{price}.",
                f"I stay at {price}.",
                "Give me a reason to move.",
                "That's it.",
                f"Take {price} or leave it."
            ]
        elif personality == "Polite Merchant":
            lines = [
                "I can't move yet.",
                "Still feels high.",
                "Could you do better?",
                "I'll hold here.",
                "Let's stay here."
            ]
            transitions = [
                f"I'll stick to {price}.",
                f"Maybe {price}?",
                f"How about {price}?",
                f"I can only do {price}.",
                f"Let's work from {price}."
            ]
        else:
            lines = [
                "I shouldn't move yet.",
                "Still feels a bit high.",
                "I don't know...",
                "I'm not ready to move.",
                "I'll hold here."
            ]
            transitions = [
                f"I'll stay at {price}.",
                f"Maybe {price}?",
                f"I think {price} is fair.",
                f"Let's try {price}.",
                f"I offer {price}."
            ]
        return pick_varied("hold_position_line", lines) + " " + pick_varied("hold_position_transition", transitions)

    if proposal_type == "quantity_too_small":
        lines = [
            "Not for that little.",
            "That's too high for so little.",
            "I need more for that price.",
            "Too small an amount.",
            "That hardly justifies the cost."
        ]
        transitions = [
            f"I'll give {price}.",
            f"For that, I offer {price}.",
            "Give me more or lower the price.",
            f"I can only do {price}.",
            f"Take {price} instead."
        ]
        return pick_varied("quantity_too_small_line", lines) + " " + pick_varied("quantity_too_small_transition", transitions)

    if proposal_type == "reject_small_quantity":
        lines = [
            "I can't agree to that.",
            "Not for that amount.",
            "Too little for that price.",
            "I need a better deal.",
            "That doesn't seem right."
        ]
        transitions = [
            f"I'll stay at {price}.",
            "We need a fairer trade.",
            f"I can offer {price}.",
            f"Sticking with {price}.",
            "Make it fair."
        ]
        return pick_varied("reject_small_quantity_line", lines) + " " + pick_varied("reject_small_quantity_transition", transitions)

    if proposal_type == "quantity_answer":
        quantity_text = format_quantity_grams(context.get("current_quantity_grams"))
        if personality == "Aggressive Trader":
            lines = [
                f"For {price}? Then make it {quantity_text}.",
                f"At {price}, I'd expect {quantity_text}.",
                f"{quantity_text} for {price}. That's it."
            ]
        elif personality == "Polite Merchant":
            lines = [
                f"If you mean {price}, I could do {quantity_text}.",
                f"For {price} varahas, let's say {quantity_text}.",
                f"At {price}, I think {quantity_text} is fair."
            ]
        else:
            lines = [
                f"For {price}? I suppose {quantity_text}.",
                f"At {price}, I'd need {quantity_text}.",
                f"Maybe {quantity_text} for {price}."
            ]
        return pick_varied(f"quantity_answer:{personality}", lines)

    if proposal_type in ["quantity_change", "bundle_offer"]:
        quantity_lines = [
            "Oh, for that much?",
            "That changes things.",
            "That's different.",
            "Ah, a new amount.",
            "That changes my mind."
        ]

        if proposal_type == "bundle_offer":
            quantity_lines = [
                "For both?",
                "Together? Alright.",
                "That changes it.",
                "Ah, a bundle.",
                "That mix is different."
            ]
        if personality == "Aggressive Trader":
            quantity_transitions = [
                f"For {item_name}, I'll give {price}.",
                f"That gets {price}.",
                f"My price is {price}.",
                f"Take {price} then.",
                f"For that, {price}."
            ]
        elif personality == "Polite Merchant":
            quantity_transitions = [
                f"I could do {price} for {item_name}.",
                f"For that much, maybe {price}.",
                f"How does {price} sound?",
                f"I'll offer {price}.",
                f"Perhaps {price}?"
            ]
        else:
            quantity_transitions = [
                f"Hmm, maybe {price}?",
                f"I think I could do {price}.",
                f"For that, {price} seems fair.",
                f"I'd offer {price}.",
                f"Let's say {price}."
            ]

        return pick_varied(f"quantity_change_line:{proposal_type}", quantity_lines) + " " + pick_varied(
            f"quantity_change_transition:{proposal_type}", quantity_transitions
        )

    if proposal_type in ["quantity_reduce", "bundle_adjust", "quantity_expect_more", "bundle_expect_more"]:
        seller_price = context.get("seller_price")
        counter_bundle_label = context.get("counter_bundle_label", item_name)

        if proposal_type in ["quantity_reduce", "bundle_adjust"]:
            lines = [
                f"Too much. I'll take {counter_bundle_label} for {price}.",
                f"{seller_price} is high. How about {price} for {counter_bundle_label}?",
                f"I can't pay that. I'd do {price} for {counter_bundle_label}.",
                f"For {price}, I'll take {counter_bundle_label} instead.",
                f"Let's scale down. {price} for {counter_bundle_label}."
            ]
        else:
            lines = [
                f"At that price? I'd expect more like {counter_bundle_label}.",
                f"For {seller_price}, give me {counter_bundle_label}.",
                f"That's high. Throw in {counter_bundle_label}.",
                f"Not enough for that price. Give me {counter_bundle_label}.",
                f"I need {counter_bundle_label} if you want {seller_price}."
            ]

        return pick_varied(f"quantity_counter:{proposal_type}", lines)

    if proposal_type == "quantity_too_costly":
        lines = [
            "Not for that quantity.",
            "That's way too high.",
            "Too costly for that much.",
            "I can't pay that for so little.",
            "That doesn't seem right.",
            "For that amount? No."
        ]
        if personality == "Aggressive Trader":
            transitions = [
                f"I offer {price}.",
                "Give me a better price.",
                f"My price is {price}.",
                f"Take {price}.",
                f"I'll give {price}."
            ]
        elif personality == "Polite Merchant":
            transitions = [
                f"My offer is {price}.",
                "I need a better deal.",
                f"At that quantity, just {price}.",
                f"How about {price}?",
                f"I could do {price}."
            ]
        else:
            transitions = [
                f"Hmm... I'll offer {price}.",
                "I think I need a better price.",
                f"Maybe {price}?",
                f"I can only do {price}.",
                f"Let's say {price}."
            ]
        return pick_varied("quantity_too_costly_line", lines) + " " + pick_varied("quantity_too_costly_transition", transitions)

    # NORMAL OFFER (HIGH VARIETY)
    base_lines, prefix = offer_lines_for_personality(personality, price, negotiation_stage, frustration, trust)
    chosen_line = pick_varied(f"normal_offer:{personality}:{negotiation_stage}:{'tense' if frustration >= 0.8 else 'calm'}", base_lines)
    line = merge_prefix_and_line(prefix, chosen_line)
    if reason_prefix:
        return f"{reason_prefix} {line}"
    return line
