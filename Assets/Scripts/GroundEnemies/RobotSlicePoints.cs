using UnityEngine;

[RequireComponent(typeof(BladeSlicer))]
public class RobotSlicePoints : MonoBehaviour
{
    private BladeSlicer bladeSlicer;

    private void Start() 
    {
        bladeSlicer = GetComponent<BladeSlicer>();
        bladeSlicer.OnSlice += HandleSlice;
    }

    private void HandleSlice(GameObject target, Vector3 _)
    {
        if (target.TryGetComponent(out Enemies enemy))
        {
            enemy.Die();
        }
    }
}
