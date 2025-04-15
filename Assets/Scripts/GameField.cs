using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private RectTransform _scrollView;
    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;

    [Header("Play Field Settings")]
    [Tooltip("Number of words to spawn on the field")]
    [SerializeField] private int _wordsToSpawn = 5;

    [Tooltip("Grid size: X = width, Y = height")]
    [SerializeField] private Vector2[] _buttonsToSpawn;

    private Vector2 _currentGridSize = Vector2.zero;

    [SerializeField] private Transform _targetWordsParent;
    [SerializeField] private GameObject _targetWordPrefab;


    private List<string> _targetWords = new List<string>();
    private List<GameObject> _targetWordObjects = new List<GameObject>();
    private List<GameObject> _spawnedButtons = new List<GameObject>();

    public int Width => (int)_currentGridSize.x;
    public int Height => (int)_currentGridSize.y;

    public static GameField Instance;

    private void Awake()
    {
        Instance = this;

        InitializateGrid();
        SpawnField();
    }

    private void InitializateGrid()
    {
        int grid = PlayerPrefs.GetInt("GameGridSize", 0);
        _currentGridSize = _buttonsToSpawn[grid];
    }

    private void Start()
    {
        string key = PlayerPrefs.GetString("GameWordKey", "default_theme");
        GameTextManager.Instance.LoadWordsFromAddressable(key, () =>
        {
            Debug.Log("Слова загружены, создаём поле...");
            SpawnWords();
            SpawnLetters();
        });
    }

    private void SpawnField()
    {
        _gridLayoutGroup.cellSize = new Vector2(
            _scrollView.sizeDelta.y / _currentGridSize.x,
            _scrollView.sizeDelta.y / _currentGridSize.y);

        int total = (int)(_currentGridSize.x * _currentGridSize.y);

        for (int i = 0; i < total; i++)
        {
            GameObject newLetter = Instantiate(_buttonPrefab, _scrollViewContent);
            _spawnedButtons.Add(newLetter);
        }
    }

    private void SpawnLetters()
    {
        for(int i = 0; i < _spawnedButtons.Count; i++)
        {
            Letter letter = _spawnedButtons[i].GetComponent<Letter>();
            if (string.IsNullOrEmpty(letter.GetCurrentText()))
            {
                letter.SetGridIndex(i);
                letter.SetLetter(GameTextManager.GetRandomLetter());
            }
        }
    }

    private void SpawnWords()
    {
        _targetWords.Clear();
        _targetWordObjects.Clear();

        int tries = 0;
        int maxTries = 100;

        while (_targetWords.Count < _wordsToSpawn && tries < maxTries)
        {
            string word = GameTextManager.GetRandomWord();
            if (_targetWords.Contains(word)) { tries++; continue; }

            bool placed = PlaceWordOnGrid(word);
            if (placed)
            {
                _targetWords.Add(word);
                GameObject wordUI = Instantiate(_targetWordPrefab, _targetWordsParent);
                wordUI.GetComponentInChildren<TMP_Text>().text = word;
                _targetWordObjects.Add(wordUI);
            }
            else
            {
                Debug.Log($"Не удалось разместить слово: {word}");
            }

            tries++;
        }

        if (_targetWords.Count < _wordsToSpawn)
        {
            Debug.LogWarning($"Удалось заспавнить только {_targetWords.Count} из {_wordsToSpawn} слов");
        }
    }

    private bool PlaceWordOnGrid(string word)
    {
        char[] letters = word.ToCharArray();
        bool placeHorizontally = Random.value > 0.5f;

        int maxStartX = placeHorizontally ? Width - letters.Length : Width;
        int maxStartY = placeHorizontally ? Height : Height - letters.Length;

        if (maxStartX < 0 || maxStartY < 0)
        {
            Debug.Log($"Слово «{word}» слишком длинное для поля {Width}x{Height}, пропускаем.");
            return false;
        }

        for (int attempt = 0; attempt < 10; attempt++)
        {
            int startX = Random.Range(0, maxStartX + 1);
            int startY = Random.Range(0, maxStartY + 1);

            List<int> indices = new List<int>();
            bool conflict = false;

            for (int i = 0; i < letters.Length; i++)
            {
                int index = placeHorizontally
                    ? startY * Width + (startX + i)
                    : (startY + i) * Width + startX;

                if (index >= _spawnedButtons.Count)
                {
                    conflict = true;
                    break;
                }

                Letter letter = _spawnedButtons[index].GetComponent<Letter>();
                string current = letter.GetCurrentText();

                if (!string.IsNullOrEmpty(current) && current != letters[i].ToString())
                {
                    conflict = true;
                    break;
                }

                indices.Add(index);
            }

            if (!conflict && indices.Count == letters.Length)
            {
                for (int i = 0; i < indices.Count; i++)
                {
                    Letter letter = _spawnedButtons[indices[i]].GetComponent<Letter>();
                    letter.SetLetter(letters[i].ToString());
                }

                return true;
            }
        }

        return false;
    }

    public bool IsTargetWord(string word)
    {
        return _targetWords.Contains(word);
    }
    public void StrikeOutWordInList(string word)
    {
        GameObject toRemove = null;

        foreach (GameObject obj in _targetWordObjects)
        {
            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            if (text != null && text.text == word)
            {
                text.fontStyle = FontStyles.Strikethrough;
                text.color = Color.gray;
                toRemove = obj;
            }
        }

        if (toRemove != null)
        {
            _targetWordObjects.Remove(toRemove);
        }

        Debug.Log($"Слов осталось — {_targetWordObjects.Count}");

        if (_targetWordObjects.Count == 0)
        {
            Debug.Log("Игра окончена!");
            GameCanvas.Instance.ShowPanel();
        }
    }
}
