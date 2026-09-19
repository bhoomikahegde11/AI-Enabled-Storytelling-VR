using UnityEngine;

public class NPCController : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.speed = 1;
            anim.SetTrigger("GiveCoin");
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.speed = 1;
        }
    }


    public void FreezeHand()
    {
        anim.speed = 0;
    }
}