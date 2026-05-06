using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class RoomInfo : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<int> PlayerIndex = new NetworkVariable<int>(0);
    public NetworkVariable<bool> IsReady = new NetworkVariable<bool>(false);
    
    public NetworkVariable<int> CharacterIndex = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerIndex.Value = GetAvailableIndex();
            if (IsOwner)
            {
                PlayerName.Value = GetAuthDisplayName();
            }
        }
        if (IsOwner && !IsServer)
        {
            SetNicknameServerRpc(GetAuthDisplayName());
        }
        
        if (IsOwner)
        {
            SetCharacterIndexServerRpc(CharacterSelectData.Instance.LocalCharacterIndex);
        }
        
        DelayedRegister();
    }

    public override void OnNetworkDespawn()
    {
        var roomUI = FindFirstObjectByType<RoomUI>();
        if (roomUI != null)
        {
            roomUI.ClearSlot(PlayerIndex.Value);
        }
    }

    [ServerRpc]
    private void SetNicknameServerRpc(string name)
    {
        PlayerName.Value = name;
        
        if (IsServer) 
        {
            var roomUI = FindFirstObjectByType<RoomUI>();
            if (roomUI != null) roomUI.UpdateSlot(this);
        }
    }

    [ServerRpc]
    public void ToggleReadyServerRpc()
    {
        IsReady.Value = !IsReady.Value;
        
        if (IsServer) 
        {
            var roomUI = FindFirstObjectByType<RoomUI>();
            if (roomUI != null) 
            {
                roomUI.UpdateSlot(this);
                roomUI.StartButtonActive(); // 여기서도 직접 버튼 체크 실행
            }
        }
    }
    
    [ServerRpc]
    private void SetCharacterIndexServerRpc(int index)
    {
        CharacterIndex.Value = index;
    }
    
    private int GetAvailableIndex()
    {
        RoomInfo[] allPlayers = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        
        List<int> usedIndices = new List<int>();
        foreach (var p in allPlayers)
        {
            if (p != this && p.PlayerIndex.Value >= 0) 
                usedIndices.Add(p.PlayerIndex.Value);
        }
        
        for (int i = 0; i < 4; i++)
        {
            if (!usedIndices.Contains(i)) return i;
        }
        return 0;
    }
    
    private string GetAuthDisplayName()
    {
        if (BackendManager.Auth.CurrentUser != null && !string.IsNullOrEmpty(BackendManager.Auth.CurrentUser.DisplayName))
        {
            return BackendManager.Auth.CurrentUser.DisplayName;
        }
        return "Guest";
    }
    
    private void DelayedRegister()
    {
        var roomUI = FindFirstObjectByType<RoomUI>();
        if (roomUI != null) 
        {
            roomUI.RegisterPlayer(this);
            roomUI.UpdateSlot(this);
        }
    }
}
