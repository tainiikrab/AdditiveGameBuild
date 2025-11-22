using System;
using UnityEngine;

public enum PathType
{
    ToLaptop,
    ToPrinter,
    FromLaptop
}

[Serializable]
public class WayPointPath
{
    public PathType Type;
    public GameObject[] WayPoints;
    public bool needStop;
    public Action OnPathEnd;
}

public class WayPointFollower : MonoBehaviour
{
    [Header("Paths")] public WayPointPath[] Paths;

    [Header("Movement Settings")] public float Speed = 3f;
    public float RotationSpeed = 5f;

    [Header("Rotation Offset (degrees)")] public Vector3 rotationOffsetEuler = new(0f, -90f, 0f);

    [Header("Debug Info")] public int currentPathIndex;
    public int currentWPIndex;

    private bool isStopped;
    private bool isWaitingForAction;
    private Animation animation;

    private void Awake()
    {
        isWaitingForAction = true;
    }

    private void Start()
    {
        animation = GetComponentInChildren<Animation>();

        if (animation != null && animation.GetClip("Idle") != null)
            animation.Play("Idle");
    }

    private void Update()
    {
        if (isStopped)
        {
            PlayAnimation("Idle");
            return;
        }

        if (Paths.Length == 0 || currentPathIndex >= Paths.Length) return;
        var currentPath = Paths[currentPathIndex];
        if (currentPath.WayPoints.Length == 0) return;

        var target = currentPath.WayPoints[currentWPIndex].transform;

        var direction = (target.position - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);

        var rotationOffset = Quaternion.Euler(rotationOffsetEuler);
        var adjustedRotation = lookRotation * rotationOffset;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            adjustedRotation,
            RotationSpeed * Time.deltaTime * 50
        );

        var prevDistance = Vector3.Distance(transform.position, target.position);

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            Speed * Time.deltaTime
        );

        var newDistance = Vector3.Distance(transform.position, target.position);

        if (newDistance < prevDistance - 0.001f)
            PlayAnimation("Walk");
        else
            PlayAnimation("Idle");

        if (HasReachedWayPoint(target))
            HandleWayPointReached();
    }

    private void PlayAnimation(string clipName)
    {
        if (animation == null) return;
        if (!animation.isPlaying || animation.clip == null || animation.clip.name != clipName)
        {
            if (animation.GetClip(clipName) != null)
                animation.Play(clipName);
            else
                Debug.LogWarning($"[WayPointFollower] �� ������ ������������ ����: {clipName}");
        }
    }

    private bool HasReachedWayPoint(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) < 0.1f;
    }

    private void HandleWayPointReached()
    {
        var currentPath = Paths[currentPathIndex];

        if (currentWPIndex >= currentPath.WayPoints.Length - 1)
        {
            if (currentPath.needStop)
            {
                isStopped = true;
                isWaitingForAction = true;
                currentPath.OnPathEnd?.Invoke();
                PlayAnimation("Idle");
                return;
            }

            ContinueToNextPath();
        }
        else
        {
            currentWPIndex++;
        }
    }

    private void ContinueToNextPath()
    {
        currentWPIndex = 0;
        currentPathIndex++;

        if (currentPathIndex >= Paths.Length)
            currentPathIndex = 0;
    }

    public void ContinueMovement()
    {
        if (isStopped && isWaitingForAction)
        {
            isStopped = false;
            isWaitingForAction = false;

            currentWPIndex = 0;
            currentPathIndex++;

            if (currentPathIndex >= Paths.Length)
                currentPathIndex = 0;

            PlayAnimation("Walk");
            Debug.Log("Продолжаем движение после остановки");
        }
    }

    public void StopMovement()
    {
        isStopped = true;
        PlayAnimation("Idle");
    }

    public void StartMovement()
    {
        if (Paths.Length == 0) return;

        if (currentPathIndex >= Paths.Length)
            currentPathIndex = 0;

        isStopped = false;
        isWaitingForAction = false;
        PlayAnimation("Walk");
    }

    public bool IsStopped()
    {
        return isStopped;
    }

    public bool IsWaitingForAction()
    {
        return isWaitingForAction;
    }

    public WayPointPath GetCurrentPath()
    {
        if (currentPathIndex < Paths.Length)
            return Paths[currentPathIndex];
        return null;
    }

    public void AddOnPathEndAction(int pathIndex, Action action)
    {
        if (pathIndex < 0 || pathIndex >= Paths.Length)
        {
            Debug.LogWarning($"[WayPointFollower] Incorrect index: {pathIndex}");
            return;
        }

        if (action == null)
        {
            Debug.LogWarning("[WayPointFollower] Empty action can't be added'.");
            return;
        }

        Paths[pathIndex].OnPathEnd += action;
    }

    public void ClearOnPathEndActions(int pathIndex)
    {
        if (pathIndex < 0 || pathIndex >= Paths.Length)
        {
            Debug.LogWarning($"[WayPointFollower] Incorrect path index: {pathIndex}");
            return;
        }

        Paths[pathIndex].OnPathEnd = null;
    }

    public void SwitchPath(PathType type)
    {
        for (var i = 0; i < Paths.Length; i++)
            if (Paths[i].Type == type)
            {
                currentPathIndex = i;
                currentWPIndex = 0;
                isStopped = false;
                isWaitingForAction = false;
                PlayAnimation("Walk");

                Debug.Log($"[WayPointFollower] Switched to path with type: {type}");
                return;
            }

        Debug.LogWarning($"[WayPointFollower] Not found path of type {type}");
    }
}