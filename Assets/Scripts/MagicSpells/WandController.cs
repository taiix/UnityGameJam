using UnityEngine;

public class WandController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode flickKey = KeyCode.Mouse1;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 1.5f;
    public float maxRotationAngle = 30f;
    public float rotationSpeed = 10f;

    [Header("Position Sway")]
    public float positionSwayAmount = 0.01f;
    public float positionSwaySmooth = 5f;

    private Quaternion originalLocalRotation;
    private Vector3 originalLocalPosition;
    private Vector3 lastMousePosition;

    void Start()
    {
        originalLocalRotation = transform.localRotation;
        originalLocalPosition = transform.localPosition;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (Input.GetKey(flickKey))
        {
            Vector3 mouseDelta = (Input.mousePosition - lastMousePosition) * mouseSensitivity;
            lastMousePosition = Input.mousePosition;

            // Inverted axes for natural feel (Y affects X rotation, X affects Z rotation)
            float zRotation = Mathf.Clamp(-mouseDelta.y, -maxRotationAngle, maxRotationAngle);
            float xRotation = Mathf.Clamp(mouseDelta.x, -maxRotationAngle, maxRotationAngle);

            Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, zRotation);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalLocalRotation * targetRotation, Time.deltaTime * rotationSpeed);

            // Position sway offset (slight offset opposite to mouse movement)
            Vector3 swayOffset = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f) * positionSwayAmount;
            Vector3 targetPosition = originalLocalPosition + swayOffset;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * positionSwaySmooth);
        }
        else
        {
            // Smoothly return to original position and rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, originalLocalRotation, Time.deltaTime * rotationSpeed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalLocalPosition, Time.deltaTime * positionSwaySmooth);
            lastMousePosition = Input.mousePosition; // Reset delta when exiting flick
        }
    }
}
