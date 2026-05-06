using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Serializable]
    public struct SpawnSetting
    {
        public Transform point;
        public Vector3 scale;
    }

    [Header("캐릭터 게임 프리펩 (인덱스 순서대로)")]
    [SerializeField] private GameObject[] _gamePrefabs;

    [Header("스폰 세팅 (0~3번 슬롯)")]
    [SerializeField] private SpawnSetting[] _spawnSettings;
    
    [Header("true = 룸씬 / false = 게임씬")]
    [SerializeField] private bool _isRoomScene = false;
    
    private Dictionary<ulong, NetworkObject> _spawnedCharacters = new Dictionary<ulong, NetworkObject>();

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        StartCoroutine(SpawnAllPlayers());

        if (_isRoomScene)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback   += OnClientDisconnected;
        }
    }
    
    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;
            
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
    
    private void OnClientConnected(ulong clientId)
    {
        StartCoroutine(SpawnPlayerDelayed(clientId));
    }
    
    private void OnClientDisconnected(ulong clientId)
    {
        if (_spawnedCharacters.TryGetValue(clientId, out NetworkObject netObj))
        {
            if (netObj != null) netObj.Despawn(true);
            _spawnedCharacters.Remove(clientId);
        }
    }
    
    private IEnumerator SpawnPlayerDelayed(ulong clientId)
    {
        yield return new WaitForSeconds(0.5f);

        RoomInfo[] roomInfos = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        foreach (var info in roomInfos)
        {
            if (info.OwnerClientId == clientId)
            {
                SpawnCharacter(info);
                break;
            }
        }
    }

    private IEnumerator SpawnAllPlayers()
    {
        yield return new WaitForSeconds(0.3f);

        RoomInfo[] roomInfos = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        Array.Sort(roomInfos, (a, b) => a.PlayerIndex.Value.CompareTo(b.PlayerIndex.Value));

        foreach (var info in roomInfos)
            SpawnCharacter(info);
    }

    private void SpawnCharacter(RoomInfo info)
    {
        if (_spawnedCharacters.ContainsKey(info.OwnerClientId)) return;

        int spawnIdx = info.PlayerIndex.Value;
        int charIdx  = Mathf.Clamp(info.CharacterIndex.Value, 0, _gamePrefabs.Length - 1);
        SpawnSetting setting = _spawnSettings[spawnIdx];

        GameObject player = Instantiate(_gamePrefabs[charIdx], setting.point.position, setting.point.rotation);
        player.transform.localScale = setting.scale;

        var netObj = player.GetComponent<NetworkObject>();

        if (_isRoomScene)
            netObj.Spawn(destroyWithScene: true);
        else
            netObj.SpawnAsPlayerObject(info.OwnerClientId, destroyWithScene: true);

        _spawnedCharacters[info.OwnerClientId] = netObj;
    }
}