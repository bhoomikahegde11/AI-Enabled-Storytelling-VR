Sherpa Runtime Voice Assets

These files are runtime voice assets for Level 1 NPC TTS experiments.

Current use:
- Used by SherpaEditorTtsProvider in the Unity Editor
- Loaded from Assets/StreamingAssets/Sherpa/voices/

Future intent:
- These same assets are intended to support a later Quest/Android Sherpa runtime path
- This folder is deployment-oriented, unlike docs/voice-testing which is for comparison and experiments

Included voices:
1. en_IN_female
   Local Indian female voice for:
   - Lakshmi Amma
   - Chinnamma Naik
   - Saraswati Chetti

2. en_GB_male
   Foreign male voice for:
   - Francisco de Almeida
   - Father Penteado

3. kusal_male
   Male fallback voice for:
   - Abdul Rahman
   - general fallback testing

Files expected in each voice folder:
- model.onnx
- tokens.txt
- espeak-ng-data/

Important:
- Only the 3 final selected voices are included here
- Do not delete docs/voice-testing; it remains the source of broader testing and comparisons
