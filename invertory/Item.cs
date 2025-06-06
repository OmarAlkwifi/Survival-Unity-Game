using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int value;
    public Sprite icon;
    public ItemType itemType;

    public enum ItemType
    {
        Potion,
        Book,
        Bottle,
        Molotov,
        Trap
    }

    [System.Serializable]
    public class ItemCombination
    {
        public Item otherItem;
        public Item resultItem;
    }

    public List<ItemCombination> combinations = new List<ItemCombination>();
}
