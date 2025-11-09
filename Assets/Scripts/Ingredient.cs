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
}
