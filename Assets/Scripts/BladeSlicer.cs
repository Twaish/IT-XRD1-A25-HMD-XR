using System;
using UnityEngine;

public class BladeSlicer : MonoBehaviour {
  [SerializeField] private string sliceableTag = "Sliceable";

  public event Action<GameObject> OnSlice;

  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag(sliceableTag)) {
      Slice(other);
    }
  }

  protected virtual void Slice(Collider sliceable) {
    Debug.Log($"{name} sliced {sliceable.name}!");

    OnSlice?.Invoke(sliceable.gameObject);
  }
}
