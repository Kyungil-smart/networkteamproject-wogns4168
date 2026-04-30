using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Button _copyButton;
    [SerializeField] private TextMeshProUGUI _joinCode;

    private string joinCode;

    private void Start()
    {
        OnStartRoom();
    }

    private void OnEnable()
    {
        _disconnectButton.onClick.AddListener(OnDisconnectClicked);
        _copyButton.onClick.AddListener(OnCopyClicked);
    }

    private void OnDisable()
    {
        _disconnectButton.onClick.RemoveListener(OnDisconnectClicked);
        _copyButton.onClick.RemoveListener(OnCopyClicked);
    }

    private void OnDisconnectClicked()
    {
        NetworkManager.Singleton.Shutdown();
        // 로비씬 전환
    }

    private void OnCopyClicked() => GUIUtility.systemCopyBuffer = joinCode;

    private void OnStartRoom()
    {
        joinCode = BackendManager.Instance.JoinCode;
        _joinCode.text = $"Number : {joinCode}";
    }
}