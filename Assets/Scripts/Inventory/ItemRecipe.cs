using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemRecipe : MonoBehaviour
{
    [SerializeField]
    private GameObject recipeInfo;
    private CraftRecipe craftRecipe;
    [SerializeField]
    private GameObject ingredientPrefab;
    [SerializeField]
    private GameObject ingredientParent;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private Button craftButton;
    private List<RecipeIngredient> ingredients = new List<RecipeIngredient>();
    private void Awake()
    {
        //craftButton.onClick.AddListener(HandleButtonClick);
    }
    void Start()
    {
        recipeInfo.SetActive(false);
    }
    private void FixedUpdate()
    {
        UpdateRecipe();
    }
    public void HandleButtonClick()
    {
        if (CanCraft())
        {
            InventoryManager.Instance.CraftItem(craftRecipe);
        }
        else
        {
            Debug.Log("Not enough");
        }
    }
    public void ShowRecipe()
    {
        recipeInfo.SetActive(true);
        // Чистка списка ингредиентов
        RecipeIngredient[] lastIngredients = ingredientParent.transform.GetComponentsInChildren<RecipeIngredient>();
        foreach (RecipeIngredient obj in lastIngredients)
        {
            Destroy(obj.gameObject);
        }
        // Спавним новые ингредиенты
        foreach (Ingredient ingredient in craftRecipe.ingredients)
        {
            GameObject obj = Instantiate(ingredientPrefab, ingredientParent.transform);
            RecipeIngredient recipeIngredient = obj.transform.GetComponent<RecipeIngredient>();
            recipeIngredient.SetData(ingredient.item.icon, ingredient.amount);
            recipeIngredient.ingredient = ingredient;
            recipeIngredient.LackUpdate();
            ingredients.Add(recipeIngredient);
        }
    }
    public void UpdateRecipe()
    {
        foreach (var ingredient in ingredients)
        {
            ingredient.LackUpdate();
        }
    }
    public void HideRecipe()
    {
        recipeInfo.SetActive(false);
    }
    public void SetRecipe(CraftRecipe craftRecipe)
    {
        this.craftRecipe = craftRecipe;
        itemName.text = craftRecipe.resultItem.itemName;
        itemIcon.sprite = craftRecipe.resultItem.icon;
    }
    public bool CanCraft()
    {
        foreach (var ingredient in ingredients)
        {
            if (ingredient.IsContains() == false)
            {
                return false;
            }
        }
        return true;
    }
}
