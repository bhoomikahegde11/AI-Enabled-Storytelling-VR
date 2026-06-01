# AI Storytelling VR Architecture: 1500s Vijayanagara Empire

This document outlines the modular architecture of the **AI-Enabled Storytelling VR** project. The platform is designed to take the player through a series of immersive historical simulations where they learn specific daily life skills of the 1500s Vijayanagara Empire.

---

## 🏗️ Architectural Overview

The backend uses a decoupled, level-agnostic design that keeps core game session logic completely separate from the specific rules, dialogues, and historical contexts of individual levels.

```
                  ┌─────────────────────────────────────────┐
                  │               Unity VR Client           │
                  └────────────────────┬────────────────────┘
                                       │ POST /start, /step
                                       ▼
                  ┌─────────────────────────────────────────┐
                  │             FastAPI Backend             │
                  │               (api.py)                  │
                  └────────────────────┬────────────────────┘
                                       │ delegates
                                       ▼
                  ┌─────────────────────────────────────────┐
                  │               NPCSession                │
                  │             (interface.py)              │
                  └────────────────────┬────────────────────┘
                                       │ instantiates
                                       ▼
                  ┌─────────────────────────────────────────┐
                  │             General Controller          │
                  │            (core/controller.py)         │
                  └───────────┬─────────────────┬───────────┘
                              │                 │
              injects Level 1 │                 │ injects Level 2
              components      ▼                 ▼ components (Future)
        ┌───────────────────────────┐     ┌───────────────────────────┐
        │  levels/level1_market/    │     │   levels/level2_crafts/   │
        │  - buyer_model.py         │     │  - guild_manager.py       │
        │  - negotiation_engine.py  │     │  - crafting_rules.py      │
        │  - intent_classifier.py   │     │  - intent_classifier.py   │
        │  - dialogue_generator.py  │     │  - dialogue_generator.py  │
        └───────────────────────────┘     └───────────────────────────┘
```

---

## 🛠️ Restructured Backend Components

### 1. General Runner (`core/`)
* **`controller.py`**: The general orchestrator. It manages the interaction loop between player input and the active level's simulation engine. It does **not** contain any hardcoded knowledge of spices, weights, craft tools, or royal ranks. Instead, it accepts functional callbacks (`classify_intent_fn`, `extract_quantity_info_fn`, `extract_price_fn`, `dialogue_fn`) upon initialization.
* **`models.py`**: Defines the shared, abstract dataclasses (`PlayerAction`, `EngineDecision`) that flow between the general controller and the active level.

### 2. Levels Directory (`levels/`)
This is where the gameplay rules and historical simulations reside. Each level is fully encapsulated inside its own folder:
* **`level1_market/` (Active)**: 
  * Implements the spice bazaar negotiation engine.
  * Contains the Level 1 local intent classifier (rules and LLM prompts set in a 1500s spice bazaar).
  * Houses the Level 1 dialogue generator containing hundreds of tailored buyer responses matching personalities (*Polite Merchant*, *Aggressive Trader*, *Cautious Buyer*).

---

## 🚀 How to Add a New Level (e.g., Level 2: Craftsmanship)

Adding a new level is simple and will not break Level 1 or the Unity client:

1. **Create the Folder**:
   Create a new directory under levels: `backend/npc_engine/levels/level2_crafts/`.

2. **Implement the Level Engine**:
   Write a state manager or ruleset engine (e.g., `crafting_engine.py`) that handles crafting steps, tool selections, workshop finances, or guild master patience. It must return a standard `EngineDecision` dataclass.

3. **Write the Level Intent Classifier**:
   Write a level-specific `intent_classifier.py` that parses player verbal/text craft suggestions (e.g., clay quality, heating temperature, decorative patterns) instead of spice weights and prices.

4. **Write the Level Dialogue Generator**:
   Write a `dialogue_generator.py` containing templated responses matching characters like guild masters, clay suppliers, or buyers of fine Vijayanagara pottery.

5. **Register in `interface.py`**:
   Expose an endpoint or session setting that dynamically loads `levels.level2_crafts` instead of `levels.level1_market` based on the level requested by Unity:
   ```python
   # Sample future dynamic Level loader
   if level_id == 2:
       self.engine = CraftingEngine()
       self.controller = Controller(
           self.engine,
           classify_intent_fn=level2_classify_intent,
           extract_quantity_info_fn=level2_extract_tool,
           extract_price_fn=level2_extract_cost,
           dialogue_fn=level2_generate_dialogue
       )
   ```

---

## 📁 Shared Knowledge & Persistent Memory (Root Folders)

* **`knowledge/`**: Contains structured JSON fact-sheets and RAG context about the empire. NPCs can leverage this data to sound historically accurate.
* **`memory/`**: Houses persistent session progress. A player's performance in Level 1 (e.g., high merchant trust) can save a reputational token in `memory/sessions/` that characters in Level 2 or Level 3 read and react to!
