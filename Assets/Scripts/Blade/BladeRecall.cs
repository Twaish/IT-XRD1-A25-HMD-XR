using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class BladeRecall : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform of the player's right-hand controller.")]
    public Transform rightHand;

    [Tooltip("XR Direct Interactor on the right-hand controller.")]
    public XRDirectInteractor rightHandInteractor;

    [Tooltip("Input action bound to R3 (right stick click) to recall the blade.")]
    public InputActionProperty recallAction;

    [Header("Recall Settings")]
    public float recallForce = 35f;
    public float rotateSpeed = 10f;
    public float grabDistance = 0.25f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isRecalling;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (rightHandInteractor == null)
        {
            rightHandInteractor = FindFirstObjectByType<XRDirectInteractor>();
            Debug.Log("[BladeRecall] RightHandInteractor auto-assigned: " + (rightHandInteractor != null));
        }

        Debug.Log("[BladeRecall] Awake complete. Rigidbody and GrabInteractable assigned.");
    }

    void OnEnable()
    {
        recallAction.action.performed += OnRecallPressed;
        recallAction.action.Enable();
        Debug.Log("[BladeRecall] Recall action enabled.");
    }

    void OnDisable()
    {
        recallAction.action.performed -= OnRecallPressed;
        recallAction.action.Disable();
        Debug.Log("[BladeRecall] Recall action disabled.");
    }

    void OnRecallPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("[BladeRecall] Recall pressed. IsSelected: " + grabInteractable.isSelected);

        if (!grabInteractable.isSelected)
        {
            isRecalling = true;
            Debug.Log("[BladeRecall] Blade recall started.");
        }
        else
        {
            Debug.Log("[BladeRecall] Blade is currently held. Recall ignored.");
        }
    }

    void FixedUpdate()
    {
        if (!isRecalling)
            return;

        if (rightHand == null)
        {
            Debug.LogWarning("[BladeRecall] Right hand not assigned!");
            return;
        }

        Vector3 direction = (rightHand.position - transform.position).normalized;
        rb.linearVelocity = direction * recallForce;

        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * rotateSpeed);

        float distance = Vector3.Distance(transform.position, rightHand.position);
        Debug.Log("[BladeRecall] Recalling... Distance to hand: " + distance);

        if (distance < grabDistance)
        {
            Debug.Log("[BladeRecall] Blade reached hand.");
            CatchInHand();
        }
    }

    void CatchInHand()
    {
        isRecalling = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = rightHand.position;
        transform.rotation = rightHand.rotation;

        if (rightHandInteractor != null && grabInteractable != null)
        {
            rightHandInteractor.StartManualInteraction(grabInteractable as IXRSelectInteractable);
            Debug.Log("[BladeRecall] Manual interaction started.");
        }
        else
        {
            Debug.LogWarning("[BladeRecall] Cannot start manual interaction. Check references.");
        }

        if (rightHandInteractor != null)
        {
            rightHandInteractor.SendHapticImpulse(0.6f, 0.1f);
            Debug.Log("[BladeRecall] Haptic impulse sent.");
        }
    }
}
