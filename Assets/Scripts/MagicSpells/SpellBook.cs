using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellDefinition
{
    public string spellName;
    public List<string> combo;
    public GameObject effectPrefab;
}

[CreateAssetMenu(fileName = "Spellbook", menuName = "Magic/Spellbook")]
public class Spellbook : ScriptableObject
{
    public List<SpellDefinition> spells;

    public SpellDefinition GetSpellByCombo(List<string> inputCombo)
    {
        foreach (var spell in spells)
        {
            if (spell.combo.Count != inputCombo.Count)
                continue;

            bool match = true;
            for (int i = 0; i < inputCombo.Count; i++)
            {
                if (spell.combo[i] != inputCombo[i])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return spell;
        }
        return null;
    }
}
