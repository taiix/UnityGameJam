using UnityEngine;

public class CookingTripod : MonoBehaviour
{
    private bool isOccupied = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;

        if (other.TryGetComponent<CookingPot>(out CookingPot pot))
        {
            if (pot.currentIngredientIndex >= pot.ingredientsInPot.Length)
            {
                Debug.Log("Cooking tripod detected a completed pot! You can now cook.");
                pot.gameObject.transform.SetParent(this.gameObject.transform);
                pot.gameObject.transform.localPosition = Vector3.zero + new Vector3(0, 0, 60);
                isOccupied = true;
                
                pot.GetComponent<PickupInteractable>().enabled = false;
            }
        }
    }
}
