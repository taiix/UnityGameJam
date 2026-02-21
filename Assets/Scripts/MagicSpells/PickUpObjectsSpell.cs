using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpObjectsSpell : MonoBehaviour
{
    [Header("Spell")]
    [SerializeField] private string spellName = "Levitas";

    [Header("References (Scene Objects)")]
    [SerializeField] private Transform wandTip;
    [SerializeField] private ParticleSystem beam;
    [SerializeField] private ParticleSystem secondBeamEffect;

    [Header("Aiming")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask rayMask = ~0;
    [SerializeField] private bool reverseBeamForward = false;

    [Header("Length Scale (ParticleSystemRenderer)")]
    [SerializeField] private float lengthScaleMultiplier = 1f;
    [SerializeField] private float minLengthScale = 0.01f;
    [SerializeField] private float overrideHoldDistance = 0f;

    [Header("Beam Smoothing (optional)")]
    [SerializeField] private bool smoothLength = true;
    [SerializeField] private float lengthSmoothSpeed = 20f;

    private Camera cam;
    private ParticleSystemRenderer beamRenderer;
    private ParticleSystemRenderer secondBeamEffectRenderer;

    private bool spellArmed;
    private PickupInteractable heldPickup;
    private float heldDistance;
    private float currentLengthScale;

    private void Awake()
    {
        cam = Camera.main;

        if (beam != null)
            beamRenderer = beam.GetComponent<ParticleSystemRenderer>();

        if(secondBeamEffect != null)
            secondBeamEffectRenderer = secondBeamEffect.GetComponent<ParticleSystemRenderer>();

        spellArmed = false;
        SetBeamActive(false);
    }

    private void OnEnable()
    {
        SpellCaster.OnSpellCast += HandleSpellCast;
    }

    private void OnDisable()
    {
        SpellCaster.OnSpellCast -= HandleSpellCast;
        ReleaseHeld();
        spellArmed = false;
        SetBeamActive(false);
    }

    private void HandleSpellCast(string castSpellName, Vector3 castPosition)
    {
        if (castSpellName != spellName)
            return;

        spellArmed = true;
        SetBeamActive(false);
    }

    private void Update()
    {
        if (!spellArmed)
            return;

        if (wandTip == null || beam == null)
            return;

        if (Mouse.current == null)
            return;

        bool isHoldingLmb = Mouse.current.leftButton.isPressed;

        if (!isHoldingLmb)
        {
            ReleaseHeld();
            SetBeamActive(false);
            return;
        }

        SetBeamActive(true);

        Ray ray = GetMouseRay();
        if (TryRaycast(ray, out var hit))
        {
            AimBeamToPoint(hit.point);

            if (heldPickup == null)
                TryPickUpFromHit(hit);
        }
        else
        {
            Vector3 end = ray.origin + ray.direction * maxDistance;
            AimBeamToPoint(end);
        }


        UpdateHeldPosition();
    }

    private void TryPickUpFromHit(RaycastHit hit)
    {
        var pickup = hit.collider != null ? hit.collider.GetComponentInParent<PickupInteractable>() : null;
        if (pickup == null)
            return;

        heldPickup = pickup;
        heldPickup.BeginHold();

        float dist = Vector3.Distance(cam.transform.position, hit.point);
        heldDistance = overrideHoldDistance > 0f ? overrideHoldDistance : dist;

        // Set initial holdDistance once, then let PickupInteractable scroll logic adjust it.
        TrySetPickupHoldDistance(heldPickup, heldDistance);
    }

    private void UpdateHeldPosition()
    {
        // Intentionally empty.
        // PickupInteractable.Update() handles scroll wheel (holdDistance) itself while isHeld == true.
        // If we keep forcing holdDistance here, we break scroll wheel functionality.
    }

    private void ReleaseHeld()
    {
        if (heldPickup == null)
            return;

        heldPickup.EndHold();
        heldPickup = null;
        heldDistance = 0f;
        spellArmed = false; 
    }

    private Ray GetMouseRay()
    {
        if (cam == null) cam = Camera.main;
        return cam.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    private bool TryRaycast(Ray ray, out RaycastHit hit)
    {
        // If not holding anything, normal raycast.
        if (heldPickup == null)
            return Physics.Raycast(ray, out hit, maxDistance, rayMask, QueryTriggerInteraction.Ignore);

        // If holding something, raycast-all and pick the first hit that is NOT the held object.
        var hits = Physics.RaycastAll(ray, maxDistance, rayMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0)
        {
            hit = default;
            return false;
        }

        // Sort by distance (RaycastAll doesn't guarantee order).
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i].collider;
            if (col == null) continue;

            // Skip any collider belonging to the held pickup.
            if (col.GetComponentInParent<PickupInteractable>() == heldPickup)
                continue;

            hit = hits[i];
            return true;
        }

        hit = default;
        return false;
    }

    private void AimBeamToPoint(Vector3 hitPoint)
    {
        Vector3 toHitWorld = hitPoint - wandTip.position;
        if (toHitWorld.sqrMagnitude < 0.000001f)
            toHitWorld = wandTip.forward;

        beam.transform.position = wandTip.position;

        Vector3 dir = toHitWorld.normalized;
        if (reverseBeamForward)
            dir = -dir;

        beam.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        if (beamRenderer != null )
        {
            float distance = toHitWorld.magnitude;
            float target = Mathf.Max(minLengthScale, distance * lengthScaleMultiplier);

            if (!smoothLength)
            {
                beamRenderer.lengthScale = target;
                secondBeamEffectRenderer.lengthScale = target;
            }
            else
            {
                currentLengthScale = Mathf.Lerp(currentLengthScale, target, Time.deltaTime * lengthSmoothSpeed);
                beamRenderer.lengthScale = currentLengthScale;
                secondBeamEffectRenderer.lengthScale = currentLengthScale;
            }
        }
    }
    private void TrySetPickupHoldDistance(PickupInteractable pickup, float distance)
    {
        if (pickup == null) return;

        var field = typeof(PickupInteractable).GetField("holdDistance", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
            field.SetValue(pickup, distance);
    }

    private void SetBeamActive(bool active)
    {
        if (beam == null)
            return;

        if (active)
        {
            if (!beam.gameObject.activeSelf)
                beam.gameObject.SetActive(true);

            if (!beam.isPlaying)
            {
                beam.Clear(true);
                beam.Play(true);

                // Reset smoothing state
                currentLengthScale = beamRenderer != null ? beamRenderer.lengthScale : 0f;
            }
        }
        else
        {
            if (beam.gameObject.activeSelf)
            {
                beam.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                beam.gameObject.SetActive(false);
            }
        }
    }
}
