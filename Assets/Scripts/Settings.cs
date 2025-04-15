using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;

    [SerializeField] private Sprite[] _allSprites;

    [SerializeField] private Transform _buttonsSpawnParent;
    [SerializeField] private GameObject _buttonPrefab;

    [SerializeField] private GameObject _settingsPanel;

    [SerializeField] private GameObject _audioCheckMark;

    private void Awake()
    {
        LoadBackgroundsFromAddressables(SetPhotoToImage);

        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        _audioCheckMark.SetActive(musicEnabled);
    }
    private void LoadBackgroundsFromAddressables(Action onComplete = null)
    {
        Addressables.LoadResourceLocationsAsync("Backgrounds", typeof(Sprite)).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                int total = handle.Result.Count;
                Sprite[] tempArray = new Sprite[total];
                int loadedCount = 0;

                for (int i = 0; i < total; i++)
                {
                    int index = i;
                    var location = handle.Result[index];

                    Addressables.LoadAssetAsync<Sprite>(location).Completed += spriteHandle =>
                    {
                        if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            tempArray[index] = spriteHandle.Result;
                            loadedCount++;

                            if (loadedCount == total)
                            {
                                _allSprites = tempArray;
                                Debug.Log($"Фоны загружены в порядке: {_allSprites.Length}");
                                _backgroundImage.sprite = _allSprites[PlayerPrefs.GetInt("CurrentBackground", 0)];
                                onComplete?.Invoke();
                            }
                        }
                        else
                        {
                            Debug.LogError("Не удалось загрузить спрайт: " + location.PrimaryKey);
                        }
                    };
                }
            }
            else
            {
                Debug.LogError("Не удалось получить локации спрайтов с меткой 'Backgrounds'");
            }
        };
    }


    private void SetPhotoToImage()
    {
        for(int i = 0; i < _allSprites.Length; i++)
        {
            int index = i;
            GameObject obj = Instantiate(_buttonPrefab, _buttonsSpawnParent);
            Button button = obj.GetComponent<Button>();
            Image image = obj.GetComponent<Image>();

            image.sprite = _allSprites[index];
            button.onClick.AddListener(() => SetNewBackground(index));
        }
    }

    private void SetNewBackground(int index)
    {
        PlayerPrefs.SetInt("CurrentBackground", index);
        _backgroundImage.sprite = _allSprites[index];
        _settingsPanel.SetActive(false);
    }

    public void SwitchSoundsEnable()
    {
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        _audioCheckMark.SetActive(!musicEnabled);
        AudioManager.Instance.ToggleMusic(!musicEnabled);
        AudioManager.Instance.ToggleSFX(!musicEnabled);
    }
}
