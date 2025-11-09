using UnityEngine;

public enum SurfaceType
{
    Default,
    Wood,
    Stone,
    Carpet
}

[DisallowMultipleComponent]
public class SurfaceAudio : MonoBehaviour
{
    [Tooltip("Surface type used by FootstepAudio when this collider is hit.")]
    public SurfaceType surfaceType = SurfaceType.Default;
}