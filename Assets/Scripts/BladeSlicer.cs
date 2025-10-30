using System;
using UnityEngine;
using EzySlice;

// https://www.youtube.com/watch?v=GQzW6ZJFQ94
public class BladeSlicer : MonoBehaviour
{
  [SerializeField] private Material crossSectionMaterial;
  [SerializeField] private LayerMask sliceableLayer;
  [SerializeField] private Transform startSlicePoint;
  [SerializeField] private Transform endSlicePoint;
  [SerializeField] private VelocityEstimator velocityEstimator;
  [SerializeField] private float cutForce = 2000;

  public event Action<GameObject, Vector3> OnSlice;

  private void FixedUpdate()
  {
    bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
    if (hasHit)
    {
      GameObject target = hit.transform.gameObject;
      Slice(target);
    }
  }

  protected virtual void Slice(GameObject target)
  {
    Vector3 velocity = velocityEstimator.GetVelocityEstimate();
    Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
    SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal.normalized);

    if (hull != null)
    {
      GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
      SetupSlicedComponent(upperHull);
      GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
      SetupSlicedComponent(lowerHull);
      Destroy(target);
    }

    Debug.Log($"{name} sliced {target.name}!");
  }

  public void SetupSlicedComponent(GameObject hull)
  {
    Rigidbody rigidbody = hull.AddComponent<Rigidbody>();
    MeshCollider collider = hull.AddComponent<MeshCollider>();
    collider.convex = true;
    rigidbody.AddExplosionForce(cutForce, hull.transform.position, 1);
  }
}
