using Oculus.Interaction;
using UnityEngine;

public class ModernStudyRoomIntroController : MonoBehaviour
{
    [SerializeField]
    private Grabbable _timeTravelNotebookGrabbable;

    private bool _hasStarted;

    private void OnEnable()
    {
        if (_timeTravelNotebookGrabbable != null)
        {
            _timeTravelNotebookGrabbable.WhenPointerEventRaised += HandleNotebookPointerEvent;
        }
    }

    private void OnDisable()
    {
        if (_timeTravelNotebookGrabbable != null)
        {
            _timeTravelNotebookGrabbable.WhenPointerEventRaised -= HandleNotebookPointerEvent;
        }
    }

    private void HandleNotebookPointerEvent(PointerEvent pointerEvent)
    {
        if (_hasStarted || pointerEvent.Type != PointerEventType.Select)
        {
            return;
        }

        _hasStarted = true;
        Debug.Log("[INTRO] TimeTravelNotebook grabbed. Intro sequence started.", this);
    }
}
