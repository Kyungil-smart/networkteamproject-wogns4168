using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button guestLoginButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button guestButton;
    [SerializeField] private Button loginCloseButton;
    [SerializeField] private Button nickNameCloseButton;
    [SerializeField] private Button signUpCloseButton;
    [SerializeField] private Button signUpOkButton;
    [SerializeField] private Button signUpOkCloseButton;
    [SerializeField] private GameObject loginObject;
    [SerializeField] private GameObject signUpObject;
    [SerializeField] private GameObject nickNameObject;
    [SerializeField] private GameObject signUpOkObject;

    private void Awake()
    {
        loginObject.SetActive(false);
        signUpObject.SetActive(false);
        nickNameObject.SetActive(false);
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartButton);
        loginButton.onClick.AddListener(LoginButton);
        guestLoginButton.onClick.AddListener(LoginButton);
        signUpButton.onClick.AddListener(SignUpButton);
        guestButton.onClick.AddListener(GuestButton);
        loginCloseButton.onClick.AddListener(LoginCloseButton);
        nickNameCloseButton.onClick.AddListener(NicknameCloseButton);
        signUpCloseButton.onClick.AddListener(SignUpCloseButton);
        signUpOkButton.onClick.AddListener(SignUpOkButton);
    }

    public void StartButton()
    {
        loginObject.SetActive(true);
    }

    public void SignUpButton()
    {
        signUpObject.SetActive(true);
    }

    public void SignUpCloseButton()
    {
        signUpObject.SetActive(false);
    }

    public void LoginButton()
    {
        // 게스트 / 회원가입 에 따라 유저 정보 저장
        SceneManager.LoadScene("LobbyScene");
    }
    
    public void LoginCloseButton()
    {
        loginObject.SetActive(false);
    }
    
    public void GuestButton()
    {
        nickNameObject.SetActive(true);
    }

    public void NicknameCloseButton()
    {
        nickNameObject.SetActive(false);
    }

    public void SignUpOkButton()
    {
        // 회원가입 완료 패널? 액티브
        // 중복 여부 체크
        // 회원가입 정보 저장
        signUpOkObject.SetActive(true);
    }

    public void SignUpOkButtonClose()
    {
        signUpObject.SetActive(false);
        signUpOkObject.SetActive(false);
    }
}
