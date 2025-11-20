using UnityEngine;

// https://github.com/Twaish/Go-Marble/blob/main/Assets/Scripts/Utilities/SelfDestruct.cs
public class SelfDestruct : MonoBehaviour {
  [SerializeField] private float lifetime = 5f;

  private void OnEnable() {
    Invoke(nameof(DestroySelf), lifetime);
  }

  private void DestroySelf() {
    Destroy(gameObject);
  }
}