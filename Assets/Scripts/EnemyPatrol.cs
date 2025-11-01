using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex;

    public float speed = 2f;
    public float rotationSpeed = 5f; // Speed at which the enemy rotates to face target

    public float sightRange;
    public float sightRadius;
    public Transform sightStart;
    public LayerMask playerLayer;
    public GameObject player;


    [Tooltip("Set true if the model's forward (+Z) points backwards visually; this will invert the intended facing direction.")]
    public bool modelForwardIsBackwards = false;

    private bool aggroed;
    private Vector3 modelDirection;
    private Coroutine chaseRoutine = null;
    // memory: keep chasing for a short time after losing sight
    [Tooltip("How many seconds the enemy will continue chasing after losing sight of the player")]
    public float chaseMemoryDuration = 2f;
    private float lastSeenTime = -Mathf.Infinity;
    private Vector3 lastSeenPosition;

    private bool IsChasing()
    {
        return (Time.time - lastSeenTime) <= chaseMemoryDuration;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentWaypointIndex = 0;
        aggroed = false;
        StartCoroutine(Patrol());
    }

    // Update is called once per frame
    void Update()
    {
        modelDirection = modelForwardIsBackwards ? -transform.forward : transform.forward;

        RaycastHit hit;
        aggroed = Physics.SphereCast(sightStart.position, sightRadius, modelDirection, out hit, sightRange, playerLayer);
        // update last-seen info when we detect the player
        if (aggroed)
        {
            lastSeenTime = Time.time;
            // prefer the player's transform position if available
            if (player == null)
            {
                var found = GameObject.FindWithTag("Player");
                if (found != null) player = found;
            }
            lastSeenPosition = (player != null) ? player.transform.position : hit.point;
        }

        // Decide whether we should be chasing (still within memory duration)
        bool shouldChase = IsChasing();
        // Start/stop chase coroutine on chase state transitions
        if (shouldChase)
        {
            if (chaseRoutine == null && player != null)
            {
                chaseRoutine = StartCoroutine(Chase());
            }
        }
        else
        {
            if (chaseRoutine != null)
            {
                StopCoroutine(chaseRoutine);
                chaseRoutine = null;
            }
        }
    }

    IEnumerator Patrol()
    {
        // Basic safety checks
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("EnemyPatrol: no waypoints assigned.");
            yield break;
        }

        // Local index and direction (1 forward, -1 backward)
        int waypointIndex = currentWaypointIndex;
        int direction = (waypointIndex >= waypoints.Length - 1) ? -1 : 1;

        // Main loop: run forever, but pause while aggroed and resume when cleared
        while (true)
        {
            // If we are in a chasing state (within memory window), wait here until chasing stops
            if (IsChasing())
            {
                yield return new WaitWhile(() => IsChasing());
                // After resuming, continue to next iteration without advancing waypoint so enemy picks up where it left off
                continue;
            }

            // Ensure target is valid
            if (waypointIndex < 0 || waypointIndex >= waypoints.Length || waypoints[waypointIndex] == null)
            {
                // Reset to 0 if index invalid
                waypointIndex = 0;
                currentWaypointIndex = 0;
                yield return null;
                continue;
            }

            Vector3 targetPosition = waypoints[waypointIndex].position;

            // Move towards the target each frame until we are close enough or we enter chasing state
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (IsChasing()) break; // pause movement immediately if chasing started

                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // Calculate the raw direction to target and apply model-forward inversion if necessary
                Vector3 rawDirectionToTarget = targetPosition - transform.position;
                rawDirectionToTarget.y = 0f; // keep yaw-only rotation
                rawDirectionToTarget = rawDirectionToTarget.normalized;
                Vector3 directionToTarget = modelForwardIsBackwards ? -rawDirectionToTarget : rawDirectionToTarget;

                if (directionToTarget != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                yield return null;
            }

            // If we exited because chasing started, the outer loop will handle waiting; do not advance waypoint; this line of code returns us to the top of the outer loop so that the waypoint doesn't advance mid-movement
            if (IsChasing())
            {
                continue;
            }

            // If there's only one waypoint, just pause briefly and try again
            if (waypoints.Length == 1)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Flip direction at endpoints so we ping-pong
            if (waypointIndex == waypoints.Length - 1)
            {
                direction = -1;
            }
            else if (waypointIndex == 0)
            {
                direction = 1;
            }

            // Advance index and update public field
            waypointIndex += direction;
            currentWaypointIndex = waypointIndex;

            // small pause before heading to next point
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Chase() {
        // Chase while within chase memory window. Move toward last seen position if player not currently visible.
        while (IsChasing())
        {
            Vector3 target = lastSeenPosition;
            // if player is visible/assigned, prefer player's current position
            if (player != null)
            {
                target = player.transform.position;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-(target - transform.position).normalized), rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target, (speed * 2) * Time.deltaTime);
            Debug.Log("chasing player/last seen");
            yield return null;
        }
    }
    
    private void OnDrawGizmos() 
    {
        // Use the same facing direction as the SphereCast
        Vector3 facingDirection = modelForwardIsBackwards ? -transform.forward : transform.forward;
        
        // Draw sight range visualization
        Gizmos.color = aggroed ? Color.red : Color.yellow; // Red when player detected, yellow otherwise
        
        // Draw the spherecast volume (cylinder)
        Vector3 endPoint = sightStart.position + facingDirection * sightRange;
        // Draw spheres at start and end of cast
        Gizmos.DrawWireSphere(sightStart.position, sightRadius);
        Gizmos.DrawWireSphere(endPoint, sightRadius);
        
        // Draw lines connecting the spheres to show the sweep volume
        Vector3 right = sightStart.right * sightRadius;
        Vector3 up = sightStart.up * sightRadius;
        // Draw four lines to represent the swept volume
        Gizmos.DrawLine(sightStart.position + right, endPoint + right);
        Gizmos.DrawLine(sightStart.position - right, endPoint - right);
        Gizmos.DrawLine(sightStart.position + up, endPoint + up);
        Gizmos.DrawLine(sightStart.position - up, endPoint - up);
    }
}
