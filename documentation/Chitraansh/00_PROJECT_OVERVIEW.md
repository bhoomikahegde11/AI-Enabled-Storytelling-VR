# Project Overview: AI-Enabled Storytelling VR

This project implements an immersive, interactive historical storytelling simulation set in the peak era of the **Vijayanagara Empire (1500s CE)**. By combining virtual reality (VR) with local, low-latency AI agents, it provides players with a first-person roleplaying experience as a marketplace merchant in the vibrant bazaar of Hampi.

## 🎯 Project Goals

1. **Autonomous AI NPCs in VR**: Create fully voiced historical characters (Persian traders, Portuguese trade agents, local wholesales buyers) with unique personalities, motivations, and economic constraints.
2. **Dynamic Speech-to-Speech Loop**: Enable hands-free voice-based interaction combining Local Automatic Speech Recognition (STT), natural conversation understanding, and Text-to-Speech (TTS) synthesis.
3. **Vijayanagara Marketplace Bargaining**: Model a realistic historical trade economy. Players negotiate weight (traditional metrics like *seers*, *veesai*, *palams*) and prices (*varahas*) for highly coveted Malabar spices (pepper, clove, cinnamon, cardamom).
4. **Historical & Cultural Immersion**: Authenticate dialogues using a Retrieval-Augmented Generation (RAG) system with historical records of Hampi, ruling dynasties, trading caravans, and global spice routes.

## 🏛️ Context & Historical Authenticity

The Vijayanagara Empire (centered around the capital Hampi in modern-day Karnataka, India) was one of the wealthiest and most powerful empires in medieval Asia. Serving as a crucial hub for international spice and gem trading, the Hampi marketplace was frequented by global merchants. The system simulates these historical cross-cultural negotiations:
- **Abdul Rahman**: A wealthy Persian merchant buying pepper for merchant fleets.
- **Francisco de Almeida**: A Portuguese crown representative bargaining for cinnamon.
- **Lakshmi Amma**: A local experienced buyer negotiating practical market-rate spice purchases.

## 🎙️ Speech Integration

To achieve maximum presence in Virtual Reality, keyboard input is replaced by natural spoken English:
- **Whisper STT**: Real-time voice transcription using `faster-whisper` (CUDA accelerated) to transcribe the player's speech.
- **Vosk Offline Quest STT**: Standalone Android speech recognition path for Quest builds, loading the Vosk model locally without a backend dependency.
- **Fuzzy Speech Corrections**: Preprocessing homophones and common STT errors before intent classification to ensure colloquial voice patterns are correctly mapped.
- **Piper TTS ONNX**: Real-time local speech synthesis, returning responses in low latency to prevent breaking the flow of immersive gameplay.

## 🗣️ Standalone VR Dialogue Layer

Recent Level 1 standalone VR work adds a character-driven dialogue table layer on top of the negotiation rules. The bargaining engine still decides the actual trade action (accept, counter, reject, ask quantity, ask price), but NPC delivery is now selected from reusable response templates with character-specific greetings, mood-sensitive phrasing, and placeholder substitution for prices, quantities, spices, and buyer identity.
