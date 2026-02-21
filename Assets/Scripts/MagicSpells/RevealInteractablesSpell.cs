using UnityEngine;

public class RevealInteractablesSpell : MonoBehaviour
{
    [Header("Revelio Settings")]
    [SerializeField] private string revelioSpellName = "Revelio";
    [SerializeField] private float range = 12f;
    [SerializeField] private float duration = 15f;
    [SerializeField] private Material highlightMaterial;

    [Header("Filtering")]
    [Tooltip("If your Interactable.Awake sets layer = 6, set this to layer 6 for fast filtering.")]
    [SerializeField] private LayerMask interactableMask;

    [Tooltip("Center for the scan. Usually the player transform or camera.")]
    [SerializeField] private Transform scanOrigin;

    private void OnEnable()
    {
        SpellCaster.OnSpellCast += HandleSpellCast;
    }

    private void OnDisable()
    {
        SpellCaster.OnSpellCast -= HandleSpellCast;
    }

    private void HandleSpellCast(string spellName, Vector3 position)
    {
        if (spellName != revelioSpellName) return;
        CastRevelio(position);
    }

    private void CastRevelio(Vector3 position)
    {
        if (highlightMaterial == null)
        {
            Debug.LogWarning("[RevelioSystem] Highlight material not assigned.");
            return;
        }

        Collider[] hits = Physics.OverlapSphere(position, range, interactableMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hits.Length; i++)
        {
            var interactable = hits[i].GetComponentInParent<Highlightable>();
            if (interactable == null) continue;

            var h = interactable.GetComponent<Highlightable>();
            if (h == null) h = interactable.GetComponentInChildren<Highlightable>();

            h.Highlight(highlightMaterial, duration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(scanOrigin != null ? scanOrigin.position : transform.position, range);
    }
}
