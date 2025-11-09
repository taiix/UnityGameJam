using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public enum IngredientType
    {
       MushroomFromThePast,
       FreshFromThePresent,
       PowderFromTheFuture
    }

    public IngredientType _ingredientType;
    public string ingredientName;

    public float freshnessLevel;
    public float cookingTime;
    public bool isCooked;

    void Update()
    {
        
    }
}
