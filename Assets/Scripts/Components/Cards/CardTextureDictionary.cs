using System.Collections.Generic;
using UnityEngine;

public class CardTextureDictionary : MonoBehaviour
{
    [SerializeField] private List<CardTextureMapping> cardTextureList;
    [SerializeField] private Material defaultCardTexture;

    private Dictionary<string, Material> dict;

    void Awake()
    {
        ServiceLocator.Register<CardTextureDictionary>(this);
        dict = new Dictionary<string, Material>();
        foreach (var m in cardTextureList)
            if (!dict.ContainsKey(m.cardName))
                dict.Add(m.cardName, m.cardTexture);
    }

    public Material GetCardTexture(string cardName)
    {
        if (dict.TryGetValue(cardName, out var tex))
            return tex;
        Debug.LogWarning($"[CardTextureDictionary] No texture for {cardName}");
        return defaultCardTexture;
    }
}