using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

namespace StorytellingVR.Tutorial
{
    public static class TutorialDialogueUIHelper
    {
        public static IEnumerator PlayDialogue(
            TMP_Text subtitleText,
            AudioSource audioSource,
            AudioClip clip,
            string[] lines,
            Func<bool> isTriggerPressed,
            Action onFinished = null)
        {
            if (lines == null || lines.Length == 0) yield break;

            if (audioSource != null && clip != null)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            }
            else if (audioSource != null && clip == null)
            {
                audioSource.Stop();
            }

            float charactersPerSecond = 40f;
            float commaPause = 0.08f;
            float sentencePause = 0.16f;

            bool previousPressed = isTriggerPressed != null && isTriggerPressed.Invoke();

            for (int i = 0; i < lines.Length; i++)
            {
                string text = lines[i];

                if (subtitleText != null)
                {
                    subtitleText.text = text;
                    subtitleText.maxVisibleCharacters = 0;
                    subtitleText.ForceMeshUpdate();
                }

                TMP_TextInfo textInfo = subtitleText != null ? subtitleText.textInfo : null;
                int totalCharacters = textInfo != null ? textInfo.characterCount : text.Length;
                
                bool textCompletedInstantly = false;

                if (subtitleText != null && totalCharacters > 0)
                {
                    for (int visibleCount = 0; visibleCount < totalCharacters; visibleCount++)
                    {
                        float characterDelay = 1f / charactersPerSecond;
                        
                        if (textInfo != null && visibleCount < textInfo.characterCount)
                        {
                            char currentCharacter = textInfo.characterInfo[visibleCount].character;

                            if (currentCharacter == ',') characterDelay += commaPause;
                            else if (currentCharacter == '.' || currentCharacter == '!' || currentCharacter == '?' || currentCharacter == ':' || currentCharacter == ';')
                                characterDelay += sentencePause;
                        }

                        float elapsed = 0f;
                        while (elapsed < characterDelay)
                        {
                            bool currentlyPressed = isTriggerPressed != null && isTriggerPressed.Invoke();
#if UNITY_EDITOR
                            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) currentlyPressed = true;
#endif
                            bool freshPress = currentlyPressed && !previousPressed;
                            previousPressed = currentlyPressed;

                            if (freshPress)
                            {
                                subtitleText.maxVisibleCharacters = totalCharacters;
                                textCompletedInstantly = true;
                                break;
                            }

                            elapsed += Time.unscaledDeltaTime;
                            yield return null;
                        }

                        if (textCompletedInstantly) break;
                        subtitleText.maxVisibleCharacters = visibleCount + 1;
                    }

                    subtitleText.maxVisibleCharacters = totalCharacters;
                }

                // Wait for release if it was pressed to skip typing
                if (isTriggerPressed != null)
                {
                    while (isTriggerPressed.Invoke()
#if UNITY_EDITOR
                           || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)
#endif
                    )
                    {
                        previousPressed = true;
                        yield return null;
                    }
                    previousPressed = false;
                }

                // Wait for next fresh press to advance
                bool advanceLine = false;
                while (!advanceLine)
                {
                    bool currentlyPressed = isTriggerPressed != null && isTriggerPressed.Invoke();
#if UNITY_EDITOR
                    if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) currentlyPressed = true;
#endif
                    bool freshPress = currentlyPressed && !previousPressed;

                    if (freshPress)
                    {
                        advanceLine = true;
                    }

                    previousPressed = currentlyPressed;
                    yield return null;
                }
            }

            // End of entire dialogue unit
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            // Prevent a held trigger from skipping the next line outside this sequence
            if (isTriggerPressed != null)
            {
                while (isTriggerPressed.Invoke()
#if UNITY_EDITOR
                       || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)
#endif
                )
                {
                    yield return null;
                }
            }

            if (subtitleText != null)
            {
                subtitleText.text = "";
            }

            onFinished?.Invoke();
        }

        public static bool GetRightTriggerPressed()
        {
            InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            bool pressed = false;
            rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out pressed);
            return pressed;
        }
    }
}