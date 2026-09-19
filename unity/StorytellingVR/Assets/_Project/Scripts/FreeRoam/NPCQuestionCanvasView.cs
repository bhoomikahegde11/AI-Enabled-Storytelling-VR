using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestionCanvasView : MonoBehaviour
{
    [Header("Canvas Root")]
    [Tooltip("Usually this same GameObject.")]
    [SerializeField] private GameObject canvasRoot;

    [Header("Question Buttons")]
    [SerializeField] private Button[] questionButtons;

    [Header("Question Button Texts")]
    [SerializeField] private TMP_Text[] questionButtonTexts;

    public GameObject CanvasRoot
    {
        get
        {
            if (canvasRoot != null)
                return canvasRoot;

            return gameObject;
        }
    }

    public Button[] QuestionButtons => questionButtons;
    public TMP_Text[] QuestionButtonTexts => questionButtonTexts;

    public bool IsConfigured()
    {
        if (CanvasRoot == null)
            return false;

        if (questionButtons == null ||
            questionButtonTexts == null)
        {
            return false;
        }

        if (questionButtons.Length == 0)
            return false;

        if (questionButtons.Length != questionButtonTexts.Length)
            return false;

        return true;
    }

    public void SetVisible(bool visible)
    {
        CanvasRoot.SetActive(visible);
    }
}