using UnityEngine;

public class PlankPuzzle : MonoBehaviour
{

    int planksPlaced;
    public GameObject planksToAppear;
    public GameObject ColliderToBeRemoved;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsItCompleted()
    {
        planksPlaced++;
        if (planksPlaced >= 2)
        {
            planksToAppear.SetActive(true);
            ColliderToBeRemoved.SetActive(false);
        }
    }
}
