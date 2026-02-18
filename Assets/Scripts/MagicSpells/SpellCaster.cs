using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [System.Serializable]
    public class SpellDefinition
    {
        public int page;
        public List<string> sequence;
        public string spellName;
        public bool mirrorLeftRight = true;
    }

    public List<SpellDefinition> knownSpells = new List<SpellDefinition>();

    [Header("Mirroring Settings")]
    public bool mirrorLeftRight = false;

    void Awake()
    {
        Debug.Log("[SpellCaster] Loaded spells:");
        foreach (var spell in knownSpells)
        {
            Debug.Log($"- {spell.spellName} Page:{spell.page} Sequence:{string.Join(",", spell.sequence)}");
        }
    }


    public string MatchSpell(int currentPage, List<string> inputSequence)
    {
        int adjustedPage = currentPage + 1;

        Debug.Log($"[SpellCaster] Matching for Page {adjustedPage}. Raw input: {string.Join(",", inputSequence)}");

        if (mirrorLeftRight)
        {
            for (int i = 0; i < inputSequence.Count; i++)
            {
                if (inputSequence[i] == "Left")
                    inputSequence[i] = "Right";
                else if (inputSequence[i] == "Right")
                    inputSequence[i] = "Left";
            }
            Debug.Log($"[SpellCaster] Mirrored input: {string.Join(",", inputSequence)}");
        }

        foreach (var spell in knownSpells)
        {
            Debug.Log($"Checking spell: {spell.spellName} Page:{spell.page} Sequence:{string.Join(",", spell.sequence)}");

            if (spell.page != adjustedPage) continue;

            if (spell.sequence.Count != inputSequence.Count)
            {
                Debug.Log($"Length mismatch: expected {spell.sequence.Count}, got {inputSequence.Count}");
                continue;
            }

            bool match = true;
            for (int i = 0; i < spell.sequence.Count; i++)
            {
                if (spell.sequence[i] != inputSequence[i])
                {
                    Debug.Log($"Mismatch at index {i}: expected '{spell.sequence[i]}' but got '{inputSequence[i]}'");
                    match = false;
                    break;
                }
            }

            if (match)
            {
                Debug.Log($"[SpellCaster] ✅ Match found: {spell.spellName}");
                return spell.spellName;
            }
        }

        Debug.Log("[SpellCaster] ❌ No matching spell found.");
        return null;
    }
}
