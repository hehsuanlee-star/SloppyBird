using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _scoreUI;
    private TMP_Text _score;
    [SerializeField] private GameObject _gameOverTitle;
    [SerializeField] private GameObject _gameTitle;

    private void Awake()
    {
        _gameOverTitle.SetActive(false);
        _gameTitle.SetActive(false);
        _score = _scoreUI.GetComponent<TMP_Text>();
    }

    public event Action GameStart;
    public void OnButtonClicked()
    { 
        GameStart?.Invoke();
        _gameOverTitle.SetActive(false);
        _gameTitle.SetActive(false);
    }
    public void ScoreUpdate(int score)
    {
        _score.text = ($"Score: {score}");
    }

    public void ShowGameTitle()
    {
        _gameTitle.SetActive(true);
    }
    public void ShowGameOver()
    {
        _gameOverTitle.SetActive(true);
    }
}
