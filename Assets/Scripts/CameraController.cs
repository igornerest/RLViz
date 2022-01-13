using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 10f;
    [SerializeField] private float scrollSpeed = 40f;

    [SerializeField] private Vector2 panLimit = new Vector2(2,2);
    [SerializeField] private float minHeight = 3f;
    [SerializeField] private float maxHeight = 10f;

    void Update()
    {
        Vector3 pos = transform.position;

        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        pos.x += horizontalMove * panSpeed * Time.deltaTime;
        pos.z += verticalMove * panSpeed * Time.deltaTime;
        pos.y += scroll * scrollSpeed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);

        transform.position = pos;
    }
}
