using UnityEngine;
public enum IngredientType
{
    MushroomFromThePast,
    FreshFromThePresent,
    PowderFromTheFuture,

    RustySword,
    NormalSword,
    Excalibur
}
public class Ingredient : MonoBehaviour
{
    public IngredientType _ingredientType;
    public string ingredientName;
}
