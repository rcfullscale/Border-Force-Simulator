using UnityEngine;
using UnityEngine.UI;


public class BoatRandomMovement2D : MonoBehaviour
{
    private BoatData boatData;
    [Header("Movement Settings")]
    public float maxSpeed = 1.8f;              // Top speed
    public float acceleration = 0.6f;          // How quickly it reaches max speed
    public float deceleration = 0.4f;          // How quickly it slows down (idle/turning)
    public float turnSpeed = 35f;              // Degrees per second — keep low for sluggish turns
    public float turnSlowdown = 0.6f;          // Speed multiplier applied while turning sharply

    [Header("Wandering Settings")]
    public float wanderRadius = 10f;
    public float minWaypointDistance = 3f;
    public float maxWaypointDistance = 8f;
    public float waypointRadius = 0.5f;

    [Header("Water Physics Feel")]
    public float driftAmount = 0.04f;          // Lateral drift from water
    public float driftSpeed = 0.5f;
    public float momentumDrag = 0.98f;         // 1 = no drag, lower = more drag (coasting feel)

    [Header("Idle / Drift Settings")]
    public float idleChance = 0.2f;
    public float idleMinTime = 2f;
    public float idleMaxTime = 6f;
    public float idleDriftSpeed = 0.15f;       // Slow passive drift while idle

    // Private state
    private Vector2 _startPosition;
    private Vector2 _targetWaypoint;
    private Vector2 _velocity;
    private float _currentAngle;
    private float _speed;
    private bool _isIdle;
    private float _idleTimer;
    private float _driftOffset;
    private float _idleDriftAngle;

    void Start()
    {
        boatData = GetComponent<BoatData>();
        _startPosition = transform.position;
        _driftOffset = Random.Range(0f, Mathf.PI * 2f);
        _currentAngle = transform.eulerAngles.z;
        _idleDriftAngle = Random.Range(0f, 360f);
        PickNewWaypoint();
        
    }

    void Update()
    {
        if (boatData != null && boatData.isDead)
        {
            maxSpeed = 0f;
            return;
        }
        if (_isIdle)
            HandleIdle();
        else
            HandleMovement();

        ApplyMomentum();
        ApplyVisualRotation();
    }

    // ─────────────────────────────────────────────
    //  Active movement
    // ─────────────────────────────────────────────

    void HandleMovement()
    {
        Vector2 toTarget = _targetWaypoint - (Vector2)transform.position;
        float dist = toTarget.magnitude;

        // Left-facing sprite: angle to target needs +180 offset
        float desiredAngle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg + 180f;

        // How sharp is the turn?
        float absDiff = Mathf.Abs(Mathf.DeltaAngle(_currentAngle, desiredAngle));

        // Slow down proportionally when turning hard (boats can't turn at full speed)
        float turnFactor = Mathf.Lerp(1f, turnSlowdown, absDiff / 90f);

        // Gradually rotate toward desired heading
        _currentAngle = Mathf.MoveTowardsAngle(_currentAngle, desiredAngle, turnSpeed * Time.deltaTime);

        // Target speed reduced when turning or near the waypoint
        float targetSpeed = maxSpeed * turnFactor;
        float brakeDist = maxSpeed * 1.2f;
        if (dist < brakeDist)
            targetSpeed *= Mathf.Clamp01(dist / brakeDist);

        _speed = Mathf.MoveTowards(_speed, targetSpeed, acceleration * Time.deltaTime);

        // Build velocity from heading + subtle lateral water drift
        Vector2 heading = GetForwardVector();
        Vector2 lateral = new Vector2(-heading.y, heading.x);
        float drift = Mathf.Sin(Time.time * driftSpeed + _driftOffset) * driftAmount;

        _velocity = heading * _speed + lateral * drift;

        // Waypoint reached?
        if (dist <= waypointRadius)
        {
            if (Random.value < idleChance)
            {
                _isIdle = true;
                _idleTimer = Random.Range(idleMinTime, idleMaxTime);
                _idleDriftAngle = _currentAngle;
            }
            else
            {
                PickNewWaypoint();
            }
        }
    }

    // ─────────────────────────────────────────────
    //  Idle — boat coasts and drifts gently
    // ─────────────────────────────────────────────

    void HandleIdle()
    {
        _idleTimer -= Time.deltaTime;

        // Bleed off speed naturally
        _speed = Mathf.MoveTowards(_speed, 0f, deceleration * Time.deltaTime);

        // Passive slow arc drift (simulates wind/current nudging the boat)
        _idleDriftAngle += 4f * Time.deltaTime;
        float rad = _idleDriftAngle * Mathf.Deg2Rad;
        _velocity = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * idleDriftSpeed;

        if (_idleTimer <= 0f)
        {
            _isIdle = false;
            PickNewWaypoint();
        }
    }

    // ─────────────────────────────────────────────
    //  Physics & rendering
    // ─────────────────────────────────────────────

    void ApplyMomentum()
    {
        transform.position += (Vector3)(_velocity * Time.deltaTime);
        _velocity *= momentumDrag;
    }

    void ApplyVisualRotation()
    {
        Quaternion target = Quaternion.Euler(0f, 0f, _currentAngle);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, 10f * Time.deltaTime);
    }

    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    // Left-facing sprite: forward = negative local X axis
    Vector2 GetForwardVector()
    {
        float rad = _currentAngle * Mathf.Deg2Rad;
        return new Vector2(-Mathf.Cos(rad), -Mathf.Sin(rad));
    }

    void PickNewWaypoint()
    {
        for (int i = 0; i < 15; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minWaypointDistance, maxWaypointDistance);

            Vector2 candidate = (Vector2)transform.position + new Vector2(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance
            );

            if (Vector2.Distance(candidate, _startPosition) <= wanderRadius)
            {
                _targetWaypoint = candidate;
                return;
            }
        }

        // Fallback: drift back toward origin
        _targetWaypoint = _startPosition + Random.insideUnitCircle * (wanderRadius * 0.4f);
    }

    // ─────────────────────────────────────────────
    //  Gizmos
    // ─────────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Vector2 origin = Application.isPlaying ? _startPosition : (Vector2)transform.position;

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.35f);
        DrawCircle2D(origin, wanderRadius);

        if (Application.isPlaying)
        {
            Gizmos.color = _isIdle ? Color.gray : Color.yellow;
            Gizmos.DrawSphere(_targetWaypoint, 0.2f);
            Gizmos.DrawLine(transform.position, _targetWaypoint);

            // Show actual forward direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, (Vector3)GetForwardVector() * 1.2f);
        }
    }

    void DrawCircle2D(Vector2 center, float radius)
    {
        int segments = 40;
        float step = 360f / segments * Mathf.Deg2Rad;
        for (int i = 0; i < segments; i++)
        {
            Vector3 a = (Vector3)center + new Vector3(Mathf.Cos(i * step) * radius, Mathf.Sin(i * step) * radius);
            Vector3 b = (Vector3)center + new Vector3(Mathf.Cos((i + 1) * step) * radius, Mathf.Sin((i + 1) * step) * radius);
            Gizmos.DrawLine(a, b);
        }
    }
}