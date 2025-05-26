using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();

    private bool isCombining = false;
    private List<Item> itemsToCombine = new List<Item>();

    public Transform ItemContent;
    public GameObject InventoryItem;

    public Toggle EnableRemove;

    public InventoryItemController[] InventoryItems;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleCombineMode();
        }
    }

    public void ToggleCombineMode()
    {
        isCombining = !isCombining;
        itemsToCombine.Clear();
        ListItems(); // Refresh UI to enable click listeners
        Debug.Log(isCombining ? "Combine mode activated" : "Combine mode deactivated");
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {
        // Clear existing items
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        // Add items to UI
        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemNameTransform = obj.transform.Find("ItemName");
            var itemIconTransform = obj.transform.Find("ItemIcon");
            var removeButton = obj.transform.Find("RemoveItem").GetComponent<Button>();

            if (itemNameTransform == null || itemIconTransform == null)
            {
                Debug.LogError("Missing UI element on InventoryItem prefab.");
                continue;
            }

            var itemName = itemNameTransform.GetComponent<TMP_Text>();
            var itemIcon = itemIconTransform.GetComponent<Image>();

            if (itemName == null || itemIcon == null)
            {
                Debug.LogError("Missing TMP_Text or Image component on InventoryItem prefab.");
                continue;
            }

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            if (EnableRemove.isOn)
            {
                removeButton.gameObject.SetActive(true);
            }

            // 🔥 Combo logic setup
            var itemController = obj.GetComponent<InventoryItemController>();
            itemController.AddItem(item);

            var button = obj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();

                if (isCombining)
                {
                    button.onClick.AddListener(() => SelectForCombination(item));
                }
                else
                {
                    // Optional: Add normal use behavior
                    button.onClick.AddListener(() => itemController.UseItem());
                }
            }
        }

        SetInventoryItems();
    }

    public void EnableItemsRemove()
    {
        if (EnableRemove.isOn)
        {
            foreach(Transform item in ItemContent)
            {
                item.Find("RemoveItem").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveItem").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        for ( int i = 0; i < Items.Count; i++)
        {
            InventoryItems[i].AddItem(Items[i]);
        }
    }

    public void SelectForCombination(Item item)
    {
        if (!itemsToCombine.Contains(item))
        {
            itemsToCombine.Add(item);
        }

        if (itemsToCombine.Count == 2)
        {
            TryCombineItems(itemsToCombine[0], itemsToCombine[1]);
            itemsToCombine.Clear();
        }
    }


    public void TryCombineItems(Item item1, Item item2)
    {
        foreach (var combo in item1.combinations)
        {
            if (combo.otherItem == item2)
            {
                Remove(item1);
                Remove(item2);
                Add(combo.resultItem);
                Debug.Log($"Combined {item1.itemName} + {item2.itemName} → {combo.resultItem.itemName}");
                ListItems();
                return;
            }
        }

        foreach (var combo in item2.combinations)
        {
            if (combo.otherItem == item1)
            {
                Remove(item1);
                Remove(item2);
                Add(combo.resultItem);
                Debug.Log($"Combined {item2.itemName} + {item1.itemName} → {combo.resultItem.itemName}");
                ListItems();
                return;
            }
        }

        Debug.Log("These items cannot be combined.");
    }

}
