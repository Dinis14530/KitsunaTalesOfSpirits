using UnityEngine;

public static class LootHelper
{
    public static GameObject SpawnLootItem(ItemSO itemSO, Vector3 position, int quantity)
    {
        GameObject lootObject = new(itemSO.itemName);
        lootObject.transform.position = position;
        lootObject.layer = LayerMask.NameToLayer("Items");

        // Child Sprite
        GameObject spriteChild = new("ItemSprite");
        spriteChild.transform.SetParent(lootObject.transform);
        spriteChild.transform.localPosition = Vector3.zero;
        spriteChild.layer = LayerMask.NameToLayer("Items");

        SpriteRenderer spriteRenderer = spriteChild.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSO.sprite;
        spriteRenderer.sortingLayerName = "default";

        // Collider + Física
        CircleCollider2D col = lootObject.AddComponent<CircleCollider2D>();
        col.isTrigger = false;
        int itemsLayer = LayerMask.NameToLayer("Items");
        Physics2D.IgnoreLayerCollision(itemsLayer, itemsLayer, true);
        

        Rigidbody2D rb = lootObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Script do item
        Item itemComponent = lootObject.AddComponent<Item>();
        itemComponent.SetItemData(itemSO, quantity);

        lootObject.tag = "Item";

        return lootObject;
    }
}
