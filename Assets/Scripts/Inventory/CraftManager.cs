using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public static CraftManager Instance;

    [SerializeField]
    private RecipeDatabase database;
    [SerializeField]
    private GameObject recipePrefab;
    [SerializeField]
    private GameObject recipeParent;
    public List<ItemRecipe> activeRecipes = new List<ItemRecipe>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void Tools()
    {
        activeRecipes = ShowRecipes(CraftRecipe.Section.Tools);
    }
    public void Materials()
    {
        activeRecipes = ShowRecipes(CraftRecipe.Section.Materials);
    }
    public void Placeables()
    {
        activeRecipes = ShowRecipes(CraftRecipe.Section.Placeables);
    }
    public void Edible()
    {
        activeRecipes = ShowRecipes(CraftRecipe.Section.Edible);
    }
    public void Decorations()
    {
        activeRecipes = ShowRecipes(CraftRecipe.Section.Decorations);
    }
    public void Everything()
    {
        activeRecipes = ShowRecipes(CraftRecipe.Section.Everything);
    }
    private List<ItemRecipe> ShowRecipes(CraftRecipe.Section section)
    {
        // Destroy previous recipe items using DestroyImmediate-safe pattern
        foreach (ItemRecipe recipe in activeRecipes)
        {
            if (recipe != null)
            {
                Destroy(recipe.gameObject);
            }
        }
        activeRecipes.Clear();

        List<CraftRecipe> recipes;
        if (section == CraftRecipe.Section.Everything)
        {
            recipes = database.GetAllRecipes();
        }
        else
        {
            recipes = database.GetRecipesPerSection(section);
        }

        List<ItemRecipe> newRecipes = new List<ItemRecipe>();
        foreach (CraftRecipe recipe in recipes)
        {
            GameObject obj = Instantiate(recipePrefab, recipeParent.transform);
            ItemRecipe item = obj.transform.GetComponent<ItemRecipe>();
            item.SetRecipe(recipe);
            newRecipes.Add(item);
        }
        return newRecipes;
    }
    public void UpdateRecipes()
    {
        foreach (var recipe in activeRecipes)
        {
            if (recipe != null)
            {
                recipe.UpdateRecipe();
            }
        }
    }
}