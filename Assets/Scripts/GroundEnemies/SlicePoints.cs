using UnityEngine;

[RequireComponent(typeof(BladeSlicer))]
public class SlicePoints : MonoBehaviour
{
    private BladeSlicer bladeSlicer;

    private void Start() 
    {
        bladeSlicer = GetComponent<BladeSlicer>();
        bladeSlicer.OnSlice += HandleSlice;
    }

    private void HandleSlice(GameObject target, Vector3 _)
    {
        // Janky but works
        if (target.TryGetComponent(out Enemies enemy))
        {
            enemy.Die();
        }
        else if (target.TryGetComponent(out Drone drone))
        {
            drone.Die();
        }
    }
}
