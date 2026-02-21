using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlightable : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;

    private readonly Dictionary<Renderer, Material[]> originals = new();
    private Coroutine routine;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    public void Highlight(Material highlightMat, float duration)
    {
        if (highlightMat == null) return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(DoHighlight(highlightMat, duration));
    }

    private IEnumerator DoHighlight(Material highlightMat, float duration)
    {
        Apply(highlightMat);
        yield return new WaitForSeconds(duration);
        Remove();
        routine = null;
    }

    private void Apply(Material highlightMat)
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;

            if (!originals.ContainsKey(r))
                originals[r] = r.sharedMaterials;

            // add highlight as an extra material slot
            var mats = r.materials;

            // avoid double-adding
            for (int i = 0; i < mats.Length; i++)
                if (mats[i] == highlightMat) return;

            var newMats = new Material[mats.Length + 1];
            for (int i = 0; i < mats.Length; i++) newMats[i] = mats[i];
            newMats[^1] = highlightMat;

            r.materials = newMats;
        }
    }

    private void Remove()
    {
        foreach (var kv in originals)
        {
            if (kv.Key == null) continue;
            kv.Key.sharedMaterials = kv.Value;
        }
        originals.Clear();
    }
}
