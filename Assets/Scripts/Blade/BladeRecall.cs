using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody))]
public class LightsaberRecall : MonoBehaviour
{
    [Header("References")]
    public Transform rightHand; 
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor rightHandInteractor;
    public InputActionProperty recallAction; 

    [Header("Recall Settings")]
    public float recallForce = 35f;
    public float rotateSpeed = 10f;
    public float grabDistance = 0.25f;

    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isRecalling;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        recallAction.action.performed += OnRecallPressed;
        recallAction.action.Enable();
    }

    void OnDisable()
    {
        recallAction.action.performed -= OnRecallPressed;
        recallAction.action.Disable();
    }

    void OnRecallPressed(InputAction.CallbackContext ctx)
    {
        if (!grabInteractable.isSelected)
            isRecalling = true;
    }

    void FixedUpdate()
    {
        if (!isRecalling || rightHand == null)
            return;

        Vector3 direction = (rightHand.position - transform.position).normalized;
        rb.linearVelocity = direction * recallForce;

        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * rotateSpeed);

        if (Vector3.Distance(transform.position, rightHand.position) < grabDistance)
        {
            isRecalling = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.position = rightHand.position;
            transform.rotation = rightHand.rotation;

            if (rightHandInteractor != null && grabInteractable != null)
            {
                rightHandInteractor.StartManualInteraction((UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grabInteractable);
            }
        }
    }
}
