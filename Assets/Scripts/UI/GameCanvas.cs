using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    [SerializeField]
    private string[] _endMessage =
    {
        "Отлично справился!",
        "Все слова найдены — ты молодец!",
        "Это было круто! Еще раз?",
        "Ты раскрыл весь потенциал!",
        "Слова сдались тебе без боя!",
        "Мощно! Попробуем посложнее?"
    };

    [SerializeField] private TMP_Text _endMessageText;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _toMenuButton;

    [SerializeField] private GameObject _winOrLosePanel;

    [Header("Pause menu")]

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _restartPauseButton;
    [SerializeField] private Button _toMenuPauseButton;

    [Header("GUI")]

    [SerializeField] private TMP_Text _currentWordText;


    private bool _gameIsEnd = false;

    public static GameCanvas Instance;

    private void Awake()
    {
        Instance = this;

        _currentWordText.text = "";
        _endMessageText.text = _endMessage[Random.Range(0, _endMessage.Length)];
        _restartButton.onClick.AddListener(() => SceneLoader.LoadScene("GameScene"));
        _toMenuButton.onClick.AddListener(() => SceneLoader.LoadScene("MenuScene"));
        _restartPauseButton.onClick.AddListener(() => SceneLoader.LoadScene("GameScene"));
        _toMenuPauseButton.onClick.AddListener(() => SceneLoader.LoadScene("MenuScene"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameIsEnd)
        {
            _pauseMenu.SetActive(!_pauseMenu.activeSelf);
        }
    }
    public void ShowPanel()
    {
        _winOrLosePanel.SetActive(true);
        _gameIsEnd = true;
    }

    public void SetCurrentWord(string word = "", bool set = false)
    {
        if (set) _currentWordText.text = word;
        else _currentWordText.text += word;
    }

}
