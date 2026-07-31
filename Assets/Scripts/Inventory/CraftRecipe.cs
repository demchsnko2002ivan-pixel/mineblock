using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Crafting Recipe", menuName = "Inventory/Create New Crafting Recipe")]
public class CraftRecipe : ScriptableObject
{
    public enum Section
    {
        Tools, Materials, Placeables, Edible, Decorations, Everything
    }

    public Item resultItem;
    public int resultAmount;
    public List<Ingredient> ingredients;
    public Section section;
}
[Serializable]
public class Ingredient
{
    public Item item;
    public int amount;
}
