using UnityEngine;
using TMPro;

public class NPCManager : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform destination; // The "Stop Point" empty object at your stall
    public float speed = 1.2f;

    [Header("UI & Content")]
    public GameObject infoCanvas; // The World-Space Canvas floating next to them
    public TextMeshProUGUI factText; // The text element inside that canvas
    [TextArea] public string historicalFact; // Type the Portuguese facts here

    private Animator anim;
    private bool hasArrived = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        infoCanvas.SetActive(false); // Hide the UI until they arrive
        factText.text = historicalFact;
    }

    void Update()
    {
        if (hasArrived) return;

        // Calculate distance to the stall
        float dist = Vector3.Distance(transform.position, destination.position);

        if (dist > 0.1f)
        {
            // Move and Look towards the stall
            transform.position = Vector3.MoveTowards(transform.position, destination.position, speed * Time.deltaTime);

            // Smoother rotation
            Vector3 direction = (destination.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            Arrive();
        }
    }

    void Arrive()
    {
        hasArrived = true;
        anim.SetBool("isAtStall", true); // Trigger the Idle/Gesture animation
        infoCanvas.SetActive(true); // Pop up the info box
    }
}