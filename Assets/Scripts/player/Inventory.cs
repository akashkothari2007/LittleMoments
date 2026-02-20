using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{

    [Header("UI")]
    public Transform inventoryPanel;    
    public GameObject slotPrefab;         

    [Header("Icons")]
    public Sprite redFlowerSprite;
    public Sprite blueFlowerSprite;
    public Sprite logSprite;
    public Sprite flintAndSteelSprite;

    public AudioSource pickupSource;
    public AudioClip pickupClip;


    private Dictionary<ItemType, int> counts = new();

    public int GetCount(ItemType type)
        => counts.TryGetValue(type, out var c) ? c : 0;

    public void Add(ItemType type, int amount)
    {
        if (type == ItemType.None) return;
        counts[type] = GetCount(type) + amount;
        pickupSource.PlayOneShot(pickupClip);
        updateUI();
        Debug.Log($"{type}: {counts[type]}");
        // later: update UI here or raise an event
    }

    public bool TrySpend(ItemType type, int amount)
    {
        int current = GetCount(type);
        if (current < amount) return false;
        counts[type] = current - amount;
        updateUI();
        return true;
    }

    public void updateUI()
    {
        // Clear old UI
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // Rebuild UI
        foreach (var kv in counts)
        {
            ItemType type = kv.Key;
            int count = kv.Value;

            if (count <= 0) continue;

            GameObject slot = Instantiate(slotPrefab, inventoryPanel);

            Image icon = slot.transform.GetChild(0).GetComponent<Image>();
            TextMeshProUGUI text = slot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            icon.sprite = GetSpriteForType(type);
            text.text = "x " + count.ToString();
        }
    }

    private Sprite GetSpriteForType(ItemType type)
    {
        switch (type)
        {
            case ItemType.RedFlower:
                return redFlowerSprite;
            case ItemType.BlueFlower:
                return blueFlowerSprite;
            case ItemType.Log:
                return logSprite;
            case ItemType.FlintAndSteel:
                return flintAndSteelSprite;
            default:
                return null;
        }
    }
}
