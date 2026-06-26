using UnityEngine;

// Plays a "waking up" intro: locks the player, starts looking down, then animates
// the eyelids open and the head lifting to a normal forward view at the same time.
public class WakeUpSequence : MonoBehaviour
{
    [SerializeField] private EyelidWipe eyelids;
    [SerializeField] private CharacterMovement characterMovement;

    [SerializeField] private float duration = 2.5f;
    [SerializeField] private float startPitch = 55f;
    [SerializeField] private float startCameraHeight = 0.3f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool playOnStart = true;

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play()
    {
        if (eyelids != null)
        {
            eyelids.SetOpenAmount(0f);
        }

        if (characterMovement != null)
        {
            characterMovement.SetPitch(startPitch);
            characterMovement.SetCameraHeight(startCameraHeight);
            characterMovement.DisableControls();
            characterMovement.AnimatePitch(0f, duration, curve, disableControlsAfter: false,
                onComplete: () => characterMovement.EnableControls());
            characterMovement.AnimateCameraHeight(characterMovement.CameraRestHeight, duration, curve);
        }

        eyelids?.Open(duration, curve);
    }
}
