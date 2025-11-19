using UnityEngine;

public class XRColliderSync : MonoBehaviour
{
    public CapsuleCollider capsule;
    public Transform cameraTransform;

    void Update()
    {
        float headHeight = Mathf.Clamp(cameraTransform.localPosition.y, 0.5f, 2.0f);

        capsule.height = headHeight;
        capsule.center = new Vector3(cameraTransform.localPosition.x, headHeight / 2f, cameraTransform.localPosition.z);
    }
}
