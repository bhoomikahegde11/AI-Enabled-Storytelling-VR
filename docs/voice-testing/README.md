Offline TTS Voice Experiments for AI Enabled Historical Storytelling VR

Purpose

This folder contains offline TTS voice experiments for AI Enabled Historical Storytelling VR.

These files are experiments only.
They are not integrated into Unity yet.
The goal is to compare offline Quest-compatible voices before any engine integration work begins.

How To Review

Reviewers should listen to the sample WAV files and comment on:

- Accent quality
- Gender impression
- Naturalness
- Historical NPC suitability
- Whether the voice feels right for long-form NPC dialogue

Current Focus

- Indian English accent quality
- Male and female voice coverage
- Historical suitability for Hampi bazaar NPCs

Target NPCs

- Abdul Rahman
- Francisco de Almeida
- Lakshmi Amma
- Chinappa Naik
- Siddharth Chetti
- Father Penteado

Current Findings

- The Indian-accent Piper voices tested so far sound female-coded or female-leaning.
- We are still looking for a strong Indian English male offline voice.
- Non-Indian fallback voices are included for comparison only.

Folder Guide

- `models/piper/`: Piper model files used for testing
- `models/sherpa/`: placeholder for future Sherpa-ONNX model tests
- `samples/piper/`: generated Piper WAV samples
- `samples/sherpa/`: placeholder for future Sherpa-ONNX sample outputs
- `notes/`: uncertain or rejected voice notes, plus temporary sorting items

Important

- Do not move anything from this folder into `unity/StorytellingVR/Assets`.
- Voice choice should be finalized before Unity integration work starts.
- Sherpa testing is the next comparison step after this Piper review pass.
