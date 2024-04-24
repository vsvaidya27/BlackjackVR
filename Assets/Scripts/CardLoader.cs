using System.Collections.Generic;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    // List to hold all card prefabs
    private List<GameObject> allCardPrefabs = new List<GameObject>();

    void Start()
    {
        LoadAllCardPrefabs();
    }

    void LoadAllCardPrefabs()
    {
        // Load all GameObjects in the specified subdirectory of Resources
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Playing Cards/Resource/Prefab/BackColor_Black");

        // Add loaded prefabs to the list
        foreach (GameObject prefab in prefabs)
        {
            allCardPrefabs.Add(prefab);
            Debug.Log("Loaded prefab: " + prefab.name);
        }

        // Optionally: Instantiate them directly to see them in the scene (for testing)
        foreach (GameObject cardPrefab in allCardPrefabs)
        {
            Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
        }
    }
}
