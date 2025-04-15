using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{
    private TMP_Text _text;
    private Image _image;

    private bool _isSelected = false;
    private bool _isFound = false;

    public int GridIndex { get; private set; }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void SetLetter(string text)
    {
        _text.text = text;
        _isFound = false;
        _isSelected = false;
        _image.color = Color.white;
    }

    public string GetCurrentText()
    {
        return _text.text;
    }

    public void Highlight()
    {
        if (_isSelected || _isFound) return; 

        _isSelected = true;
        _image.color = Color.yellow;
    }

    public void ResetState()
    {
        if (_isFound) return; 

        _isSelected = false;
        _image.color = Color.white;
    }

    public void MarkAsFound()
    {
        _isSelected = true;
        _isFound = true;
        _image.color = Color.green;
    }

    public void SetGridIndex(int index)
    {
        GridIndex = index;
    }
}
