import re
import csv
import json
from collections import defaultdict, Counter
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
CHAR_DIR = REPO_ROOT / "unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters"
OUT_DIR = Path(__file__).resolve().parent
PROVIDER = REPO_ROOT / "unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/DialogueTableResponseProvider.cs"
SCENARIOS = REPO_ROOT / "unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/DialogueScenario.cs"

CHARACTER_FILES = [
    ("Lakshmi Amma", "lakshmi_amma", "LakshmiAmmaDialogue.cs", "LAK"),
    ("Chinnamma Naik", "chinnamma_naik", "ChinappaNaikDialogue.cs", "CHI"),
    ("Saraswati Chetti", "saraswati_chetti", "SiddharthChettiDialogue.cs", "SAR"),
    ("Abdul Rahman", "abdul_rahman", "AbdulRahmanDialogue.cs", "ABD"),
    ("Francisco de Almeida", "francisco_de_almeida", "FranciscoDialogue.cs", "FRA"),
    ("Father Penteado", "father_penteado", "FatherPenteadoDialogue.cs", "FAT"),
]

SCENARIO_GATING_CONDITIONS = {
    "CustomerGreeting": "First-time greeting for this NPC in session (GetGreeting: isRepeatCustomer=false)",
    "RepeatCustomerGreeting": "Repeat visit by this NPC in session (GetGreeting: isRepeatCustomer=true)",
    "AskWhatBuyerWants": "Player intent ITEM_QUERY (or tradeOpeningQuery without price/reason)",
    "AskSellerPrice": "Player intent PRICE_QUERY when buyer offer not yet introduced (!trade.priceIntroduced)",
    "AskBuyerBudget": "Player intent QUERY_BUYER_BUDGET or PRICE_QUERY after price introduced (or ITEM_QUERY asking current offer)",
    "AskQuantity": "Player intent QUANTITY_QUERY",
    "QuantityChanged": "Player intent QUANTITY_CHANGE (or valid quantity change committed during turn)",
    "SellerPriceTooHigh": "Counter-offer when seller price >= 1.25x buyer maxPrice or >= 1.35x updatedOffer",
    "SellerPriceSlightlyHigh": "Counter-offer when seller price is within <= 12% of updatedOffer",
    "SellerPriceBelowExpected": "Nonterminal seller price below the standing buyer offer; accepted outcomes use an agreement scenario",
    "BuyerCounterFirst": "Standard counter-offer on RoundBucket.First (round <= 1)",
    "BuyerCounterMiddle": "Standard counter-offer on RoundBucket.Middle (rounds 2-4)",
    "BuyerCounterFinal": "Standard counter-offer on RoundBucket.Final (round >= 5)",
    "BuyerHoldsFirm": "Buyer holds current offer without increasing (updatedOffer == previousOffer) or CONTINUE intent",
    "SellerPriceAccepted": "Buyer accepts seller's stated price (isAccepted=true, intent != ACCEPT)",
    "PlayerAcceptedDeal": "Player accepts buyer's standing offer (isAccepted=true, intent == ACCEPT)",
    "PlayerRejectedBuyerOffer": "Player rejects buyer's offer (intent == REJECT) and buyer does not raise offer",
    "OfferReason": "Player asks reason for buyer's offer (input.asksReason=true)",
    "AskName": "Social/general dialogue mentioning 'name' or 'who are you'",
    "AskOrigin": "Social/general dialogue mentioning 'where', 'from', or 'origin'",
    "WeatherQuestion": "Social/general dialogue mentioning 'weather', 'rain', 'sun', or 'heat'",
    "HistoryKing": "Social/general dialogue mentioning 'king' or 'krishnadevaraya'",
    "HistoryTemple": "Social/general dialogue mentioning 'temple' or 'virupaksha'",
    "HistoryQuestion": "Social/general dialogue mentioning 'history', 'hampi', 'vijayanagara', 'bazaar', or 'city'",
    "SocialGreeting": "Player intent GREETING, SOCIAL, or GENERAL_DIALOGUE (without specific name/origin/weather/history keyword)",
    "OffTopic": "Player intent OFF_TOPIC (out-of-world count < 4)",
    "HostileWarning": "Player intent HOSTILE (before walk-away threshold)",
    "UnclearSpeech": "Clarification required (needsClarification=true, CLARIFICATION, CONFUSED, or invalid terms); wraps {ruleReply}",
    "Unknown": "Fallback scenario when intent is UNKNOWN; outputs {ruleReply} directly",
    "TransactionSuccess": "Accepted physical order delivered and settlement claimed (GetTransactionSuccess)",
    "TransactionFailure": "Buyer walks away (walkedAway=true, patience/frustration/round limit/dismiss/hard reject) or silence timeout stage 2",
    "TimePressure": "Intentionally inactive; the existing silence path uses IdleReminderFirst/Second and terminal TransactionFailure",
    "IdleReminderFirst": "First silence/idle reminder while waiting for player reply (GetIdleReminder stage 1)",
    "IdleReminderSecond": "Second silence/idle reminder while waiting for player reply before timeout (GetIdleReminder stage 2, terminalTimeout=false)",
}

def extract_string_literals(content, start_idx, end_idx, offset_to_line):
    templates = []
    j = start_idx
    while j < end_idx:
        if content[j] == '"':
            str_start = j
            j += 1
            chars = []
            while j < end_idx:
                c = content[j]
                if c == '\\' and j + 1 < end_idx:
                    nxt = content[j+1]
                    if nxt == '"':
                        chars.append('"')
                    elif nxt == '\\':
                        chars.append('\\')
                    elif nxt == 'n':
                        chars.append('\n')
                    else:
                        chars.append('\\' + nxt)
                    j += 2
                    continue
                elif c == '"':
                    j += 1
                    break
                else:
                    chars.append(c)
                    j += 1
            str_val = "".join(chars)
            str_line = offset_to_line(str_start)
            templates.append((str_val, str_line))
        else:
            j += 1
    return templates

def parse_character_file(npc_name, char_id, filename, prefix):
    filepath = CHAR_DIR / filename
    rel_path = f"unity/StorytellingVR/Assets/_Project/Scripts/Level1/Core/Dialogues/Characters/{filename}"
    with open(filepath, "r", encoding="utf-8") as f:
        lines = f.readlines()

    content = "".join(lines)
    m_set = re.search(r'new\s+CharacterDialogueSet\s*\(\s*"([^"]+)"\s*,\s*"([^"]+)"\s*,\s*NpcPersonalityBucket\.(\w+)', content)
    if not m_set or m_set.group(1) != char_id or m_set.group(2) != npc_name:
        raise ValueError(f"Character identity does not match registry inventory: {filepath}")
    code_display_name = m_set.group(2)
    default_personality = m_set.group(3)

    list_start = content.find("new List<DialogueLine>")
    list_end = content.rfind("});")
    if list_start < 0 or list_end <= list_start:
        raise ValueError(f"Could not find authored dialogue list: {filepath}")

    line_starts = [0]
    for line in lines:
        line_starts.append(line_starts[-1] + len(line))

    def offset_to_line(offset):
        for i in range(len(line_starts) - 1):
            if line_starts[i] <= offset < line_starts[i+1]:
                return i + 1
        return len(lines)

    entry_pattern = re.compile(r'\b(Line|Mood|Low|new\s+DialogueLine)\s*\(', re.MULTILINE)

    entries = []
    pos = list_start
    while True:
        m = entry_pattern.search(content, pos, list_end)
        if not m:
            break
        call_type = re.sub(r'\s+', ' ', m.group(1).strip())
        call_start = m.start()
        paren_depth = 1
        i = m.end()
        in_str = False
        escape = False
        while i < list_end and paren_depth > 0:
            ch = content[i]
            if in_str:
                if escape:
                    escape = False
                elif ch == '\\':
                    escape = True
                elif ch == '"':
                    in_str = False
            else:
                if ch == '"':
                    in_str = True
                elif ch == '(':
                    paren_depth += 1
                elif ch == ')':
                    paren_depth -= 1
            i += 1
        call_end = i
        call_body = content[m.end():call_end-1]
        entry_start_line = offset_to_line(call_start)
        entry_end_line = offset_to_line(call_end - 1)

        m_scen = re.search(r'DialogueScenario\.(\w+)', call_body)
        scenario = m_scen.group(1) if m_scen else "UNKNOWN"

        mood_bucket = "Any"
        patience_bucket = "Any"
        reputation_bucket = None
        desperation_bucket = None
        round_bucket = None

        if call_type == "Mood":
            m_mood = re.search(r'NpcPersonalityBucket\.(\w+)', call_body)
            if m_mood:
                mood_bucket = m_mood.group(1)
        elif call_type == "Low":
            patience_bucket = "Low"
        elif call_type == "new DialogueLine":
            m_mood = re.search(r'personalityBucket\s*:\s*NpcPersonalityBucket\.(\w+)', call_body)
            if m_mood:
                mood_bucket = m_mood.group(1)
            m_pat = re.search(r'patienceBucket\s*:\s*NpcPatienceBucket\.(\w+)', call_body)
            if m_pat:
                patience_bucket = m_pat.group(1)
            m_rep = re.search(r'reputationBucket\s*:\s*PlayerReputationBucket\.(\w+)', call_body)
            if m_rep:
                reputation_bucket = m_rep.group(1)
            m_desp = re.search(r'desperationBucket\s*:\s*NpcDesperationBucket\.(\w+)', call_body)
            if m_desp:
                desperation_bucket = m_desp.group(1)
            m_rnd = re.search(r'roundBucket\s*:\s*RoundBucket\.(\w+)', call_body)
            if m_rnd:
                round_bucket = m_rnd.group(1)

        # Extract all string literals in call_body
        all_strs = extract_string_literals(content, m.end(), call_end - 1, offset_to_line)
        if call_type == "new DialogueLine":
            if not all_strs or all_strs[-1][0] != char_id:
                raise ValueError(f"Unexpected explicit DialogueLine format at {filepath}:{entry_start_line}")
            templates = all_strs[:-1]
        else:
            templates = all_strs
        if scenario == "UNKNOWN" or not templates:
            raise ValueError(f"Missing scenario or templates at {filepath}:{entry_start_line}")

        entries.append({
            "npc_name": npc_name,
            "code_display_name": code_display_name,
            "char_id": char_id,
            "default_personality": default_personality,
            "call_type": call_type,
            "scenario": scenario,
            "mood_bucket": mood_bucket,
            "patience_bucket": patience_bucket,
            "reputation_bucket": reputation_bucket,
            "desperation_bucket": desperation_bucket,
            "round_bucket": round_bucket,
            "entry_start_line": entry_start_line,
            "entry_end_line": entry_end_line,
            "templates": templates,
            "rel_path": rel_path,
            "class_name": filename.replace(".cs", ""),
            "prefix": prefix
        })

        pos = call_end

    return entries

def main():
    known_scenarios = set(re.findall(r'^\s*(\w+),?\s*$', SCENARIOS.read_text(encoding="utf-8"), re.MULTILINE))
    supported_placeholders = set(re.findall(r'\.Replace\("\{(\w+)\}"', PROVIDER.read_text(encoding="utf-8")))
    if len(known_scenarios) != 34 or not supported_placeholders:
        raise ValueError("Scenario or placeholder source format changed; review the exporter before publishing.")
    all_entries = []
    for npc_name, char_id, filename, prefix in CHARACTER_FILES:
        entries = parse_character_file(npc_name, char_id, filename, prefix)
        all_entries.extend(entries)

    csv_rows = []
    placeholder_pattern = re.compile(r'\{([^{}]+)\}')
    validation_errors = []

    npc_seq = Counter()
    for entry_idx, e in enumerate(all_entries, 1):
        for tmpl_idx, (tmpl_text, tmpl_line) in enumerate(e["templates"], 1):
            npc_seq[e["prefix"]] += 1
            dialogue_id = f"L1_{e['prefix']}_{npc_seq[e['prefix']]:03d}"
            if e["scenario"] not in known_scenarios:
                validation_errors.append(f"{dialogue_id}: unknown scenario {e['scenario']}")
            if not tmpl_text.strip():
                validation_errors.append(f"{dialogue_id}: empty template")

            found_vars = placeholder_pattern.findall(tmpl_text)
            unknown_vars = set(found_vars) - supported_placeholders
            if unknown_vars:
                validation_errors.append(f"{dialogue_id}: unsupported placeholders {sorted(unknown_vars)}")
            unique_vars = []
            for v in found_vars:
                tok = f"{{{v}}}"
                if tok not in unique_vars:
                    unique_vars.append(tok)
            dyn_vars_str = "; ".join(unique_vars) if unique_vars else "None"

            cond_parts = [f"characterId={e['char_id']}", f"defaultPersonality={e['default_personality']}"]
            specificity = 0
            if e["patience_bucket"] != "Any":
                cond_parts.append(f"patienceBucket={e['patience_bucket']} (patience <= 2, specificity >= 2)")
                specificity += 2
            if e["mood_bucket"] != "Any":
                cond_parts.append(f"personalityBucket={e['mood_bucket']} (specificity +1)")
                specificity += 1
            if e["reputation_bucket"]:
                cond_parts.append(f"reputationBucket={e['reputation_bucket']} (specificity +1)")
                specificity += 1
            if e["desperation_bucket"]:
                cond_parts.append(f"desperationBucket={e['desperation_bucket']} (specificity +1)")
                specificity += 1
            if e["round_bucket"]:
                cond_parts.append(f"roundBucket={e['round_bucket']} (specificity +1)")
                specificity += 1
            cond_parts.append(f"specificity={specificity}")
            scen_gate = SCENARIO_GATING_CONDITIONS.get(e["scenario"], "")
            if scen_gate:
                cond_parts.append(f"Trigger: {scen_gate}")

            conditions_str = " | ".join(cond_parts)
            source_loc = f"{e['class_name']}.Create ({e['call_type']} entry L{e['entry_start_line']}-L{e['entry_end_line']}, template L{tmpl_line})"

            csv_rows.append({
                "Dialogue_ID": dialogue_id,
                "NPC": e["npc_name"],
                "Scenario": e["scenario"],
                "Dialogue_Text": tmpl_text,
                "Mood": e["mood_bucket"],
                "Patience": e["patience_bucket"],
                "Conditions": conditions_str,
                "Dynamic_Variables": dyn_vars_str,
                "Source_File": e["rel_path"],
                "Source_Location": source_loc,
                "Machine_Validation": "PASS",
                "Reachability": "INTENTIONALLY_INACTIVE" if e["scenario"] == "TimePressure" else "CONDITIONALLY_REACHABLE",
                "Review_Status": "PENDING_MANUAL_VERIFICATION",
                "Review_Notes": "No runtime route currently selects TimePressure." if e["scenario"] == "TimePressure" else "Editorial and in-game delivery approval pending."
            })

    covered = {row["Scenario"] for row in csv_rows}
    if covered != known_scenarios:
        validation_errors.append(f"Scenario coverage mismatch: missing={sorted(known_scenarios - covered)}, extra={sorted(covered - known_scenarios)}")
    if len({row["Dialogue_ID"] for row in csv_rows}) != len(csv_rows):
        validation_errors.append("Duplicate Dialogue_ID")
    if validation_errors:
        raise ValueError("Dialogue inventory validation failed:\n" + "\n".join(validation_errors))

    csv_path = OUT_DIR / "Level1_AllNPC_Dialogues.csv"
    fieldnames = [
        "Dialogue_ID", "NPC", "Scenario", "Dialogue_Text", "Mood", "Patience",
        "Conditions", "Dynamic_Variables", "Source_File", "Source_Location",
        "Machine_Validation", "Reachability",
        "Review_Status", "Review_Notes"
    ]
    with open(csv_path, "w", encoding="utf-8", newline="") as f:
        writer = csv.DictWriter(f, fieldnames=fieldnames, quoting=csv.QUOTE_MINIMAL)
        writer.writeheader()
        writer.writerows(csv_rows)

    print(f"Wrote {len(csv_rows)} rows to {csv_path}")
    for npc_name, _, _, prefix in CHARACTER_FILES:
        print(f"  {npc_name}: {npc_seq[prefix]} templates")

    scen_counts = Counter()
    npc_scen_counts = defaultdict(Counter)
    var_counts = Counter()
    tmpl_with_vars = 0
    tmpl_without_vars = 0
    exact_text_map = defaultdict(list)
    mood_restricted_counts = Counter()
    patience_restricted_counts = Counter()

    for row in csv_rows:
        scen_counts[row["Scenario"]] += 1
        npc_scen_counts[row["Scenario"]][row["NPC"]] += 1
        exact_text_map[row["Dialogue_Text"]].append((row["Dialogue_ID"], row["NPC"], row["Scenario"], row["Mood"], row["Patience"]))
        if row["Mood"] != "Any":
            mood_restricted_counts[f"{row['NPC']} | {row['Scenario']} | Mood={row['Mood']}"] += 1
        if row["Patience"] != "Any":
            patience_restricted_counts[f"{row['NPC']} | {row['Scenario']} | Patience={row['Patience']}"] += 1
        if row["Dynamic_Variables"] == "None":
            tmpl_without_vars += 1
        else:
            tmpl_with_vars += 1
            for v in row["Dynamic_Variables"].split("; "):
                var_counts[v] += 1

    duplicates = {k: v for k, v in exact_text_map.items() if len(v) > 1}

    stats = {
        "total_rows": len(csv_rows),
        "total_dialogue_lines": len(all_entries),
        "npc_template_counts": {name: npc_seq[prefix] for name, _, _, prefix in CHARACTER_FILES},
        "scenario_count": len(covered),
        "intentionally_inactive_time_pressure": scen_counts["TimePressure"],
        "machine_validation_failures": validation_errors,
        "editorial_status": "PENDING_MANUAL_VERIFICATION",
        "tmpl_with_vars": tmpl_with_vars,
        "tmpl_without_vars": tmpl_without_vars,
        "var_counts": dict(var_counts.most_common()),
        "scen_counts": dict(scen_counts),
        "npc_scen_counts": {k: dict(v) for k, v in npc_scen_counts.items()},
        "duplicates": duplicates,
        "mood_restricted_counts": dict(mood_restricted_counts),
        "patience_restricted_counts": dict(patience_restricted_counts),
    }

    stats_path = OUT_DIR / "audit_stats.json"
    with open(stats_path, "w", encoding="utf-8") as f:
        json.dump(stats, f, indent=2)
    print(f"Wrote stats to {stats_path}")

if __name__ == "__main__":
    main()
