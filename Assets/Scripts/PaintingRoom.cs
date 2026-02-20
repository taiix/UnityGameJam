using UnityEngine;
using System.Linq;

public class PaintingRoom : MonoBehaviour
{
    public Material kuwaharaMat;
    public Transform teleportPoint;

    public bool[] switches = new bool[7];
    private bool[] correctCombination = new bool[7]
    {
        false,
        true,
        false,
        false,
        true,
        true,
        true
    };

    private void Start()
    {
        kuwaharaMat = GetComponent<Renderer>().material;
        kuwaharaMat.SetInt("_Radius", 6);
    }
    public bool CheckSolution()
    {
        return switches.SequenceEqual(correctCombination);
    }
    public void SetSwitch(int index, bool value)
    {
        switches[index] = value;

        if (CheckSolution())
        {
            PuzzleSolved();
            Debug.Log("Puzzle Solved");
        }
    }

    void PuzzleSolved()
    {
        kuwaharaMat.SetInt("_Radius", 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Teleporting");
            other.gameObject.transform.position = teleportPoint.position;
        }
    }
}
