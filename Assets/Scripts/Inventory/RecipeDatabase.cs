using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Recipe Database", menuName = "Inventory/Recipe Database")]
public class RecipeDatabase : ScriptableObject
{
    public List<CraftRecipe> allRecipes;
    public CraftRecipe GetRecipeByResult(Item item)
    {
        foreach (CraftRecipe recipe in allRecipes)
        {
            if (recipe.resultItem == item)
            {
                return recipe;
            }
        }
        return null;
    }
    public List<CraftRecipe> GetRecipesPerSection(CraftRecipe.Section section)
    {
        List<CraftRecipe> recipes = new List<CraftRecipe>();
        foreach (CraftRecipe recipe in allRecipes)
        {
            if (recipe.section == section)
            {
                recipes.Add(recipe);
            }
        }
        return recipes;
    }
    public List<CraftRecipe> GetAllRecipes()
    {
        return allRecipes;
    }
}
