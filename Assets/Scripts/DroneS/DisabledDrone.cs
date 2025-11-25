using UnityEngine;

public class DisabledDrone : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second (positive = clockwise when viewed from above)")]
    public float rotationSpeed = 90f;

    void Update()
    {
        // Rotate around itself
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
