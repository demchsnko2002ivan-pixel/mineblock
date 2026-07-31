using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecipeIngredient : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI amount;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject lackingObj;
    public Ingredient ingredient;
    public void SetData(Sprite icon, int amount)
    {
        this.amount.text = amount.ToString();
        this.icon.sprite = icon;
    }
    public void LackUpdate()
    {
        bool contains = IsContains();
        lackingObj.SetActive(!contains);
        Debug.Log(ingredient.item.itemName + contains + ingredient.amount);
    }
    public bool IsContains()
    {
        bool contains = InventoryManager.Instance.Contains(ingredient.item, ingredient.amount);
        return contains;
    }
}
