using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    private Dictionary<ulong, float> _gameScores  = new Dictionary<ulong, float>();
    
    private Dictionary<ulong, int> _totalScores = new Dictionary<ulong, int>();
    
    private static readonly int[] RankPoints = { 4, 3, 2, 1 };

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddGameScore(ulong clientId, float amount)
    {
        if (!_gameScores.ContainsKey(clientId)) _gameScores[clientId] = 0;
        _gameScores[clientId] += amount;
    }
    public void ApplyRankScore()
    {
        var sorted = _gameScores.OrderByDescending(kv => kv.Value).ToList();

        int rank = 1;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (i > 0 && sorted[i].Value < sorted[i - 1].Value)
                rank = i + 1;

            ulong clientId  = sorted[i].Key;
            int   rankPoint = rank <= RankPoints.Length ? RankPoints[rank - 1] : 1;

            if (!_totalScores.ContainsKey(clientId)) _totalScores[clientId] = 0;
            _totalScores[clientId] += rankPoint;

            Debug.Log($"[Score] clientId:{clientId} 게임점수:{sorted[i].Value:F0} → {rank}등 → +{rankPoint}점");
        }
        
        _gameScores.Clear();
    }
    
    public float GetGameScore(ulong clientId)
        => _gameScores.TryGetValue(clientId, out float s) ? s : 0f;
    
    public int GetTotalScore(ulong clientId)
        => _totalScores.TryGetValue(clientId, out int s) ? s : 0;
    
    public List<(ulong clientId, int score, int rank)> GetTotalRankings()
    {
        var sorted = _totalScores.OrderByDescending(kv => kv.Value).ToList();
        var result = new List<(ulong, int, int)>();

        int rank = 1;
        for (int i = 0; i < sorted.Count; i++)
        {
            if (i > 0 && sorted[i].Value < sorted[i - 1].Value)
                rank = i + 1;
            result.Add((sorted[i].Key, sorted[i].Value, rank));
        }
        return result;
    }
    
    public void ResetAll()
    {
        _gameScores.Clear();
        _totalScores.Clear();
    }
}