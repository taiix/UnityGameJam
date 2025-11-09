using UnityEngine;
using System;

public class CookingPot : MonoBehaviour
{
    //order of ingredients matters
    public IngredientType[] ingredientsInPot;
    public IngredientType expectedType;
    public int currentIngredientIndex = 0;

    public float cookingTime = 10f; // Time required to cook the dish

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

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<CookingTripod>(out CookingTripod t))
        {
            if (currentIngredientIndex >= ingredientsInPot.Length)
            {
                // Start cooking process
                Debug.Log("Cooking started...");
                // Here you can implement a timer or coroutine to handle cooking time
                cookingTime -= Time.deltaTime;
                if (cookingTime <= 0)
                {
                    var child = this.gameObject.transform.GetChild(0).gameObject;
                    cookingTime = 0;
                    child.GetComponent<Renderer>().material.color = Color.red;
                    foreach (Transform grandChild in child.transform)
                    {
                        if (grandChild.TryGetComponent<ParticleSystem>(out ParticleSystem p))
                        { 
                            var main = p.main;
                            main.startColor = Color.red;
                        }
                    }
                }
            }
        }
    }
}
