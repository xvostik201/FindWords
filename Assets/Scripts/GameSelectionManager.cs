using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.PackageManager;

public class GameSelectionManager : MonoBehaviour
{
    public static GameSelectionManager Instance;

    [SerializeField] private float _comparisonError = 1f;

    [SerializeField] private GraphicRaycaster _raycaster;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameField _gameField;

    private List<Letter> _selectedLetters = new List<Letter>();
    private bool _isDraggingWord = false;

    private bool _directionLocked = false;
    private bool _isHorizontal = false;

    private void Awake()
    {
        Instance = this;

        if (_raycaster == null)
            _raycaster = FindObjectOfType<GraphicRaycaster>();

        if (_eventSystem == null)
            _eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDraggingWord = true;
            ResetSelection();
        }

        if (Input.GetMouseButton(0) && _isDraggingWord)
        {
            PointerEventData pointerData = new PointerEventData(_eventSystem)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            _raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Letter letter = result.gameObject.GetComponent<Letter>();

                if (letter != null)
                {
                    if (!_selectedLetters.Contains(letter))
                    {
                        if (_selectedLetters.Count == 1)
                        {
                            Vector2 first = ((RectTransform)_selectedLetters[0].transform).anchoredPosition;
                            Vector2 second = ((RectTransform)letter.transform).anchoredPosition;

                            _isHorizontal = Mathf.Abs(first.x - second.x) > Mathf.Abs(first.y - second.y);
                            _directionLocked = true;
                        }

                        if (_selectedLetters.Count >= 1 && _directionLocked)
                        {
                            Vector2 last = ((RectTransform)_selectedLetters.Last().transform).anchoredPosition;
                            Vector2 current = ((RectTransform)letter.transform).anchoredPosition;

                            if (_isHorizontal && Mathf.Abs(last.y - current.y) > _comparisonError) return;
                            if (!_isHorizontal && Mathf.Abs(last.x - current.x) > _comparisonError) return;
                            
                        }
                        //if (_selectedLetters.Count >= 1)
                        //{
                        //    int lastIndex = _selectedLetters.Last().GridIndex;
                        //    int currentIndex = letter.GridIndex;
                        //    int step = _isHorizontal ? 1 : _gameField.Width;

                        //    if (Mathf.Abs(currentIndex - lastIndex) != step)
                        //        return;
                        //}

                        letter.Highlight();
                        _selectedLetters.Add(letter);
                        SetCurrentText();
                    }
                    else if (_selectedLetters.Count >= 2 && _selectedLetters[_selectedLetters.Count - 2] == letter)
                    {
                        var last = _selectedLetters.Last();
                        last.ResetState();
                        _selectedLetters.RemoveAt(_selectedLetters.Count - 1);
                        SetCurrentText();
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _isDraggingWord)
        {
            _isDraggingWord = false;
            _directionLocked = false;

            string selectedWord = GetSelectedWord();
            if (_gameField.IsTargetWord(selectedWord))
            {
                foreach (var letter in _selectedLetters)
                    letter.MarkAsFound();

                _gameField.StrikeOutWordInList(selectedWord);
            }
            else
            {
                ResetSelection();
            }

            GameCanvas.Instance.SetCurrentWord("",true);
            _selectedLetters.Clear();
        }
    }

    private void SetCurrentText()
    {
        string word = "";
        foreach (Letter letterChar in _selectedLetters)
        {
            word += letterChar.GetCurrentText();
        }
        GameCanvas.Instance.SetCurrentWord(word, true);
    }

    private string GetSelectedWord()
    {
        string word = "";
        foreach (var l in _selectedLetters)
            word += l.GetCurrentText();
        return word;
    }

    public void ResetSelection()
    {
        foreach (var letter in _selectedLetters)
            letter.ResetState();

        _selectedLetters.Clear();
        _directionLocked = false;
    }
}
