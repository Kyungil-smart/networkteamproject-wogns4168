using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private Tilemap _backgroundTilemap;
    
    public Bounds MapBounds { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (_backgroundTilemap == null) return;
        
        _backgroundTilemap.CompressBounds();
        
        MapBounds = _backgroundTilemap.localBounds;
    }
}