using UnityEngine;

public class ItemProperties : IInteractableMono
{
    public Sprite image;
    public bool isActiveInInventory;
    public Item representedItem;
    public string representedItemKey;
    public bool moreThanOne;
    public int count;

    public override void OnInteract()
    {
        if (representedItem == null)
            representedItem = Inventory.GetItemFromKey(representedItemKey);
        Inventory.AddItem(this);
        Destroy(gameObject);
    }

    public void Copy(ItemProperties props)
    {
        image = props.image;
        isActiveInInventory = props.isActiveInInventory;
        representedItem = props.representedItem;
        moreThanOne = props.moreThanOne;
        count = props.count;
        interactableName = props.interactableName;
        interactType = props.interactType;
        offset = props.offset;
        canBeInteracted = props.canBeInteracted;
    }
}
