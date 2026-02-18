using System.Collections.Generic;
using UnityEngine;

public class PathfindingEnemyTestSmooth : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("Repathing")]
    [SerializeField] private float repathInterval = 0.5f;
    [SerializeField] private float repathIfOffPathMoreThan = 1.25f;

    [Header("Path Rules (per enemy)")]
    [SerializeField] private bool canMoveDiagonal = false;
    [SerializeField] private bool preventCornerCutting = true;
    [SerializeField] private LayerMask blockedLayers;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("How close we must be to a waypoint to advance the index.")]
    [SerializeField] private float waypointReachDist = 0.12f;

    [Tooltip("How many waypoints ahead we aim at. Higher = smoother/rounder turns.")]
    [Range(0, 8)]
    [SerializeField] private int lookAheadWaypoints = 2;

    [Tooltip("How quickly the enemy can change direction. Higher = snappier, lower = smoother.")]
    [Range(1f, 30f)]
    [SerializeField] private float turnResponsiveness = 10f;

    [Header("Debug")]
    [SerializeField] private bool drawPathGizmos = true;

    private Transform _target;
    private PathfindingGridController _grid;
    private Rigidbody2D _rb;

    private List<Vector3> _path;
    private int _pathIndex;
    private float _nextRepathTime;
    private Vector3Int _lastTargetCell;

    // Smoothed velocity we steer toward desired direction
    private Vector2 _vel;

    void Start()
    {
        _grid = GameController.Instance.PathfindingGrid;
        _rb = GetComponent<Rigidbody2D>();

        if (_grid == null)
        {
            Debug.LogError("PathfindingEnemyTestSmooth: GameController.PathfindingGrid is null.");
            enabled = false;
            return;
        }

        FindTarget();
        _nextRepathTime = Time.time;
    }

    void Update()
    {
        if (_target == null)
        {
            FindTarget();
            return;
        }

        MaybeRepath();
    }

    void FixedUpdate()
    {
        FollowPathSmoothed();
    }

    private void FindTarget()
    {
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go == null) return;

        _target = go.transform;
        _lastTargetCell = _grid.WorldToCell(_target.position);
        RepathToTarget();
    }

    private void MaybeRepath()
    {
        bool timeToRepath = Time.time >= _nextRepathTime;

        Vector3Int targetCellNow = _grid.WorldToCell(_target.position);
        bool targetChangedCell = targetCellNow != _lastTargetCell;

        bool noPath = _path == null || _pathIndex >= _path.Count;

        bool offPath = false;
        if (_path != null && _pathIndex < _path.Count)
            offPath = Vector2.Distance(transform.position, _path[_pathIndex]) >= repathIfOffPathMoreThan;

        if (targetChangedCell || (timeToRepath && (noPath || offPath)))
        {
            _lastTargetCell = targetCellNow;
            RepathToTarget();
            _nextRepathTime = Time.time + repathInterval;
        }
    }

    private void RepathToTarget()
    {
        if (_target == null) return;

        bool ok = _grid.TryToFindPath(
            startWorld: transform.position,
            goalWorld: _target.position,
            blockedLayers: blockedLayers,
            canMoveDiagonal: canMoveDiagonal,
            preventCuttingCorners: preventCornerCutting,
            worldPath: out var newPath,
            maxExpandedNodes: 50000
        );

        if (!ok || newPath == null || newPath.Count == 0)
            return;

        _path = newPath;
        _pathIndex = 0;

        SkipWaypointsInCurrentCell();
        SkipReachedWaypoints();
    }

    private void FollowPathSmoothed()
    {
        if (_path == null || _pathIndex >= _path.Count)
        {
            // Smoothly slow down when no path
            _vel = Vector2.Lerp(_vel, Vector2.zero, turnResponsiveness * Time.fixedDeltaTime);
            ApplyMove(_vel);
            return;
        }

        SkipWaypointsInCurrentCell();
        SkipReachedWaypoints();
        if (_pathIndex >= _path.Count) return;

        // Pick a lookahead waypoint to aim at (smoother turning)
        int aimIndex = Mathf.Min(_pathIndex + lookAheadWaypoints, _path.Count - 1);
        Vector2 aimPoint = _path[aimIndex];

        Vector2 pos = _rb ? _rb.position : (Vector2)transform.position;
        Vector2 desiredDir = (aimPoint - pos);
        float dist = desiredDir.magnitude;

        if (dist > 0.0001f)
            desiredDir /= dist;
        else
            desiredDir = Vector2.zero;

        Vector2 desiredVel = desiredDir * moveSpeed;

        // Steering: gradually move current velocity toward desired velocity
        _vel = Vector2.Lerp(_vel, desiredVel, turnResponsiveness * Time.fixedDeltaTime);

        ApplyMove(_vel);
    }

    private void ApplyMove(Vector2 velocity)
    {
        Vector2 pos = _rb ? _rb.position : (Vector2)transform.position;
        Vector2 nextPos = pos + velocity * Time.fixedDeltaTime;

        if (_rb) _rb.MovePosition(nextPos);
        else transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
    }

    private void SkipWaypointsInCurrentCell()
    {
        if (_path == null) return;

        Vector3Int currentCell = _grid.WorldToCell(transform.position);
        while (_pathIndex < _path.Count)
        {
            Vector3Int wpCell = _grid.WorldToCell(_path[_pathIndex]);
            if (wpCell != currentCell) break;
            _pathIndex++;
        }
    }

    private void SkipReachedWaypoints()
    {
        while (_path != null && _pathIndex < _path.Count &&
               Vector2.Distance(transform.position, _path[_pathIndex]) <= waypointReachDist)
        {
            _pathIndex++;
        }
    }

    void OnDrawGizmos()
    {
        if (!drawPathGizmos) return;
        if (_path == null || _path.Count < 2) return;

        for (int i = 0; i < _path.Count - 1; i++)
            Gizmos.DrawLine(_path[i], _path[i + 1]);

        if (_pathIndex < _path.Count)
            Gizmos.DrawWireSphere(_path[_pathIndex], 0.12f);
    }
}
