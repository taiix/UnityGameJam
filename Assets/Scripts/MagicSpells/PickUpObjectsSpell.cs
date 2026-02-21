using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpObjectsSpell : MonoBehaviour
{
    [Header("Spell")]
    [SerializeField] private string spellName = "Levitas";

    [Header("References (Scene Objects)")]
    [SerializeField] private Transform wandTip;
    [SerializeField] private ParticleSystem beam; 

    [Header("Aiming")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask rayMask = ~0;
    [SerializeField] private bool reverseBeamForward = false; // enable if your beam points backwards

    [Header("Length Scale (ParticleSystemRenderer)")]
    [SerializeField] private float lengthScaleMultiplier = 1f;
    [SerializeField] private float minLengthScale = 0.01f;

    private Camera cam;
    private ParticleSystemRenderer beamRenderer;

    private bool spellArmed;

    private void Awake()
    {
        cam = Camera.main;

        if (beam != null)
            beamRenderer = beam.GetComponent<ParticleSystemRenderer>();

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
        spellArmed = false;
        SetBeamActive(false);
    }

    private void HandleSpellCast(string castSpellName, Vector3 castPosition)
    {
        if (castSpellName != spellName)
            return;

        // Arm the spell; beam will show while LMB is held.
        spellArmed = true;

        // Ensure beam is reset/off until the player actually holds LMB.
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

        // After spell is cast, hold LMB to show/aim beam.
        bool shouldBeamBeActive = Mouse.current.leftButton.isPressed;

        if (!shouldBeamBeActive)
        {
            SetBeamActive(false);
            return;
        }

        SetBeamActive(true);
        AimBeamAtMouse();
    }

    private void AimBeamAtMouse()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        Vector3 hitPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, rayMask, QueryTriggerInteraction.Ignore))
            hitPoint = hit.point;
        else
            hitPoint = ray.origin + ray.direction * maxDistance;

        Vector3 toHitWorld = hitPoint - wandTip.position;
        if (toHitWorld.sqrMagnitude < 0.000001f)
            toHitWorld = wandTip.forward;

        // Keep beam anchored at wand tip (beam should already be parented; this enforces origin).
        beam.transform.position = wandTip.position;

        // Rotate beam to face target.
        Vector3 dir = toHitWorld.normalized;
        if (reverseBeamForward)
            dir = -dir;

        beam.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        // Adjust renderer length scale.
        if (beamRenderer != null)
        {
            float distance = toHitWorld.magnitude;
            beamRenderer.lengthScale = Mathf.Max(minLengthScale, distance * lengthScaleMultiplier);
        }
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
