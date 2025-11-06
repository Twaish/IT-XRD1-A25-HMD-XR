using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
public class DeflectSfxPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BladeDeflect smallDeflectZone;
    [SerializeField] private BladeDeflect bigDeflectZone;

    private SoundPlayer soundPlayer;

    private void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        smallDeflectZone.OnDeflect += HandleDeflect;
        bigDeflectZone.OnDeflect += HandleDeflect;
    }

    private void HandleDeflect()
    {
        soundPlayer.PlaySound("deflect");
    }
}
