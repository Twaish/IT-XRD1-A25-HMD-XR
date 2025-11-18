using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HomingThrowActivator : MonoBehaviour
{
    private SlightHomingThrow homing;

    void Awake()
    {
        homing = GetComponent<SlightHomingThrow>();
        var grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (!args.isCanceled)
        {
            homing.ActivateHoming();
        }
    }

    void OnDestroy()
    {
        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }
}