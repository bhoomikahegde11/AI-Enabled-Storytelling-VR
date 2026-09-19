# Persistent Player State & Story Memory

This directory is reserved for managing player progression, reputation, and state across different storytelling levels. 

---

## 💾 Directory Structure

```
memory/
  ├── README.md             # This guide
  └── sessions/             # Active persistent state json files per player
        └── [session_id].json
```

---

## 📈 Cross-Level Progression Design

For a true multi-level storytelling experience, a player's behavior in one level should impact how characters react to them in subsequent levels.

### Example Schema (`memory/sessions/[session_id].json`)
```json
{
  "player_name": "Merchant Traveler",
  "current_level": 2,
  "level_history": {
    "level1_market": {
      "completed": true,
      "final_trust": 0.85,
      "total_profit_varahas": 240,
      "reputation_archetype": "Fair Trader",
      "hostility_level": "None"
    }
  },
  "global_metrics": {
    "reputation": 85,
    "total_varahas": 240,
    "completed_levels": ["level1_market"]
  }
}
```

---

## ⛓️ Level Integration Points

### 1. Level 1 Marketplace (Negotiation)
* **Output**: Writes final deal results, merchant `trust`, and total `profit` to the session file upon the final transaction.
* **Impact**: If the player was highly respectful and built trust, they receive a `"Fair Trader"` reputation flag.

### 2. Level 2 Craftsmanship (Guilds)
* **Input**: Reads the Level 1 reputation flag.
* **Dynamic Reaction**: When entering the Weaver's Guild, the Guild Master reads the `"Fair Trader"` flag and greets the player warmly:
  > *"Welcome, friend. Word travels fast in the bazaar. They say you deal fairly with our spice merchants. Let us discuss our silk supplies."*
* **Alternate Reaction (Low Trust / Greed)**: If the player was aggressive or scammy:
  > *"Ah, you are the one who tried to squeeze our spice sellers dry. The Weaver's Guild has no room for greedy hagglers. Our prices on fine dyes are fixed."*

This allows for a truly cohesive **AI-powered storytelling experience** where player agency has meaningful, compounding consequences!
