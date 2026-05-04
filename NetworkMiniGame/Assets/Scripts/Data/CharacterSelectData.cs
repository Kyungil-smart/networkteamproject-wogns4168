using UnityEngine;

public class CharacterSelectData : MonoBehaviour
{
    public static CharacterSelectData Instance { get; private set; }

    public int LocalCharacterIndex { get; private set; } = 0;
    public GameObject LocalGamePrefab { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetLocalSelection(int index, GameObject prefab)
    {
        LocalCharacterIndex = index;
        LocalGamePrefab = prefab;
    }
}