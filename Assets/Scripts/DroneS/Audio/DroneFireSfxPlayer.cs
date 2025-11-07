using UnityEngine;

[RequireComponent(typeof(Drone))]
[RequireComponent(typeof(SoundPlayer))]
public class DroneFireSfxPlayer : MonoBehaviour
{
    private SoundPlayer soundPlayer;
    private Drone drone;

    private void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        drone = GetComponent<Drone>();
        drone.OnFire += HandleFire;
    }

    private void HandleFire()
    {
        soundPlayer.PlaySound("fire");
    }
}
