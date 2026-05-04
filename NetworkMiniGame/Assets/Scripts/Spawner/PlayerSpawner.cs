using System;
using System.Collections;
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

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        StartCoroutine(SpawnAllPlayers());
    }

    private IEnumerator SpawnAllPlayers()
    {
        // RoomInfo CharacterIndex 동기화 대기
        yield return new WaitForSeconds(0.3f);

        RoomInfo[] roomInfos = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        Array.Sort(roomInfos, (a, b) => a.PlayerIndex.Value.CompareTo(b.PlayerIndex.Value));

        foreach (var info in roomInfos)
        {
            int spawnIdx = info.PlayerIndex.Value;
            int charIdx  = Mathf.Clamp(info.CharacterIndex.Value, 0, _gamePrefabs.Length - 1);

            SpawnSetting setting = _spawnSettings[spawnIdx];

            GameObject player = Instantiate(
                _gamePrefabs[charIdx],
                setting.point.position,
                setting.point.rotation
            );
            player.transform.localScale = setting.scale;

            player.GetComponent<NetworkObject>()
                .SpawnAsPlayerObject(info.OwnerClientId, destroyWithScene: true);
        }
    }
}