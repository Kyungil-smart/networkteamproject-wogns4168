using Unity.Netcode;
using UnityEngine;
using TMPro;

public class CrossHairMove : NetworkBehaviour
{
    [Header("플레이어 이름 표시 텍스트")]
    [SerializeField] private TextMeshProUGUI _nameText;

    [Header("초당 획득 점수 (캐쳐가 러너와 겹칠 때)")]
    [SerializeField] private float _scorePerSecond = 10f;

    [Header("플레이어 인덱스별 색상 (0~3)")]
    [SerializeField] private Color[] _playerColors =
    {
        Color.red,
        new Color(1f, 0.5f, 0f),
        Color.yellow,
        Color.green
    };
    
    public NetworkVariable<bool> IsCatcher = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
    public NetworkVariable<int> PlayerIdx = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Camera _cam;

    public override void OnNetworkSpawn()
    {
        _cam = Camera.main;
        
        PlayerIdx.OnValueChanged += (_, _) => ApplyColor();
        ApplyColor();
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void InitServerRpc(int playerIndex, bool isCatcher)
    {
        PlayerIdx.Value  = playerIndex;
        IsCatcher.Value  = isCatcher;

        // 닉네임 찾아서 모든 클라이언트에 표시
        RoomInfo[] players = FindObjectsByType<RoomInfo>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p.PlayerIndex.Value == playerIndex)
            {
                SetNameClientRpc(p.PlayerName.Value.ToString());
                break;
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetNameClientRpc(string playerName)
    {
        if (_nameText != null) _nameText.text = playerName;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!IsServer || !IsCatcher.Value) return;

        if (other.TryGetComponent<CrossHairMove>(out var otherAim) && !otherAim.IsCatcher.Value)
        {
            // int → float으로 변경 (정밀도 유지)
            ScoreManager.Instance.AddGameScore(OwnerClientId, _scorePerSecond * Time.deltaTime);
        }
    }
    
    private void ApplyColor()
    {
        if (PlayerIdx.Value < 0 || PlayerIdx.Value >= _playerColors.Length) return;

        Color color = _playerColors[PlayerIdx.Value];
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.color = color;
    }
}