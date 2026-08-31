using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrinkNameDisplay : MonoBehaviour
{
    [SerializeField] private GlyphCastController caster;
    [SerializeField] private TextMeshProUGUI label;

    private void OnEnable() => caster.OnRecipeAssigned += HandleRecipeAssigned;
    private void OnDisable() => caster.OnRecipeAssigned -= HandleRecipeAssigned;

    private void HandleRecipeAssigned(DrinkRecipe recipe)
    {
        label.text = recipe != null ? recipe.drinkName : string.Empty;
    }
}
