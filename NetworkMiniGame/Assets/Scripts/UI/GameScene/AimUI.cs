using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AimUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Image backGround;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        backGround.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void Update()
    {
        time.text = TimeManager.Instance.GetTime().ToString("F2");
        score.text = ScoreManager.Instance.GetGameScore(TurnManager.Instance.CurrentCatcherId.Value).ToString("F2");
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartGame);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        startButton.gameObject.SetActive(false);
        backGround.gameObject.SetActive(false);
        TurnManager.Instance.StartGame();
    }
}