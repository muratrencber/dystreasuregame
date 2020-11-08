using UnityEngine;

public class ThrowableItem : Item
{
    const float throwForce = 200;
    public GameObject original;

    public override void OnEquip()
    {
        base.OnEquip();
        if (properties.moreThanOne)
            Inventory.SetDetails(properties.count.ToString());
    }

    public override void OnCountChanged()
    {
        Inventory.SetDetails(properties.count.ToString());
    }

    public override void OnAction1()
    {
        GameObject instance = Instantiate(original);
        instance.transform.SetParent(AreaManager.currentArea.transform);
        instance.transform.position = transform.position;
        ItemProperties props = instance.GetComponent<ItemProperties>();
        props.interactableName = properties.interactableName;
        props.representedItem = this;
        Rigidbody instanceRB = instance.GetComponent<Rigidbody>();
        instanceRB.AddForce(transform.forward * throwForce);
        properties.count--;
        if (properties.moreThanOne)
            Inventory.SetDetails(properties.count.ToString());
        if (properties.moreThanOne && properties.count > 0)
            return;
        Inventory.DisableItem(indexInInventory);
    }
}
