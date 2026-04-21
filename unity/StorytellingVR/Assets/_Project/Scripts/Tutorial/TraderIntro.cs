using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TraderIntro : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI subtitleText;

    [Header("NPC")]
    public Transform npc;
    public Transform npcTarget;
    public float npcSpeed = 1.5f;

    [Header("Player")]
    public Transform playerCamera;
    public Volume globalVolume;
    DepthOfField dof;
    Vignette vignette;
    Camera cam;
    Animator animator;
    Coroutine dynamicFocusCoroutine;

    private bool isFrozen = false;

    void Start()
    {
        globalVolume.profile.TryGet(out dof);

        globalVolume.profile.TryGet(out vignette);

        cam = Camera.main;

        // start normal
        vignette.intensity.value = 0.12f; // subtle always-on

        // no blur initially (Bokeh approach)
        dof.focusDistance.value = 10f;
        dof.aperture.value = 5f;

        animator = npc.GetComponentInChildren<Animator>();
        animator.SetBool("isWalking", false);

        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        yield return new WaitForSecondsRealtime(1f);

        yield return ShowLine("Wait… someone approaches.", 1.5f);

        // NPC walks IN (blocking)
        yield return StartCoroutine(MoveNPC());

        // small pause after arrival
        yield return new WaitForSecondsRealtime(0.3f);

        // NPC turns to player
        yield return StartCoroutine(FacePlayerSmooth());

        // 🎬 CINEMATIC FOCUS START
        yield return StartCoroutine(SmoothFreeze());
        StartCoroutine(FocusOnNPC_Bokeh());
        dynamicFocusCoroutine = StartCoroutine(LockFocusOnNPC_Bokeh());
        StartCoroutine(CinematicFocusIn());

        yield return ShowLine("Look closely.", 2f);
        yield return new WaitForSecondsRealtime(0.3f);

        yield return ShowLine(
            "From his attire and manner, he appears to be a trader of these lands… perhaps from the Deccan, or the southern regions of the empire.",
            5f
        );
        yield return new WaitForSecondsRealtime(0.3f);

        yield return ShowLine(
            "In the markets of Vijayanagara, merchants came from many places — local farmers, inland traders, and even foreign visitors from distant shores.",
            5f
        );
        yield return new WaitForSecondsRealtime(0.3f);

        yield return ShowLine(
            "Each carried different goods, different knowledge, and different intentions.",
            3.5f
        );
        yield return new WaitForSecondsRealtime(0.3f);

        // OPTIONAL historical reference
        yield return ShowLine(
            "Some traders came from far beyond the seas — Portuguese, Arabs — drawn by the wealth of spices.",
            4f
        );
        yield return new WaitForSecondsRealtime(0.3f);

        // 🎬 RESUME
        if (dynamicFocusCoroutine != null) StopCoroutine(dynamicFocusCoroutine);
        StartCoroutine(ResetFocus_Bokeh());
        StartCoroutine(CinematicFocusOut());
        ResumeTime();

        yield return ShowLine("And now… he stands before you.", 2.5f);
        yield return new WaitForSecondsRealtime(0.3f);

        yield return ShowLine("Let us see how you handle your first customer.", 3f);
    }

    IEnumerator MoveNPC()
    {
        float stoppingDistance = 0.15f;

        animator.SetBool("isWalking", true);

        while (true)
        {
            if (isFrozen)
            {
                yield return null;
                continue;
            }

            Vector3 direction = npcTarget.position - npc.position;
            direction.y = 0;

            float distance = direction.magnitude;

            // 🛑 STOP CONDITION
            if (distance < stoppingDistance)
            {
                break;
            }

            direction.Normalize();

            // 🔥 MOVE
            Vector3 newPos = npc.position + direction * npcSpeed * Time.deltaTime;
            newPos.y = npc.position.y; // 🔥 locks height
            npc.position = newPos;

            // 🔥 ROTATE CONTINUOUSLY (FIXES SNAP TURN)
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            npc.rotation = Quaternion.Slerp(
                npc.rotation,
                targetRotation,
                Time.deltaTime * 6f   // smoother turning
            );

            yield return null;
        }

        // 🛑 STOP CLEANLY
        animator.SetBool("isWalking", false);

        // 🔥 FORCE IDLE (important safety)
        animator.Play("Idle");
    }

    IEnumerator FacePlayerSmooth()
    {
        Vector3 dir = Camera.main.transform.position - npc.position;
        dir.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        while (Quaternion.Angle(npc.rotation, targetRot) > 1f)
        {
            npc.rotation = Quaternion.Slerp(
                npc.rotation,
                targetRot,
                Time.deltaTime * 3f
            );
            yield return null;
        }
    }

    IEnumerator ShowLine(string text, float duration)
    {
        subtitleText.text = text;
        yield return new WaitForSecondsRealtime(duration);
        subtitleText.text = "";
    }

    IEnumerator FocusOnNPC_Bokeh()
    {
        float t = 0;

        // Target upper body/face for accurate distance computation instead of the feet
        Vector3 focusPoint = npc.position + Vector3.up * 1.5f;
        float distance = Vector3.Distance(
            Camera.main.transform.position,
            focusPoint
        );

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 2f;

            // 🔥 strong blur targeting the specific distance with adjusted aperture for close-up
            dof.focusDistance.value = Mathf.Lerp(10f, distance, t);
            dof.aperture.value = Mathf.Lerp(5f, 0.6f, t);

            yield return null;
        }
    }

    IEnumerator LockFocusOnNPC_Bokeh()
    {
        // Wait for FocusOnNPC_Bokeh to finish interpolating before locking the values
        yield return new WaitForSecondsRealtime(0.5f);

        while (true)
        {
            Vector3 focusPoint = npc.position + Vector3.up * 1.5f;
            float distance = Vector3.Distance(
                Camera.main.transform.position,
                focusPoint
            );

            dof.focusDistance.value = distance;
            dof.aperture.value = 0.6f;

            yield return null;
        }
    }

    IEnumerator ResetFocus_Bokeh()
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 2f;

            dof.aperture.value = Mathf.Lerp(dof.aperture.value, 5f, t);

            yield return null;
        }
    }

    IEnumerator CinematicFocusIn()
    {
        float t = 0;

        // float startFOV = cam.fieldOfView;
        // float targetFOV = 75f;

        float startVignette = vignette.intensity.value;
        float targetVignette = 0.35f;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 2f;

            // 🎥 NOTE: Unity XR / VR restricts FOV tweaks native to stereoscopic lenses.
            // Executing math over fieldOfView in VR triggers motion sickness and SDK conflicts! (Left commented below)
            // cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            // 🌑 Vignette increase
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, t);

            yield return null;
        }
    }

    IEnumerator CinematicFocusOut()
    {
        float t = 0;

        // float startFOV = cam.fieldOfView;
        // float targetFOV = 90f;

        float startVignette = vignette.intensity.value;
        float targetVignette = 0.12f; // back to subtle

        while (t < 1)
        {
            t += Time.unscaledDeltaTime * 2f;

            // 🎥 Keep disabled for VR stability
            // cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, t);

            yield return null;
        }
    }

    IEnumerator SmoothFreeze()
    {
        isFrozen = true;

        float t = 1f;
        while (t > 0.1f)
        {
            t -= Time.unscaledDeltaTime * 2f;
            Time.timeScale = t;
            yield return null;
        }
    }

    void ResumeTime()
    {
        Time.timeScale = 1f;
        isFrozen = false;
    }

    IEnumerator SmoothFacePlayer()
    {
        Vector3 dir = playerCamera.position - npc.position;
        dir.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        while (Quaternion.Angle(npc.rotation, targetRot) > 1f)
        {
            npc.rotation = Quaternion.Slerp(
                npc.rotation,
                targetRot,
                Time.deltaTime * 3f
            );
            yield return null;
        }
    }
}