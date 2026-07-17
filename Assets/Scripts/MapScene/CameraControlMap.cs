using UnityEngine;
using UnityEngine.InputSystem;

public class MapCameraPan : MonoBehaviour
{
    [Header("Pan")]
    public float panSpeed = 12f;

    [Header("Bounds")]
    public float minX = -20f;
    public float maxX = 20f;
    public float minZ = -20f;
    public float maxZ = 20f;

    [Header("Startup Focus")]
    public bool startAbovePlayer = true;
    public Transform playerTransform;
    public Transform fallbackTarget;

    private float fixedY = 10f;

    void Start()
    {
        fixedY = transform.position.y;

        if (!startAbovePlayer)
            return;

        Transform target = fallbackTarget;

        if (target == null && PathfindingManager.Instance != null)
        {
            if (playerTransform != null)
                target = playerTransform;
            else if (PathfindingManager.Instance.startNode != null)
                target = PathfindingManager.Instance.startNode.transform;
        }

        if (target != null)
        {
            Vector3 pos = transform.position;
            pos.x = target.position.x;
            pos.z = target.position.z;
            pos.y = fixedY;
            transform.position = ClampPosition(pos);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        Vector3 move = Vector3.zero;

        if (Keyboard.current.leftArrowKey.isPressed)
            move.x -= 1f;
        if (Keyboard.current.rightArrowKey.isPressed)
            move.x += 1f;
        if (Keyboard.current.downArrowKey.isPressed)
            move.z -= 1f;
        if (Keyboard.current.upArrowKey.isPressed)
            move.z += 1f;

        if (move == Vector3.zero) return;

        move.Normalize();

        Vector3 nextPosition = transform.position + move * panSpeed * Time.deltaTime;
        nextPosition.y = fixedY;

        transform.position = ClampPosition(nextPosition);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.z = Mathf.Clamp(position.z, minZ, maxZ);
        return position;
    }
}