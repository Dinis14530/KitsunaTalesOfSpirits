using UnityEngine;

public static class LootHelper
{
    public static GameObject SpawnLootItem(ItemSO itemSO, Vector3 position, int quantity)
    {
        if (itemSO == null)
        {
            Debug.LogWarning("[LootHelper] Cannot spawn loot: itemSO is null.");
            return null;
        }

        if (itemSO.sprite == null)
        {
            Debug.LogWarning($"[LootHelper] itemSO '{itemSO.itemName}' has no sprite assigned.");
        }

        GameObject lootObject = new(itemSO.itemName);
        lootObject.transform.position = position;
        lootObject.layer = LayerMask.NameToLayer("Items");

        // Map Icon Child
        // GameObject mapIconChild = new("MapIcon");
        // mapIconChild.transform.SetParent(lootObject.transform);
        // mapIconChild.transform.localPosition = Vector3.zero;
        // mapIconChild.layer = LayerMask.NameToLayer("Background");

        // SpriteRenderer mapRenderer = mapIconChild.AddComponent<SpriteRenderer>();
        // mapRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/Circle.png");
        // mapRenderer.color = Color.green;
        // mapRenderer.sortingLayerName = "Default";
        // mapRenderer.sortingOrder = -1;
        // mapIconChild.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // Sprite do Item
        // O sprite fica num filho separado para manter o objeto principal com fisica e trigger
        GameObject spriteChild = new("ItemSprite");
        spriteChild.transform.SetParent(lootObject.transform);
        spriteChild.transform.localPosition = Vector3.zero;
        spriteChild.layer = LayerMask.NameToLayer("Items");

        SpriteRenderer spriteRenderer = spriteChild.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = itemSO.sprite;
        spriteRenderer.sortingLayerName = "Default";
        spriteRenderer.sortingOrder = 0;

        // Collider
        // O collider pertence ao objecto raiz para simplificar pickups e interacoes
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
