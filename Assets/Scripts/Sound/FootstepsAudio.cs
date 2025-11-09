using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FootstepsAudio : MonoBehaviour
{
    [Serializable]
    public class SurfaceClips
    {
        public SurfaceType surface = SurfaceType.Default;
        public AudioClip[] clips;
    }

    [Header("Audio")]
    [Tooltip("AudioSource used to play footstep sounds. Will be created if not assigned.")]
    public AudioSource audioSource;
    [Range(0f, 1f)] public float volume = 0.9f;
    [Range(0f, 1f)] public float spatialBlend = 1f;
    [Tooltip("Random pitch variation applied per step.")]
    [Range(0f, 0.5f)] public float pitchJitter = 0.07f;

    [Header("Surface Detection")]
    [Tooltip("Optional origin for the ground ray (e.g., a foot bone). Uses this transform if not set.")]
    public Transform rayOrigin;
    [Tooltip("How far down to raycast to find the ground.")]
    public float rayDistance = 1.5f;
    [Tooltip("Layers considered ground.")]
    public LayerMask groundMask = ~0;

    [Header("Clips per Surface")]
    public List<SurfaceClips> surfaces = new List<SurfaceClips>();
    [Tooltip("Fallback when no surface is detected.")]
    public SurfaceType defaultSurface = SurfaceType.Default;

    // Optional: quick mapping for Tag -> SurfaceType (if you prefer tags over the SurfaceAudio component)
    [Header("Optional Tag Mapping (fallback)")]
    public string woodTag = "Wood";
    public string stoneTag = "Stone";
    public string carpetTag = "Carpet";

    Dictionary<SurfaceType, AudioClip[]> _map;

    void Awake()
    {
        BuildMap();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        audioSource.spatialBlend = spatialBlend;
    }

    void OnValidate()
    {
        if (audioSource != null) audioSource.spatialBlend = spatialBlend;
    }

    void BuildMap()
    {
        _map = new Dictionary<SurfaceType, AudioClip[]>();
        foreach (var s in surfaces)
        {
            if (s == null || s.clips == null) continue;
            _map[s.surface] = s.clips;
        }
    }

    // Call this from an Animation Event on foot down
    public void OnFootstep()
    {
        var surface = DetectSurfaceType(out bool grounded);
        if (!grounded) return;

        var clip = GetRandomClip(surface);
        if (clip == null) return;

        var basePitch = 1f + UnityEngine.Random.Range(-pitchJitter, pitchJitter);
        var oldPitch = audioSource.pitch;
        audioSource.pitch = basePitch;
        audioSource.PlayOneShot(clip, volume);
        audioSource.pitch = oldPitch;
    }

    SurfaceType DetectSurfaceType(out bool grounded)
    {
        grounded = false;

        Vector3 origin = (rayOrigin ? rayOrigin.position : transform.position) + Vector3.up * 0.05f;
        if (Physics.Raycast(origin, Vector3.down, out var hit, rayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            grounded = true;

            // Prefer explicit component on the collider
            if (hit.collider.TryGetComponent<SurfaceAudio>(out var sa))
                return sa.surfaceType;

            // Or look on the hit object's hierarchy
            var saInParent = hit.collider.GetComponentInParent<SurfaceAudio>();
            if (saInParent != null)
                return saInParent.surfaceType;

            // Optional tag fallback
            var tag = hit.collider.tag;
            if (!string.IsNullOrEmpty(tag))
            {
                if (tag == woodTag) return SurfaceType.Wood;
                if (tag == stoneTag) return SurfaceType.Stone;
                if (tag == carpetTag) return SurfaceType.Carpet;
            }
        }

        return defaultSurface;
    }

    AudioClip GetRandomClip(SurfaceType surface)
    {
        if (_map == null || _map.Count == 0) BuildMap();

        if (!_map.TryGetValue(surface, out var clips) || clips == null || clips.Length == 0)
        {
            // Fallback to default
            if (_map.TryGetValue(defaultSurface, out var def) && def != null && def.Length > 0)
                return def[UnityEngine.Random.Range(0, def.Length)];
            return null;
        }

        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }
}