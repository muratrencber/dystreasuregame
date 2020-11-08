using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    static Inventory inventory;
    public List<ItemProperties> items = new List<ItemProperties>();

    [SerializeField]
    Transform hands;
    [SerializeField]
    Animator handAnimator;

    [SerializeField]
    Image itemImage;
    [SerializeField]
    Text itemName;
    [SerializeField]
    Text itemDetails;

    int currentIndex;
    Item currentItem = null;

    private void Awake()
    {
        inventory = this;
    }

    private void Start()
    {
        currentIndex = -1;
        Equip(0);
    }

    public static Item GetItemFromKey(string k)
    {
        for(int i = 0; i < inventory.hands.childCount; i++)
        {
            Item item = inventory.hands.GetChild(i).gameObject.GetComponent<Item>();
            if (item && item.key == k)
                return item;
        }
        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Equip(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Equip(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Equip(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Equip(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            Equip(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            Equip(5);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            Equip(6);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            Equip(7);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            Equip(8);
        if (Input.GetKeyDown(KeyCode.Alpha0))
            Equip(9);

        if (Input.GetButtonDown("Fire1") && currentItem)
            currentItem.OnAction1();
        if (Input.GetButtonDown("Fire2") && currentItem)
            currentItem.OnAction2();

    }

    public static void ReplaceItem(string targetKey, string newKey)
    {
        Item targetItem = GetItemFromKey(targetKey);
        Item newItem = GetItemFromKey(newKey);
        inventory.Equip(0);

        inventory.items.Remove(targetItem.properties);
        inventory.items.Insert(targetItem.indexInInventory, newItem.properties);
        newItem.properties.isActiveInInventory = true;
        newItem.indexInInventory = targetItem.indexInInventory;

        inventory.Equip(newItem.indexInInventory);

    }

    public static void AddItem(ItemProperties props)
    {
        if (!props.representedItem)
            props.representedItem = GetItemFromKey(props.representedItemKey);
        foreach (ItemProperties ip in inventory.items)
            if (ip.representedItem == props.representedItem)
            {
                if (ip.moreThanOne)
                {
                    ip.count++;
                    ip.representedItem.OnCountChanged();
                }
                if (!ip.isActiveInInventory)
                    ip.isActiveInInventory = true;
                inventory.Equip(ip.representedItem.indexInInventory);
                return;
            }
        ItemProperties newProps = props.representedItem.gameObject.AddComponent<ItemProperties>();
        newProps.Copy(props);
        newProps.isActiveInInventory = true;
        inventory.items.Add(newProps);
        newProps.representedItem.indexInInventory = inventory.items.Count - 1;
        newProps.representedItem.properties = newProps;
        inventory.Equip(newProps.representedItem.indexInInventory);
    }

    public static void DisableItem(int index)
    {
        if (index < 0 || index >= inventory.items.Count || !inventory.items[index])
            return;
        inventory.items[index].gameObject.SetActive(false);
        inventory.items.RemoveAt(index);
        for(int i = 0; i < inventory.items.Count; i++)
        {
            inventory.items[i].representedItem.indexInInventory = i;
        }
        //inventory.items[index].representedItem.OnDisabledInInventory();
        //inventory.items[index].isActiveInInventory = false;
        inventory.Equip(0);
    }

    void Equip(int index)
    {
        if (DialogueManager.currentlyTalking || index == currentIndex || index <0|| index >= items.Count || !items[index].isActiveInInventory)
            return;
        SetDetails("");
        for(int i = 0; i < items.Count; i++)
        {
            ItemProperties p = items[i];
            p.gameObject.SetActive(i == index);
            if (i == index)
            {
                p.representedItem.OnEquip();
                currentItem = p.representedItem;
                itemName.text = LocalizationManager.GetText(p.interactableName);
            }
            if (i == currentIndex)
                p.representedItem.OnUnequip();
        }
        handAnimator.SetTrigger("equip");
        currentIndex = index;
    }

    public static void SetDetails(string detail)
    {
        inventory.itemDetails.text = detail;
    }
}
