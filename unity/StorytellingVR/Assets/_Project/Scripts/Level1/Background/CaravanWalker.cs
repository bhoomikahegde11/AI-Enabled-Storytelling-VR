using System.Collections.Generic;
using UnityEngine;

public class CaravanWalker : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 1.8f;
    [SerializeField] private float rotationSpeed = 3.5f;
    [SerializeField] private float stopDistance = 0.35f;

    [Header("Optional Audio")]
    [SerializeField] private AudioSource audioSource;

    private CaravanSpawner spawner;
    private List<Transform> routePoints = new List<Transform>();
    private int currentRouteIndex = 0;
    private bool isInitialized = false;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void Initialize(
        CaravanSpawner ownerSpawner,
        List<Transform> assignedRoute)
    {
        spawner = ownerSpawner;
        routePoints = assignedRoute != null
            ? new List<Transform>(assignedRoute)
            : new List<Transform>();

        currentRouteIndex = 0;
        isInitialized = routePoints.Count > 0;

        if (isInitialized && routePoints[0] != null)
        {
            Vector3 direction =
                routePoints[0].position - transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(direction.normalized);
            }
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!isInitialized)
            return;

        if (currentRouteIndex >= routePoints.Count)
        {
            FinishRoute();
            return;
        }

        Transform currentPoint = routePoints[currentRouteIndex];

        if (currentPoint == null)
        {
            currentRouteIndex++;
            return;
        }

        Vector3 direction =
            currentPoint.position - transform.position;

        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= Mathf.Max(0.01f, stopDistance))
        {
            currentRouteIndex++;
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        transform.position +=
            transform.forward *
            speed *
            Time.deltaTime;
    }

    private void FinishRoute()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (spawner != null)
        {
            spawner.CaravanRemoved();
        }

        Destroy(gameObject);
    }
}
