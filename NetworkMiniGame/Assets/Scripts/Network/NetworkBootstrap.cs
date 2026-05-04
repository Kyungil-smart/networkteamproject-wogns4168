using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkBootstrap : MonoBehaviour
{
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _startClientButton;
    [SerializeField] private Button _startClientPopupButton;
    [SerializeField] private Button _startClientPopupExitButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private GameObject _joinCodePopup;
    

    private bool _isCallbacksBound;
    private LobbyUI _lobbyUI;

    private void Start()
    {
        _lobbyUI = FindFirstObjectByType<LobbyUI>();
    }

    private void OnEnable()
    {
        BindNetworkCallbacks();
        BindButtonEvents();
    }

    private void OnDisable()
    {
        UnbindNetworkCallbacks();
        UnbindButtonEvents();
    }

    // Copy 버튼 이벤트 바인딩 추가, Host/Client 핸들러가 async 버전으로 변경됨
    private void BindButtonEvents()
    {
        _startHostButton.onClick.AddListener(OnStartHostClicked);
        _startClientButton.onClick.AddListener(OnStartClientClicked);
        _startClientPopupButton.onClick.AddListener(OnStartClientPopupClicked);
        _startClientPopupExitButton.onClick.AddListener(OnStartClientPopupExitClicked);
    }

    // Copy 버튼 이벤트 해제 추가
    private void UnbindButtonEvents()
    {
        _startHostButton.onClick.RemoveListener(OnStartHostClicked);
        _startClientButton.onClick.RemoveListener(OnStartClientClicked);
        _startClientPopupButton.onClick.RemoveListener(OnStartClientPopupClicked);
        _startClientPopupExitButton.onClick.RemoveListener(OnStartClientPopupExitClicked);
    }

    private void BindNetworkCallbacks()
    {
        if (_isCallbacksBound) return;
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback  += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnServerStarted            += OnServerStarted;
        NetworkManager.Singleton.ConnectionApprovalCallback = (req, res) =>
        {
            res.Approved = true;
            res.CreatePlayerObject = false;
        };
        _isCallbacksBound = true;
    }

    private void UnbindNetworkCallbacks()
    {
        if (!_isCallbacksBound) return;
        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback  -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Singleton.OnServerStarted            -= OnServerStarted;
        _isCallbacksBound = false;
    }

    // Relay 연동으로 변경: StartHost 직접 호출 대신 Relay Allocation 생성 후 Join Code 를 InputField 에 표시
    private async void OnStartHostClicked()
    {
        try
        {
            BackendManager.Instance.JoinCode = await RelayNetworkService.Instance.StartHostWithRelayAsync();
            NetworkManager.Singleton.SceneManager.LoadScene("RoomScene", LoadSceneMode.Single);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Bootstrap] Host 시작 오류: {e.Message}");
        }
    }

    // Relay 연동으로 변경: InputField 의 Join Code 를 읽어 Relay 에 접속
    private async void OnStartClientClicked()
    {
        string joinCode = _joinCodeInputField.text.Trim();
        if (string.IsNullOrEmpty(joinCode)) return;

        try
        {
            BackendManager.Instance.JoinCode = joinCode;
            
            await RelayNetworkService.Instance.StartClientWithRelayAsync(joinCode);
        }
        catch (Exception e)
        {
            BackendManager.Instance.JoinCode = string.Empty;
            Debug.LogError($"[Bootstrap] Client 접속 오류: {e.Message}");
        }
    }

    private void OnStartClientPopupClicked()
    {
        _joinCodePopup.SetActive(true);
        _lobbyUI.SetLobbyCharacterVisible(false);
    }

    private void OnStartClientPopupExitClicked()
    {
        _joinCodePopup.SetActive(false);
        _lobbyUI.SetLobbyCharacterVisible(true);
    }

    private void OnClientConnected(ulong clientId)  => Debug.Log($"<color=green>[Network] 접속: {clientId}</color>");
    private void OnClientDisconnect(ulong clientId) => Debug.Log($"<color=red>[Network] 해제: {clientId}</color>");
    private void OnServerStarted()                  => Debug.Log("<color=green>[Network] 서버 시작</color>");
}