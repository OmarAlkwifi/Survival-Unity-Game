using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemController : MonoBehaviour
{
    public Item item;
    public Button RemoveButton;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth script not found in the scene.");
        }
    }

    public void RemoveItem()
    {
        InventoryManager.Instance.Remove(item);
        Destroy(gameObject);
    }

    public void AddItem(Item newItem)
    {
        if (newItem == null)
        {
            Debug.LogError("AddItem called with null Item.");
        }
        item = newItem;
    }

    public void UseItem()
    {
        if (item == null)
        {
            Debug.LogError("UseItem called but item is null.");
            return;
        }

        switch (item.itemType)
        {
            case Item.ItemType.Potion:
                if (playerHealth != null)
                {
                    playerHealth.Heal(10); // Adjust the healing amount as needed
                    Debug.Log("Potion used. Health increased.");
                }
                break;
            case Item.ItemType.Book:
                if (playerHealth != null)
                {
                    playerHealth.RestoreMind(10); // Increase mind by 10
                    AddBottleToInventory();

                    Debug.Log("Book used. Mind increased.");
                }
                break;
            case Item.ItemType.Molotov:
               // EquipMolotov();
                Debug.Log("Molotov equipped.");
                break;
            case Item.ItemType.Trap:
                Debug.Log("Trap item is display-only and cannot be used.");
                return;
        }
        RemoveItem();
    }

   // void EquipMolotov()
  //  {
        // Find the Molotov GameObject in the scene (adjust as needed)
       // Molotov molotov = FindObjectOfType<Molotov>();
      //  if (molotov == null)
      //  {
      //      Debug.LogError("Molotov script not found in the scene.");
       //     return;
     //   }

        //molotov.Equip();
  //  }

    void AddBottleToInventory()
    {
        Item bottleItem = new Item
        {
            itemName = "Bottle",
            itemType = Item.ItemType.Bottle // Assuming you have a Misc type; adjust as needed
        };
        InventoryManager.Instance.Add(bottleItem);
    }
}


