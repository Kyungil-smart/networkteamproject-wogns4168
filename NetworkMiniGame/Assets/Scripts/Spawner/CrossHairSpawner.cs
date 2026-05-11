using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CrossHairSpawner : NetworkBehaviour
{
    public static CrossHairSpawner Instance { get; private set; }

    [Header("캐쳐 전용 크로스헤어 프리펩")]
    [SerializeField] private GameObject _catcherPrefab;

    [Header("러너 전용 크로스헤어 프리펩")]
    [SerializeField] private GameObject _runnerPrefab;

    [Header("플레이어 인덱스별 색상 (0~3)")]
    [SerializeField] private Color[] _playerColors =
    {
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        Color.green
    };
    
    private List<NetworkObject> _spawnedCrossHairs = new List<NetworkObject>();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SpawnCrossHairs(ulong catcherId)
    {
        if (!IsServer) return;

        RoomInfo[] players = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            bool isCatcher = player.OwnerClientId == catcherId;
            
            GameObject prefab = isCatcher ? _catcherPrefab : _runnerPrefab;

            var obj    = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            var netObj = obj.GetComponent<NetworkObject>();
            
            netObj.SpawnAsPlayerObject(player.OwnerClientId, destroyWithScene: true);
            
            var crossHairMove = obj.GetComponent<CrossHairMove>();
            crossHairMove.InitServerRpc(player.PlayerIndex.Value, isCatcher);

            _spawnedCrossHairs.Add(netObj);
        }
    }
    
    public void DespawnCrossHairs()
    {
        if (!IsServer) return;

        foreach (var netObj in _spawnedCrossHairs)
            if (netObj != null) netObj.Despawn(true);

        _spawnedCrossHairs.Clear();
    }
}