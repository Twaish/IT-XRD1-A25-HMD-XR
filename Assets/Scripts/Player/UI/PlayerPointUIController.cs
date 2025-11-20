using TMPro;
using UnityEngine;


public class PlayerPointUIController : MonoBehaviour
{
    [SerializeField]
    private PointSystem pointSystem;

    [SerializeField]
    private TextMeshProUGUI pointText;

    private void Start()
    {
        pointSystem.OnPointsChanged += UpdatePoints;
    }

    private void UpdatePoints(int points)
    {
        pointText.text = "" + Mathf.RoundToInt(points);
    }
}
