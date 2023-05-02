using System.Collections.Generic;
using UnityEngine;

/**
 * Database here is like an official registry of ScriptableObjects that allows
 * me to do lookup and such.
 * I will explore using XML or an actual DB later on, but this is a nice
 * lightweight solution for the prototyping stage.
 */
public class Database : MonoBehaviour
{
    public static Database instance;
    // public CraftingRecipeDatabase craftingRecipeDatabase;
    public ItemDatabase itemDatabase;

    #region Unity functions

    private void Awake()
    {
        // This is a singleton. Makes it so we can only have 1 active Database
        // in the game at a time.
        if (instance == null)
        {
            instance = this;
            // Ensure the gameobject this script is attached to is
            // never destroyed during a scene transition
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    //#region Crafting Recipes

    //public static CraftingRecipe GetCraftingRecipeById(string id)
    //{
    //    foreach (CraftingRecipe cr in instance.craftingRecipeDatabase.allCraftingRecipes)
    //    {
    //        if (cr.id == id)
    //        {
    //            return cr;
    //        }
    //    }
    //    return null;
    //}

    //public static List<CraftingRecipe> GetCraftingRecipesById(List<string> ids)
    //{
    //    return instance.craftingRecipeDatabase.allCraftingRecipes
    //        .FindAll(recipe => ids.Contains(recipe.id));
    //}

    //public static List<CraftingRecipe> GetAllCraftingRecipes()
    //{
    //    return instance.craftingRecipeDatabase.allCraftingRecipes;
    //}

    //#endregion

    #region Items

    public static Item GetItemById(string id)
    {
        foreach (Item item in instance.itemDatabase.allItems)
        {
            if (item.id == id)
            {
                return item;
            }
        }
        return null;
    }

    public static List<Item> GetItemsById(List<string> ids)
    {
        return instance.itemDatabase.allItems
            .FindAll(item => ids.Contains(item.id));
    }

    #endregion
}
