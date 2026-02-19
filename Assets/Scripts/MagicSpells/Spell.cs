using System.Collections.Generic;

[System.Serializable]
public class Spell
{
    public string spellName;
    public List<string> inputSequence; // "Top", "Right", etc.
    public int requiredPage = 0;

    public Spell(string name, List<string> sequence, int page)
    {
        spellName = name;
        inputSequence = sequence;
        requiredPage = page;
    }

    public bool Matches(List<string> combo)
    {
        if (combo.Count != inputSequence.Count)
            return false;

        for (int i = 0; i < combo.Count; i++)
        {
            if (combo[i] != inputSequence[i])
                return false;
        }

        return true;
    }
}

