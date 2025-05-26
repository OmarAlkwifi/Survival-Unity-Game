using UnityEngine;
using System.Collections;

public class TrapPlacer : MonoBehaviour
{
    public GameObject trapPrefab;
    public float placementDistance = 2f;
    public float placementDelay = 0.5f;

    private bool isPlacingTrap = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isPlacingTrap)
        {
            if (HasTrapInInventory())
            {
                StartCoroutine(PlaceTrapAfterDelay());
            }
            else
            {
                Debug.Log("You have no traps in your inventory.");
            }
        }
    }

    bool HasTrapInInventory()
    {
        foreach (Item item in InventoryManager.Instance.Items)
        {
            if (item.itemType == Item.ItemType.Trap)
            {
                return true;
            }
        }
        return false;
    }

    void RemoveOneTrapFromInventory()
    {
        for (int i = 0; i < InventoryManager.Instance.Items.Count; i++)
        {
            if (InventoryManager.Instance.Items[i].itemType == Item.ItemType.Trap)
            {
                InventoryManager.Instance.Remove(InventoryManager.Instance.Items[i]);
                InventoryManager.Instance.ListItems(); // Refresh UI
                return;
            }
        }
    }

    IEnumerator PlaceTrapAfterDelay()
    {
        isPlacingTrap = true;
        yield return new WaitForSeconds(placementDelay);

        Vector3 spawnPos = transform.position + transform.forward * placementDistance;
        Instantiate(trapPrefab, spawnPos, Quaternion.identity);

        RemoveOneTrapFromInventory(); // Remove trap after placement

        isPlacingTrap = false;
    }
}
