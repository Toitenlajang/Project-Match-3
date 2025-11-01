using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchEffectDisplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject effectPanel; // Parent panel for all effects
    public GameObject effectItemPrefab; // Prefab showing "Icon + Count"

    [Header("Tile Type Sprites")]
    public Sprite fireSprite;
    public Sprite waterSprite;
    public Sprite electricSprite;
    public Sprite grassSprite;

    [Header("Settings")]
    public float displayDuration = 1.5f; // How long to show the effects

    private Dictionary<string, Sprite> typeSpriteMap;

    void Start()
    {
        // Initialize sprite mapping
        typeSpriteMap = new Dictionary<string, Sprite>()
        {
            { "fire", fireSprite },
            { "water", waterSprite },
            { "electric", electricSprite },
            { "grass", grassSprite }
        };

        // Hide panel initially
        if (effectPanel != null)
        {
            effectPanel.SetActive(false);
        }
    }

    // Show the match effects
    public IEnumerator ShowMatchEffects(Dictionary<string, int> matchedTiles)
    {
        if (effectPanel == null || effectItemPrefab == null)
        {
            Debug.LogWarning("MatchEffectDisplay: Missing UI references!");
            yield break;
        }

        // Clear previous effects
        ClearEffects();

        // Show panel
        effectPanel.SetActive(true);

        // Create effect item for each matched type
        foreach (var pair in matchedTiles)
        {
            string tileType = pair.Key;
            int count = pair.Value;

            CreateEffectItem(tileType, count);
        }

        // Wait for duration
        yield return new WaitForSeconds(displayDuration);

        // Hide panel
        effectPanel.SetActive(false);
    }

    // Create one effect item (icon + count)
    private void CreateEffectItem(string tileType, int count)
    {
        GameObject item = Instantiate(effectItemPrefab, effectPanel.transform);

        // Find the Image component for the icon
        Image iconImage = item.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null && typeSpriteMap.ContainsKey(tileType.ToLower()))
        {
            iconImage.sprite = typeSpriteMap[tileType.ToLower()];
        }

        // Find the Text component for the count
        TextMeshProUGUI countText = item.transform.Find("CountText")?.GetComponent<TextMeshProUGUI>();
        if (countText != null)
        {
            countText.text = $"x{count}";
        }

        Debug.Log($"<color=yellow>Created effect: {tileType} x{count}</color>");
    }

    // Clear all effect items
    private void ClearEffects()
    {
        foreach (Transform child in effectPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
