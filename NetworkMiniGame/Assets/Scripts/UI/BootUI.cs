using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootUI : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;

    private float loadingTime;
    private string loadText;
    private float textTime;
    
    AuthService authService;

    private void Awake()
    {
        loadingSlider.value = 0;
        authService = FindFirstObjectByType<AuthService>();
    }

    private async void Start()
    {
        await authService.InitializeAsync();
    }

    private void Update()
    {
        loadingTime += Time.deltaTime;
        if (loadingTime > 1)
        {
            loadingSlider.value += (float)0.25;
            textTime = loadingSlider.value * 100;
            loadText = textTime.ToString();
            loadingText.text = $"Loading... {loadText}%";
            loadingTime = 0;
            if (loadingSlider.value >= 0.9) SceneManager.LoadScene("LobbyScene");
        }
    }
}
