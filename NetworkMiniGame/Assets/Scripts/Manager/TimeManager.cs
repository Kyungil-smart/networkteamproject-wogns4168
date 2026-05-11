using Unity.Netcode;
using UnityEngine;
using System;

public class TimeManager : NetworkBehaviour
{
    public static TimeManager Instance { get; private set; }
    
    public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    
    public event Action OnTimeUp;

    private bool _isRunning = false; 

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (!IsServer || !_isRunning) return;

        TimeRemaining.Value -= Time.deltaTime;
        
        if (TimeRemaining.Value <= 0f)
        {
            TimeRemaining.Value = 0f;
            _isRunning = false;
            NotifyTimeUpClientRpc();
        }
    }
    
    public void StartTimer(float duration)
    {
        if (!IsServer) return;
        TimeRemaining.Value = duration;
        _isRunning = true;
    }
    
    public void StopTimer()
    {
        if (!IsServer) return;
        _isRunning = false;
    }
    
    [Rpc(SendTo.Everyone)]
    private void NotifyTimeUpClientRpc()
    {
        OnTimeUp?.Invoke();
    }
}