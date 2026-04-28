using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("Buttons")]
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
    [SerializeField] private Button signUpFailCloseButton;
    [SerializeField] private Button loginFailCloseButton;
    [Header("GameObjects")]
    [SerializeField] private GameObject loginObject;
    [SerializeField] private GameObject signUpObject;
    [SerializeField] private GameObject nickNameObject;
    [SerializeField] private GameObject signUpOkObject;
    [SerializeField] private GameObject signUpFailObject;
    [SerializeField] private GameObject loginFailObject;
    [Header("InputFields")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField signUpEmailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField signUpPasswordInputField;
    [SerializeField] private TMP_InputField guestNickNameInputField;
    [SerializeField] private TMP_InputField nickNameInputField;

    private void Awake()
    {
        loginObject.SetActive(false);
        signUpObject.SetActive(false);
        nickNameObject.SetActive(false);
    }

    private void OnEnable() => BindButtonEvents();
    private void OnDisable() => UnBindButtonEvents();

    private void BindButtonEvents()
    {
        startButton.onClick.AddListener(StartButton);
        loginButton.onClick.AddListener(LoginButton);
        guestLoginButton.onClick.AddListener(GuestLoginButton);
        signUpButton.onClick.AddListener(SignUpButton);
        guestButton.onClick.AddListener(GuestButton);
        loginCloseButton.onClick.AddListener(LoginCloseButton);
        nickNameCloseButton.onClick.AddListener(NicknameCloseButton);
        signUpCloseButton.onClick.AddListener(SignUpCloseButton);
        signUpOkButton.onClick.AddListener(SignUpOkButton);
        signUpOkCloseButton.onClick.AddListener(SignUpOkButtonClose);
        signUpFailCloseButton.onClick.AddListener(SignUpFailButtonClose);
        loginFailCloseButton.onClick.AddListener(LoginFailButtonClose);
    }
    
    private void UnBindButtonEvents()
    {
        startButton.onClick.RemoveListener(StartButton);
        loginButton.onClick.RemoveListener(LoginButton);
        guestLoginButton.onClick.RemoveListener(GuestLoginButton);
        signUpButton.onClick.RemoveListener(SignUpButton);
        guestButton.onClick.RemoveListener(GuestButton);
        loginCloseButton.onClick.RemoveListener(LoginCloseButton);
        nickNameCloseButton.onClick.RemoveListener(NicknameCloseButton);
        signUpCloseButton.onClick.RemoveListener(SignUpCloseButton);
        signUpOkButton.onClick.RemoveListener(SignUpOkButton);
        signUpOkCloseButton.onClick.RemoveListener(SignUpOkButtonClose);
        signUpFailCloseButton.onClick.RemoveListener(SignUpFailButtonClose);
        loginFailCloseButton.onClick.RemoveListener(LoginFailButtonClose);
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
        BackendManager.Instance.LogIn(emailInputField.text, passwordInputField.text);
    }
    
    public void GuestLoginButton()
    {
        BackendManager.Instance.GuestLogIn(guestNickNameInputField.text);
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
        BackendManager.Instance.SignUp(signUpEmailInputField.text, signUpPasswordInputField.text, nickNameInputField.text);
    }

    public void SignUpSuccess()
    {
        signUpOkObject.SetActive(true);
    }

    public void SignUpOkButtonClose()
    {
        signUpObject.SetActive(false);
        signUpOkObject.SetActive(false);
    }

    public void SignUpFailButtonClose()
    {
        signUpFailObject.SetActive(false);
    }

    public void LoginFailButtonClose()
    {
        loginFailObject.SetActive(false);
    }

    public void SignUpFail()
    {
        signUpFailObject.SetActive(true);
    }

    public void LoginFail()
    {
        loginFailObject.SetActive(true);
    }
}
