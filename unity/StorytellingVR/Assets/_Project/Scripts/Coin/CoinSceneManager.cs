using System.Collections;
using TMPro;
using UnityEngine;

public class CoinSceneManager : MonoBehaviour
{
    [Header("References")]
    public NPCAnimationController npc;
    public TMP_Text dialogueText;

    [Header("Coins")]
    public GameObject varahaCoin;


    void Start()
    {
        StartCoroutine(StartCoinScene());
    }


    IEnumerator StartCoinScene()
    {
        // starting delay
        yield return new WaitForSeconds(2);


        yield return ShowDialogue(
            "A fair bargain, merchant. Here is your payment.",
            3
        );


        // NPC raises hand
        npc.GiveCoin();


        // wait until hand is raised
        yield return new WaitForSeconds(1.2f);


        // show coin
        varahaCoin.SetActive(true);


        yield return ShowDialogue(
            "Take a closer look at this coin.",
            3
        );


        // Now wait for grab later
    }


    IEnumerator ShowDialogue(string line, float time)
    {
        dialogueText.text = line;

        yield return new WaitForSeconds(time);

        dialogueText.text = "";
    }
}