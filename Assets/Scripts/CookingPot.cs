using UnityEngine;
using System;

public class CookingPot : MonoBehaviour
{
    //order of ingredients matters
    public IngredientType[] ingredientsInPot;
    public IngredientType expectedType;
    public int currentIngredientIndex = 0;

    private void Start()
    {
        expectedType = ingredientsInPot[0];
    }

    private void Update()
    {
        if (currentIngredientIndex >= ingredientsInPot.Length)
        {
            if (this.gameObject.transform.GetChild(0).gameObject.activeSelf == false)
            {
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            Debug.Log("All ingredients added! Ready to cook");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ingredient>(out Ingredient i))
        {
            if (i._ingredientType == expectedType)
            {
                Destroy(other.gameObject);
                expectedType
                    = ingredientsInPot
                    [(Array.IndexOf(ingredientsInPot, expectedType) + 1) % ingredientsInPot.Length];
                currentIngredientIndex++;

                Debug.Log($"Added {i.ingredientName} to the pot.");
            }
        }
    }
}
