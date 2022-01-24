using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float keyboardPanSpeed = 10f;
    [SerializeField] private float mousePanSpeed = 4f;
    [SerializeField] private float mouseScrollSpeed = 40f;

    [SerializeField] private float minX = 1f;
    [SerializeField] private float maxX = 7f;

    [SerializeField] private float minY = -1f;
    [SerializeField] private float maxY = 5f;

    [SerializeField] private float minHeight = 3f;
    [SerializeField] private float maxHeight = 7f;

    private Vector3 startPos;

    private bool isBeingHeld = false;

    private void Update()
    {
        HandleMouseMovement();
        HandleMouseScroll();
        ClampWorldBoundaries();
    }

    private void ClampWorldBoundaries()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minY, maxY);
        pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);

        transform.position = pos;
    }

    private void HandleMouseMovement()
    {
        // OnMouseDown, onMouseUp and onMouseDrag were not used due to the
        // dependency on collider and rigidbodies to work properly
        if (Input.GetMouseButton(0))
        {
            if (!isBeingHeld)
            {
                startPos = transform.position + GetMousePosition() * mousePanSpeed;
                isBeingHeld = true;
            }
            else
            {
                transform.position = startPos - GetMousePosition() * mousePanSpeed;
            }
        }
        else if (isBeingHeld)
        {
            isBeingHeld = false;
        }
    }

    private void HandleMouseScroll()
    {
        Vector3 pos = transform.position;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y += scroll * mouseScrollSpeed * Time.deltaTime;

        transform.position = pos;
    }

    private void HandleKeyboardMovement()
    {
        Vector3 pos = transform.position;

        float horizontalMove = Input.GetAxis("Horizontal");
        pos.x += horizontalMove * keyboardPanSpeed * Time.deltaTime;

        float verticalMove = Input.GetAxis("Vertical");
        pos.z += verticalMove * keyboardPanSpeed * Time.deltaTime;

        transform.position = pos;
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePos =  Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mousePos = Camera.main.ScreenToViewportPoint(mousePos);

        return new Vector3(mousePos.x, 0, mousePos.y);
    }
}
