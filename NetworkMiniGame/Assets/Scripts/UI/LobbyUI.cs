using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button characterSelectButton;
    [SerializeField] private Button characterSelectCloseButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button settingCloseButton;
    [SerializeField] private Button languageChangeButton;
    [SerializeField] private Button quitButton;

    [Header("설정")]
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject settingPanel;

    [Header("캐릭터")]
    [SerializeField] private GameObject characterSpawnPoint;
    [SerializeField] private List<Button> selectButtons = new List<Button>();
    [SerializeField] private List<GameObject> characterPrefabs = new List<GameObject>();
    
    private GameObject _currentCharacter;
    
    private void Start()
    {
        // 닉네임 표시
        nickNameText.text = BackendManager.Auth.CurrentUser?.DisplayName ?? "Guest";

        // 캐릭터 선택 버튼 이벤트 등록
        for (int i = 0; i < selectButtons.Count; i++)
        {
            int index = i;
            selectButtons[i].onClick.AddListener(() => OnCharacterSelected(index));
        }

        // 기본 캐릭터 0번 생성
        SpawnLobbyCharacter(0);
    }

    private void OnEnable()  => BindButtons();
    private void OnDisable() => UnbindButtons();

    private void BindButtons()
    {
        characterSelectButton.onClick.AddListener(OpenCharacterSelect);
        characterSelectCloseButton.onClick.AddListener(CloseCharacterSelect);
        settingButton.onClick.AddListener(OpenSetting);
        settingCloseButton.onClick.AddListener(CloseSetting);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void UnbindButtons()
    {
        characterSelectButton.onClick.RemoveListener(OpenCharacterSelect);
        characterSelectCloseButton.onClick.RemoveListener(CloseCharacterSelect);
        settingButton.onClick.RemoveListener(OpenSetting);
        settingCloseButton.onClick.RemoveListener(CloseSetting);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    // ───── 캐릭터 선택 ─────
    private void OnCharacterSelected(int index)
    {
        SpawnLobbyCharacter(index);
        CharacterSelectData.Instance.SetLocalSelection(index, characterPrefabs[index]);
        CloseCharacterSelect();
    }

    private void SpawnLobbyCharacter(int index)
    {
        if (_currentCharacter != null)
            Destroy(_currentCharacter);

        _currentCharacter = Instantiate(
            characterPrefabs[index],
            characterSpawnPoint.transform.position,
            characterSpawnPoint.transform.rotation
        );
    }

    // ───── 팝업 오픈/클로즈 ─────
    private void OpenCharacterSelect()
    {
        characterSelectPanel.SetActive(true);
        SetLobbyCharacterVisible(false);
    }

    private void CloseCharacterSelect()
    {
        characterSelectPanel.SetActive(false);
        SetLobbyCharacterVisible(true);
    }

    private void OpenSetting()
    {
        settingPanel.SetActive(true);
        SetLobbyCharacterVisible(false);
    }

    private void CloseSetting()
    {
        settingPanel.SetActive(false);
        SetLobbyCharacterVisible(true);
    }

    public void SetLobbyCharacterVisible(bool visible)
    {
        if (_currentCharacter != null)
            _currentCharacter.SetActive(visible);
    }

    // ───── 기타 ─────
    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
