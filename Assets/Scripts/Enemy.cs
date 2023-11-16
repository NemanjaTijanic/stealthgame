using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animController;
    private AudioSource audioSource;

    // States that the Enemy can be in
    enum States
    {
        GUARD,
        PATROL,
        PURSUE,
        DISTRACTED,
        FLEE
    }
    States currentState;

    [Header("General")]
    public int health = 100;
    public int maxHealth = 100;
    public float patrolSpeed = 1.5f;
    public float pursueSpeed = 3.5f;

    [Header("Senses")]
    public float attackRadius = 3f;
    private Vector3 lastPosition = Vector3.zero;

    // Sound
    public float soundDetectionRadius = 10f;

    // View
    public float viewRadius = 8f;
    [Range(0, 360)]
    public float viewAngle = 140f;
    public LayerMask obstacleMask;

    [Header("Guard")]
    public Transform guardPosition;

    [Header("Patrol")]
    public Transform[] patrolPositions;
    private int patrolIndex = 0;

    [Header("Pursue")]
    public GameObject player;
    private Player playerScript;
    public float detectionTime = 3f;
    public float detectionTimeCurrent = 0f;
    public float attackCooldown = 2f;
    private float attackCooldownCurrent = 0f;
    public bool searchedLastPosition = true;
    public int searchIndex = 0;

    [Header("Flee")]
    public int fleeHealthTreshold = 15;
    public float fleeDistance = 5;

    [Header("Distracted")]
    public Vector3 distractionPosition;
    public bool isDistracted = false;

    [Header("GM")]
    public GameObject gm;
    private GameGM gmScript;

    [Header("Audio")]
    public AudioClip detectedSFX;
    public AudioClip distractedSFX;
    public AudioClip hurtSFX;
    public AudioClip lostSFX;
    public AudioClip playerHurtSound;
    private bool playedSoundDistracted = false;
    private bool playedSoundDetected = false;

    [Header("UI")]
    public GameObject hurtPanel;
    private Image hurtImage;
    public byte hurtValueMax = 235;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animController = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerScript = player.GetComponent<Player>();
        gmScript = gm.GetComponent<GameGM>();

        isDistracted = false;

        // UI
        hurtImage = hurtPanel.GetComponent<Image>();

        // Check if Enemy has patrol routes
        if (patrolPositions.Length > 1)
            currentState = States.PATROL;
        else
            currentState = States.GUARD;
    }

    void Update()
    {
        StartCoroutine(AttackTimer());

        // Checking Enemy velocity for Animations
        if (agent.velocity.magnitude >= 0.1f)
            animController.SetFloat("speed", 100);
        else
            animController.SetFloat("speed", 0);

        // Flee on low health
        if(((health / maxHealth) * 100) < fleeHealthTreshold)
        {
            currentState = States.FLEE;
        }

        // Detection by sound
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance <= soundDetectionRadius && !playerScript.isWalking && playerScript.isMoving)
        {
            if (currentState != States.FLEE)
                currentState = States.PURSUE;
        }

        // Detection by view
        if (LookForPlayer() && currentState != States.FLEE)
        {
            currentState = States.PURSUE;
        }

        // Distraction
        if(isDistracted && currentState != States.FLEE && currentState != States.PURSUE)
        {
            currentState = States.DISTRACTED;
        }

        // States
        if (currentState == States.GUARD)
        {
            Guard();
        }
        else if(currentState == States.PATROL)
        {
            Patrol();
        }
        else if (currentState == States.PURSUE)
        {
            Pursue();
        }
        else if (currentState == States.FLEE)
        {
            Flee();
        }
        else if(currentState == States.DISTRACTED)
        {
            Distracted();
        }
    }

    private void Guard()
    {
        // Sound
        playedSoundDistracted = false;

        // Enemy is not Alarmed so he will walk
        animController.SetBool("alarmed", false);
        agent.speed = patrolSpeed;

        // Move to guard station if not already there
        if (agent.transform.position != guardPosition.position)
            agent.destination = guardPosition.position;
        else
            agent.transform.rotation = guardPosition.transform.rotation;
    }

    private void Patrol()
    {
        // Sound
        playedSoundDistracted = false;

        // Enemy is not Alarmed so he will walk
        animController.SetBool("alarmed", false);
        agent.speed = patrolSpeed;

        // Enemy needs minimum 2 positions to patrol between
        if (patrolPositions.Length < 2)
            return;

        // Moving Enemy to next checkpoint
        agent.SetDestination(patrolPositions[patrolIndex].position);
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            // Enemy loops between available positions
            patrolIndex = (patrolIndex + 1) % patrolPositions.Length;
            agent.SetDestination(patrolPositions[patrolIndex].position);
        }
    }

    private void Pursue()
    {
        // Sound
        if (!playedSoundDetected)
        {
            playedSoundDetected = true;
            audioSource.PlayOneShot(detectedSFX);
        }

        Vector3 direction = (player.transform.position - transform.position).normalized;
        StartCoroutine(DetectionTimer());

        // Enemy is running during pursuit
        animController.SetBool("alarmed", true);
        agent.speed = pursueSpeed;

        // Detection by sound
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= soundDetectionRadius && !playerScript.isWalking && playerScript.isMoving)
        {
            // Keep record of players last position and move to him
            lastPosition = player.transform.position;
            agent.destination = player.transform.position - (direction * 2);
            LookAtTarget();
            // Reset detection time
            detectionTimeCurrent = detectionTime;
        }

        // Detection by view
        if (LookForPlayer())
        {
            // Keep record of players last position and move to him
            lastPosition = player.transform.position;
            agent.destination = player.transform.position - (direction * 2);
            LookAtTarget();
            // Attack if in striking distance
            if (distance <= attackRadius)
            {
                Attack();
            }
            // Reset detection time
            detectionTimeCurrent = detectionTime;
        }

        // If detection timer is above 0, keep pursuing him
        if(detectionTimeCurrent > 0)
        {
            agent.destination = player.transform.position - (direction * 2);
            LookAtTarget();
            searchedLastPosition = false;

            searchIndex = 0;
        }
        // When detection timer goes below 0, search his last known position and 2 additional nearby positions
        else if(detectionTimeCurrent < 0 && !searchedLastPosition)
        {
            Vector3 searchPosOne = RandomLocation();
            Vector3 searchPosTwo = RandomLocation();
 
            // Check if last known positions were searched
            if(searchIndex == 0)
            {
                // Check at last seen position
                agent.destination = lastPosition;

                if (Vector3.Distance(transform.position, lastPosition) < 1)
                    searchIndex++; 
            }
            else if(searchIndex == 1)
            {
                agent.destination = searchPosOne;

                if (agent.remainingDistance <= agent.stoppingDistance)
                    searchIndex++;
            }
            else if(searchIndex == 2)
            {
                agent.destination = searchPosTwo;

                if (agent.remainingDistance <= agent.stoppingDistance)
                    searchedLastPosition = true;
            }
        }
        // After checking nearby locations, return to usual behaviour
        else if(detectionTimeCurrent < 0 && searchedLastPosition)
        {
            // Sound
            audioSource.PlayOneShot(lostSFX);
            playedSoundDetected = false;
            playedSoundDistracted = false;
            // Enemy is walking during patrol
            animController.SetBool("alarmed", false);
            agent.speed = patrolSpeed;
            // Return to Guarding/Patroling
            if (patrolPositions.Length > 1)
                currentState = States.PATROL;
            else
                currentState = States.GUARD;
        }
    }

    private void Flee()
    {
        // Enemy is running while fleeing
        animController.SetBool("alarmed", true);
        agent.speed = pursueSpeed;

        // Detection
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= soundDetectionRadius)
        {
            // Flee if Player is in range
            Vector3 direction = (player.transform.position - transform.position).normalized;
            agent.destination = transform.position - (direction * fleeDistance);
        }
        // Otherwise look at him
        else if (agent.velocity.magnitude == 0f)
            LookAtTarget();     
    }

    private void Distracted()
    {
        // Sound
        if (!playedSoundDistracted)
        {
            playedSoundDistracted = true;
            audioSource.PlayOneShot(distractedSFX);
        }

        agent.destination = distractionPosition;

        // if (agent.remainingDistance <= agent.stoppingDistance)
        if (agent.transform.position == agent.destination)
            isDistracted = false;

        if (!isDistracted)
        {
            StartCoroutine(DistractionWaitTime());
        }
    }

    private void LookAtTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private bool LookForPlayer()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= viewRadius)
        {
            Vector3 directionToTarget = (player.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, directionToTarget, distance, obstacleMask))
                {
                    //Debug.Log(!Physics.Raycast(transform.position, directionToTarget, distance, obstacleMask));
                    return true;
                }
            }
        }

        return false;
    }

    private void Attack()
    {
        if(attackCooldownCurrent <= 0 && player.activeSelf)
        {
            playerScript.health -= 10;
            playerScript.SetHealth();
            playerScript.LostHealth();

            attackCooldownCurrent = attackCooldown;
            animController.SetTrigger("attack");

            // Audio effects
            audioSource.PlayOneShot(playerHurtSound);

            // UI effects
            Color32 org = hurtImage.color;
            org.a = hurtValueMax;
            hurtImage.color = org;
            StartCoroutine(HurtImageFadeAway());

            if(playerScript.health <= 0)
            {
                gmScript.DefeatScreen();
            }
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public Vector3 RandomLocation()
    {
        float radius = 4;
        Vector3 direction = Random.insideUnitSphere * radius;
        direction += transform.position;

        NavMeshHit hit;
        Vector3 final = Vector3.zero;
        if (NavMesh.SamplePosition(direction, out hit, radius, 1))
        {
            final = hit.position;
        }

        if(!Physics.CheckSphere(final, 1))
        {
            return final;
        }
        else
        {
            return transform.position;
        }
    }

    public void HitByDistraction()
    {
        currentState = States.PURSUE;
        detectionTimeCurrent = detectionTime;

        audioSource.PlayOneShot(hurtSFX);

        // Debug
        //Debug.Log("HitByDistraction got called by " + gameObject.name);
    }

    private void OnDrawGizmosSelected()
    {
        // Sound radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, soundDetectionRadius);

        // View radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        // View angle
        Gizmos.color = Color.blue;
        Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2, false);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * 5);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * 5);
    }

    private IEnumerator DetectionTimer()
    {
        detectionTimeCurrent -= Time.deltaTime;

        yield return new WaitForSeconds(1);
    }

    private IEnumerator AttackTimer()
    {
        attackCooldownCurrent -= Time.deltaTime;

        yield return new WaitForSeconds(1);
    }

    private IEnumerator DistractionWaitTime()
    {
        yield return new WaitForSeconds(3);

        // Return to Guarding/ Patroling
        if (patrolPositions.Length > 1)
            currentState = States.PATROL;
        else
            currentState = States.GUARD;
    }

    private IEnumerator HurtImageFadeAway()
    {
        yield return new WaitForSeconds(0.75f);

        Color32 org = hurtImage.color;
        org.a = 75;
        hurtImage.color = org;

        yield return new WaitForSeconds(0.1f);
        org.a = 45;
        hurtImage.color = org;


        yield return new WaitForSeconds(0.1f);
        org.a = 0;
        hurtImage.color = org;
    }
}
