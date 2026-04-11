# AI NPC Negotiation System – Project Instructions

## 🧠 Project Overview

This is a simulation system for AI-powered NPC negotiation in a VR marketplace set in the Vijayanagara Empire (1500s).

This is NOT a chatbot.

The system simulates realistic bargaining between:
- Player (seller)
- AI NPC (buyer)

---

## ⚙️ Core Architecture (DO NOT BREAK)

### 1. Negotiation Engine (Python - deterministic)
- Controls ALL price logic
- Handles:
  - current_offer
  - max_price
  - market_price
  - patience
  - desperation
- Decides:
  - OFFER
  - ACCEPT
  - WALK_AWAY
  - ASK_ITEM

🚫 NEVER allow LLM to control pricing

---

### 2. Intent Classifier
- Hybrid system:
  - Rules for clear intents (PRICE, ACCEPT, etc.)
  - LLM for semantic classification
- Returns:
  - intent
  - tone
  - persuasion

---

### 3. Dialogue Generator
- Generates natural responses
- Uses templates (NOT full LLM generation)
- Must NOT affect logic

---

## 📁 File Structure

/engine/
  negotiation_engine.py

/llm/
  intent_classifier.py
  dialogue_generator.py

/models/
  model.gguf

/core/
  controller.py

main.py

---

## 🔒 Strict Rules (CRITICAL)

1. Buyer prices must ALWAYS increase (monotonic)
2. Buyer must NEVER:
   - generate random prices
   - accept invalid deals
3. Buyer must WALK AWAY when:
   - seller is irrelevant repeatedly
   - seller gives unrealistic prices
   - negotiation is stuck
4. No infinite loops
5. No decimal prices

---

## 🧠 Behavior Goals

The NPC must feel:
- Human-like
- Adaptive
- Emotionally responsive
- Context-aware

---

## 🔥 Current Systems Implemented

- Deterministic negotiation logic
- Personality system
- Persuasion system
- Suspicion system
- Repetition detection
- Stuck negotiation detection

---

## 🚫 What NOT to Do

- Do NOT convert system into chatbot
- Do NOT move logic into LLM
- Do NOT rewrite architecture
- Do NOT modify unrelated files

---

## ✅ How to Work on This Project

When making changes:

1. Only modify relevant files
2. Keep logic deterministic
3. Preserve architecture separation
4. Keep changes minimal and clean

---

## 🎯 Definition of Done

A change is complete when:
- Code runs without errors
- Behavior improves (not degrades)
- No core rules are broken
- Output feels more realistic

---

## 🧠 Important Notes

- This is a simulation, NOT a chatbot
- LLM is used ONLY for:
  - understanding input
  - improving dialogue
- All decisions must remain rule-based

---

## 🚀 Future Scope

- Emotion system
- Memory system
- Multi-NPC interactions
- Unity integration