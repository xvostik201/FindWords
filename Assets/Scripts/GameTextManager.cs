using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class GameTextManager : MonoBehaviour
{
    [SerializeField] private TextAsset _gameLettersText;

    private static List<string> _letters = new List<string>();
    private static List<string> _words = new List<string>();

    public static GameTextManager Instance;

    private bool _isLoaded = false;

    private void Awake()
    {
        Instance = this;
        LoadLetters();

        string themeKey = PlayerPrefs.GetString("GameWordKey", "default_theme");

        LoadWordsFromAddressable(themeKey);
    }

    private void LoadLetters()
    {
        _letters.Clear();
        string[] lines = _gameLettersText.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                _letters.Add(trimmed);
        }
    }

    public void LoadWordsFromAddressable(string key, Action onLoaded = null)
    {
        if (_isLoaded) return;

        Addressables.LoadAssetAsync<TextAsset>(key).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _words.Clear();

                _isLoaded = true;

                string[] lines = handle.Result.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    string[] parts = trimmed.Split('-');
                    string word = string.Concat(parts);
                    if (!string.IsNullOrEmpty(word))
                        _words.Add(word);
                }

                Debug.Log($"Тема '{key}' загружена. Слов: {_words.Count}");
            }
            else
            {
                Debug.LogError("Не удалось загрузить тему: " + key);
            }

            onLoaded?.Invoke();
        };
    }


    public static string GetRandomLetter()
    {
        if (_letters.Count == 0)
        {
            Debug.LogError("Список букв пуст!");
            return "?";
        }
        return _letters[Random.Range(0, _letters.Count)];
    }

    public static string GetRandomWord()
    {
        if (_words.Count == 0)
        {
            Debug.LogError("Список слов пуст!");
            return "тест";
        }
        return _words[Random.Range(0, _words.Count)];
    }
}
