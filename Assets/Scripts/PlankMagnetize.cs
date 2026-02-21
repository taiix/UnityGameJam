using UnityEngine;

public class PlankMagnetize : MonoBehaviour
{
    PlankPuzzle plankPuzzleScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plankPuzzleScript = transform.GetComponentInParent<PlankPuzzle>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plank"))
        {
            other.transform.GetComponent<PickupInteractable>().enabled = false;
            other.gameObject.transform.position = transform.position;
            other.gameObject.transform.rotation = transform.rotation;
            transform.GetComponent<MeshRenderer>().enabled = false;
            other.enabled = false;
            Destroy(other.transform.GetComponent<Rigidbody>());
            plankPuzzleScript.IsItCompleted();
            transform.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
