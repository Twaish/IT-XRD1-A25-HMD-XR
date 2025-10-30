using System;
using UnityEngine;
using EzySlice;
using EzyPlane = EzySlice.Plane;

public class BladeSlicer : MonoBehaviour {
  [SerializeField] private string sliceableTag = "Sliceable";
  [SerializeField] private Material crossSectionMaterial;

  public event Action<GameObject, Vector3> OnSlice;

  private Vector3 lastPosition;
  private Vector3 velocity;

  private void Update() {
    velocity = (transform.position - lastPosition) / Time.deltaTime;
    lastPosition = transform.position;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag(sliceableTag))
    {
      Slice(other);
    }
  }

  private void ShowSlicePlane(Vector3 point, Vector3 normal) {
    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
    plane.transform.position = point;
    plane.transform.rotation = Quaternion.LookRotation(normal);
    plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    var mat = new Material(Shader.Find("Unlit/Color"));
    mat.color = new Color(0, 1, 0, 0.4f);
    plane.GetComponent<Renderer>().material = mat;
    Destroy(plane, 0.5f);
  }

  protected virtual void Slice(Collider sliceable) {
    Vector3 sliceDirection = velocity.normalized;
    Vector3 contactPoint = sliceable.ClosestPoint(transform.position);
    EzyPlane slicePlane = new(sliceDirection, contactPoint);

    Debug.DrawRay(contactPoint, sliceDirection * 0.3f, Color.green, 1f);

    SlicedHull hull = sliceable.gameObject.Slice(contactPoint, sliceDirection, crossSectionMaterial);

    ShowSlicePlane(contactPoint, sliceDirection);
    Debug.Log(velocity.normalized);

    if (hull != null) {
      GameObject upperHull = hull.CreateUpperHull(sliceable.gameObject, crossSectionMaterial);
      GameObject lowerHull = hull.CreateLowerHull(sliceable.gameObject, crossSectionMaterial);

      upperHull.transform.position = sliceable.transform.position;
      lowerHull.transform.position = sliceable.transform.position;

      // upperHull.AddComponent<Rigidbody>();
      // lowerHull.AddComponent<Rigidbody>();

      Destroy(sliceable.gameObject);

      OnSlice?.Invoke(upperHull, sliceDirection);
      OnSlice?.Invoke(lowerHull, sliceDirection);
    } else {
      Debug.LogWarning($"Failed to slice {sliceable.name}");
    }

    Debug.Log($"{name} sliced {sliceable.name} with direction {sliceDirection}!");

    OnSlice?.Invoke(sliceable.gameObject, sliceDirection);
  }
}
