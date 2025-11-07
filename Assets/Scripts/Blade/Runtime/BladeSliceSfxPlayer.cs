using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
[RequireComponent(typeof(BladeSlicer))]
public class BladeSliceSfxPlayer : MonoBehaviour
{
    private BladeSlicer bladeSlicer;
    private SoundPlayer soundPlayer;

    private void Start()
    {
        bladeSlicer = GetComponent<BladeSlicer>();
        soundPlayer = GetComponent<SoundPlayer>();
        bladeSlicer.OnSlice += HandleSlice;
    }

    private void HandleSlice(GameObject slicedTarget, Vector3 velocity)
    {
        soundPlayer.PlaySound("slice");
    }
}
