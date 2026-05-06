using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private Button _copyButton;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _joinCode;
    [SerializeField] private GameObject _startButtonObject;
    
    [SerializeField] private List<TextMeshProUGUI> _nickNames;
    [SerializeField] private List<GameObject> _readys;

    private string joinCode;

    private void Start()
    {
        OnStartRoom();
    }

    private void OnEnable()
    {
        BindButtonEvents();
    }

    private void OnDisable()
    {
        UnBindButtonEvents();
    }

    private void BindButtonEvents()
    {
        _disconnectButton.onClick.AddListener(OnDisconnectClicked);
        _copyButton.onClick.AddListener(OnCopyClicked);
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void UnBindButtonEvents()
    {
        _disconnectButton.onClick.RemoveListener(OnDisconnectClicked);
        _copyButton.onClick.RemoveListener(OnCopyClicked);
        _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    private void OnDisconnectClicked()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
    }

    private void OnCopyClicked() => GUIUtility.systemCopyBuffer = joinCode;

    private void OnStartRoom()
    {
        joinCode = BackendManager.Instance.JoinCode;
        _joinCode.text = $"Number : {joinCode}";
        
        RoomInfo[] existingPlayers = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        foreach (var player in existingPlayers)
        {
            RegisterPlayer(player);
        }
    }
    
    public void RegisterPlayer(RoomInfo player)
    {
        player.PlayerIndex.OnValueChanged += (prev, current) => UpdateSlot(player);
        player.PlayerName.OnValueChanged += (prev, current) => UpdateSlot(player);
        player.IsReady.OnValueChanged += (prev, current) => 
        {
            UpdateSlot(player);
            StartButtonActive(); 
        };

        // 처음 생성되었을 때 한 번 실행
        UpdateSlot(player);
        StartButtonActive(); 
    }
    
    public void UpdateSlot(RoomInfo player)
    {
        int index = player.PlayerIndex.Value;
        if (index < 0) return;
        
        _nickNames[index].gameObject.SetActive(true);
        _nickNames[index].text = player.PlayerName.Value.ToString();
        if (index < _readys.Count)
        {
            _readys[index].SetActive(player.IsReady.Value);
        }
    }
    
    public void OnReadyButtonClicked()
    {
        RoomInfo[] allInfos = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        foreach (var info in allInfos)
        {
            if (info.IsOwner)
            {
                info.ToggleReadyServerRpc();
                break;
            }
        }
        StartButtonActive();
    }

    private void OnStartButtonClicked()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
    
    public void ClearSlot(int index)
    {
        if (index < 0 || index >= _nickNames.Count) return;
        
        if (_nickNames[index] == null) return;
        
        _nickNames[index].text = "Empty";
        
        if (index < _readys.Count) _readys[index].SetActive(false);
        
        Invoke(nameof(StartButtonActive), 0.1f);
    }

    public void StartButtonActive()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            _startButtonObject.SetActive(false);
            return;
        }

        RoomInfo[] players = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        
        if (players.Length < 2)
        {
            _startButtonObject.SetActive(false);
            return;
        }
        bool isAllReady = true;
        foreach (var player in players)
        {
            if (!player.IsReady.Value)
            {
                isAllReady = false;
                break;
            }
        }
        _startButtonObject.SetActive(isAllReady);
    }
}