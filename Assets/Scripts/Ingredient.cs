using UnityEngine;
public enum IngredientType
{
    MushroomFromThePast,
    FreshFromThePresent,
    PowderFromTheFuture
}
public class Ingredient : MonoBehaviour
{
  

    public IngredientType _ingredientType;
    public string ingredientName;

    public float freshnessLevel;
    public float cookingTime;
    public bool isCooked;

    

    public void CookIngredient(float time)
    {
        cookingTime += time;
        if (cookingTime >= 5.0f) // Arbitrary cooking time threshold
        {
            isCooked = true;
            Debug.Log($"{ingredientName} is cooked!");
        }
    }
}
