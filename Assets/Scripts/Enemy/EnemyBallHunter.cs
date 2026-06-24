using UnityEngine;
using UnityEngine.AI;

public class EnemyBallHunter : MonoBehaviour
{
    public float kickForce = 15f;
    public float kickDistance = 2f;

    [Header("Vision")]
    public float viewRadius = 15f;
    public float viewAngle = 90f;
    public LayerMask wallMask;

    [Header("Face Materials")]
    public Renderer enemyRenderer;

    public Material smileFace;
    public Material angryFace;

    [Header("Memory")]
    public float loseSightTime = 1.5f;
    private float loseTimer;

    [Header("Kick Cooldown")]
    public float kickCooldown = 2f;
    private float kickTimer;

    [Header("Team AI")]
    public float separationRadius = 2.5f;
    public float separationStrength = 3f;

    [Header("Anti Corner System")]
    public float stuckCheckTime = 1.2f;
    public float stuckSpeedThreshold = 0.4f;
    public float escapeForce = 6f;

    private float stuckTimer;
    private Vector3 lastBallPos;

    private NavMeshAgent agent;
    private Rigidbody ball;
    private Transform ballTarget;

    private Vector3 startPosition;

    [Header("Animation")]
    public Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip kickSFX;
    [Range(0f, 1f)] public float volume = 1f;

    // -------- PATROL IMPROVEMENT --------
    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float idleTime = 2f;

    private float idleTimer;
    private Vector3 patrolPoint;
    private bool hasPatrolPoint;

    private enum State { Patrol, Chase, Return }
    private State state;

    // -------- 2-HIT LIMIT --------
    [Header("Hit Limit")]
    public int maxHitsPerChase = 2;
    private int hitCount;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        state = State.Patrol;
        hitCount = 0;
    }

    void Update()
    {
        kickTimer -= Time.deltaTime;

        ScanForBall();

        switch (state)
        {
            case State.Patrol:
                SetFace(false);
                Patrol();
                break;

            case State.Chase:
                SetFace(true);
                Chase();
                break;

            case State.Return:
                SetFace(false);
                ReturnToStart();
                break;
        }

        if (animator)
            animator.SetFloat("Speed", agent.velocity.magnitude);

        ApplySeparation();
        HandleCornerStuck();
    }

    // ---------------- SEPARATION ----------------
    void ApplySeparation()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);

        Vector3 push = Vector3.zero;
        int count = 0;

        foreach (var col in nearby)
        {
            if (col.gameObject == gameObject) continue;

            if (col.CompareTag("Enemy"))
            {
                Vector3 diff = transform.position - col.transform.position;
                push += diff.normalized / Mathf.Max(diff.magnitude, 0.1f);
                count++;
            }
        }

        if (count > 0)
        {
            push /= count;
            agent.Move(push * separationStrength * Time.deltaTime);
        }
    }

    // ---------------- SCAN (IMPROVED MULTI AI) ----------------
    void ScanForBall()
    {
        // Don't pick up the ball again if we've already hit our limit this chase cycle
        if (hitCount >= maxHitsPerChase && state != State.Patrol)
            return;

        if (ballTarget == null)
        {
            // Only scan when patrolling or returning — not while already chasing
            if (state == State.Chase) return;

            // Reset hit counter when starting a fresh chase from patrol/return
            if (hitCount >= maxHitsPerChase) return;

            GameObject obj = GameObject.FindGameObjectWithTag("Ball");
            if (obj == null) return;

            Vector3 dir = obj.transform.position - transform.position;
            float dist = dir.magnitude;

            if (dist > viewRadius) return;

            float angle = Vector3.Angle(transform.forward, dir);
            if (angle > viewAngle / 2f) return;

            if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, dist, wallMask))
                return;

            ballTarget = obj.transform;
            ball = obj.GetComponent<Rigidbody>();
            loseTimer = 0;

            state = State.Chase;
        }
        else
        {
            Vector3 dir = ballTarget.position - transform.position;
            float dist = dir.magnitude;

            bool blocked =
                Physics.Raycast(transform.position + Vector3.up, dir.normalized, dist, wallMask);

            if (blocked)
            {
                loseTimer += Time.deltaTime;

                if (loseTimer >= loseSightTime)
                {
                    ballTarget = null;
                    ball = null;
                    state = State.Return;
                }
            }
            else
            {
                loseTimer = 0;
            }
        }
    }

    // ---------------- NATURAL PATROL (FIXED) ----------------
    void Patrol()
    {
        agent.speed = 3.5f;

        // Reset hit counter when we begin patrolling so enemy can chase again
        hitCount = 0;

        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
            return;
        }

        if (!hasPatrolPoint)
        {
            Vector3 random = startPosition + Random.insideUnitSphere * patrolRadius;
            random.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(random, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolPoint = hit.position;
                hasPatrolPoint = true;
                agent.SetDestination(patrolPoint);
            }
        }

        if (hasPatrolPoint && !agent.pathPending && agent.remainingDistance < 1f)
        {
            hasPatrolPoint = false;
            idleTimer = idleTime;
        }
    }

    // ---------------- CHASE ----------------
    void Chase()
    {
        if (ballTarget == null)
        {
            state = State.Return;
            return;
        }

        // Hit limit reached — stop chasing and go back
        if (hitCount >= maxHitsPerChase)
        {
            ballTarget = null;
            ball = null;
            state = State.Return;
            return;
        }

        agent.SetDestination(ballTarget.position);

        float dist = Vector3.Distance(transform.position, ballTarget.position);

        if (dist <= kickDistance)
        {
            Vector3 orbit = Vector3.Cross(
                (ballTarget.position - transform.position).normalized,
                Vector3.up
            );

            agent.Move(orbit * 2f * Time.deltaTime);

            KickBall();
        }
    }

    // ---------------- RETURN ----------------
    void ReturnToStart()
    {
        agent.SetDestination(startPosition);

        if (Vector3.Distance(transform.position, startPosition) < 1f)
            state = State.Patrol;
    }

    // ---------------- CORNER FIX (IMPROVED) ----------------
    void HandleCornerStuck()
    {
        if (ball == null) return;

        float speed = ball.linearVelocity.magnitude;
        float dist = Vector3.Distance(ball.position, lastBallPos);

        if (speed < stuckSpeedThreshold && dist < 0.05f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0;
        }

        lastBallPos = ball.position;

        if (stuckTimer >= stuckCheckTime)
        {
            // Cast rays to detect which walls are close so we kick away from them
            Vector3 ballPos = ball.position;
            Vector3 awayFromEnemy = (ballPos - transform.position).normalized;
            awayFromEnemy.y = 0;

            // Check for walls on left and right sides to pick the more open direction
            Vector3 right = Vector3.Cross(Vector3.up, awayFromEnemy);
            bool wallRight = Physics.Raycast(ballPos, right, 1.5f, wallMask);
            bool wallLeft  = Physics.Raycast(ballPos, -right, 1.5f, wallMask);
            bool wallAhead = Physics.Raycast(ballPos, awayFromEnemy, 1.5f, wallMask);

            Vector3 sideDir;
            if (wallRight && !wallLeft)
                sideDir = -right;           // open left
            else if (wallLeft && !wallRight)
                sideDir = right;            // open right
            else
                sideDir = right;            // both blocked or both open — pick right

            // If the direct path is also blocked, angle away more sharply
            float sideBias = wallAhead ? 1.5f : 0.8f;

            Vector3 finalKick = (awayFromEnemy + sideDir * sideBias + Vector3.up * 0.25f).normalized;

            ball.linearVelocity = Vector3.zero; // clear stuck velocity first
            ball.AddForce(finalKick * escapeForce, ForceMode.Impulse);

            PlayKickSound();

            stuckTimer = 0;

            // Count this as one of the hits
            hitCount++;

            if (hitCount >= maxHitsPerChase)
            {
                ballTarget = null;
                ball = null;
                state = State.Return;
            }
        }
    }

    // ---------------- KICK ----------------
    void KickBall()
    {
        if (kickTimer > 0 || ball == null) return;

        Vector3 dir = (ballTarget.position - transform.position).normalized;
        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z).normalized;

        Vector3 sidePush = Vector3.Cross(flatDir, Vector3.up);

        Vector3 kickDir = (flatDir + Vector3.up * 0.15f);

        ball.AddForce(kickDir.normalized * kickForce, ForceMode.Impulse);

        PlayKickSound();

        kickTimer = kickCooldown;

        hitCount++;

        if (hitCount >= maxHitsPerChase)
        {
            // Hit limit reached — disengage immediately
            ballTarget = null;
            ball = null;
            state = State.Return;
        }
        else
        {
            // Still have hits left — keep chasing
            ballTarget = null;
            ball = null;
            state = State.Return;
        }
    }

    // ---------------- SOUND ----------------
    void PlayKickSound()
    {
        if (audioSource == null || kickSFX == null) return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(kickSFX, volume);
    }

    // ---------------- COLLISION ----------------
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Ball")) return;

        Rigidbody ballRb = collision.collider.attachedRigidbody;
        if (ballRb == null) return;

        Vector3 pushDir = transform.forward;
        pushDir.y = 0.15f;

        ballRb.AddForce(pushDir.normalized * 4f, ForceMode.Impulse);
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Ball")) return;

        Rigidbody rb = collision.collider.attachedRigidbody;

        Vector3 follow = transform.forward * 2f;
        follow.y = 0;

        rb.AddForce(follow, ForceMode.Acceleration);
    }

    // ---------------- FACE ----------------
    void SetFace(bool angry)
    {
        if (enemyRenderer == null) return;

        Material[] mats = enemyRenderer.materials;

        if (mats.Length < 2) return;

        mats[1] = angry ? angryFace : smileFace;

        enemyRenderer.materials = mats;
    }
}