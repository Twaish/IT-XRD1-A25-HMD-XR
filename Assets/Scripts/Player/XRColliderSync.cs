using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class XRCapsuleController : MonoBehaviour
{
    public CapsuleCollider capsule;       
    public Transform cameraTransform;     
    
    public float minHeight = 1.0f;       
    public float maxHeight = 2.0f;        
    public float skinWidth = 0.05f;       

    void Reset()
    {
        if (capsule == null)
            capsule = GetComponent<CapsuleCollider>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (capsule == null || cameraTransform == null)
            return;

        float headHeight = Mathf.Clamp(cameraTransform.localPosition.y, minHeight, maxHeight);

        capsule.height = headHeight;

        Vector3 localCamPos = cameraTransform.localPosition;
        capsule.center = new Vector3(localCamPos.x, headHeight / 2f + skinWidth, localCamPos.z);
    }
}
