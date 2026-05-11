using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager Instance { get; private set; }

    [SerializeField] private float _roundDuration = 30f; // 라운드당 제한시간(초)


    public NetworkVariable<int>  CurrentRound     = new NetworkVariable<int>(0);

    public NetworkVariable<ulong> CurrentCatcherId = new NetworkVariable<ulong>(0);

    public event Action<ulong> OnRoundStart;
    public event Action OnAllRoundsEnd;
    
    private List<ulong> _turnOrder  = new List<ulong>();
    private int         _totalRounds = 4;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        TimeManager.Instance.OnTimeUp += OnRoundTimeUp;
    }

    public override void OnNetworkDespawn()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeUp -= OnRoundTimeUp;
    }
    
    public void StartGame()
    {
        if (!IsServer) return;
        
        _turnOrder.Clear();
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            _turnOrder.Add(clientId);
        
        for (int i = _turnOrder.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (_turnOrder[i], _turnOrder[j]) = (_turnOrder[j], _turnOrder[i]);
        }

        _totalRounds = _turnOrder.Count;
        CurrentRound.Value = 0;
        StartRound();
    }

    private void StartRound()
    {
        if (!IsServer) return;
        
        if (CurrentRound.Value >= _totalRounds)
        {
            NotifyAllRoundsEndClientRpc();
            return;
        }

        ulong catcherId = _turnOrder[CurrentRound.Value];
        CurrentCatcherId.Value = catcherId;
        
        CrossHairSpawner.Instance.SpawnCrossHairs(catcherId);
        TimeManager.Instance.StartTimer(_roundDuration);
        
        NotifyRoundStartClientRpc(catcherId);
    }
    
    private void OnRoundTimeUp()
    {
        if (!IsServer) return;

        CrossHairSpawner.Instance.DespawnCrossHairs();
        CurrentRound.Value++;
        StartRound();
    }

    [Rpc(SendTo.Everyone)]
    private void NotifyRoundStartClientRpc(ulong catcherId)
        => OnRoundStart?.Invoke(catcherId);

    [Rpc(SendTo.Everyone)]
    private void NotifyAllRoundsEndClientRpc()
    {
        // 서버에서만 점수 반영
        if (IsServer) ScoreManager.Instance.ApplyRankScore();
        OnAllRoundsEnd?.Invoke();
    }
}
