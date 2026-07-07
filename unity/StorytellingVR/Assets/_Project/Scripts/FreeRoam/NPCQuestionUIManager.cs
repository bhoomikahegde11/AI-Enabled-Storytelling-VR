using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuestionUIManager : MonoBehaviour
{
    public static NPCQuestionUIManager Instance;

    public GameObject canvas;

    public Button[] buttons;

    public TMP_Text[] buttonTexts;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvas.SetActive(false);
    }

    public void Open(NPCDialogueData data)
    {
        canvas.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < data.questions.Length)
            {
                buttons[i].gameObject.SetActive(true);

                buttonTexts[i].text =
                    data.questions[i].question;
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void Close()
    {
        canvas.SetActive(false);
    }
}