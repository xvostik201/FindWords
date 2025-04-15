using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitButton;

    [Header("Game settings")]
    [SerializeField] private Transform _themeSpawnTransform;
    [SerializeField] private GameObject _themeSpawnPrefab;

    [SerializeField] private GameObject _difficultObjToActivate;
    [SerializeField] private GameObject _themeObjToDeactivate;

    [SerializeField] private Button[] _difficultButtons;

    [SerializeField] private List<GameObject> _objectsToHide = new List<GameObject>();

    private void Awake()
    {
        LoadThemesFromAddressables();
        InitializateDifficultButtons();

        _exitButton.onClick.AddListener(QuitApplication);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _objectsToHide.Count != 0)
        {
            foreach (GameObject obj in _objectsToHide)
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                    break;
                }
            }
        }
    }

    private void LoadThemesFromAddressables()
    {
        Addressables.LoadResourceLocationsAsync("WordThemes", typeof(TextAsset)).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (IResourceLocation location in handle.Result)
                {
                    string themeKey = location.PrimaryKey;

                    GameObject obj = Instantiate(_themeSpawnPrefab, _themeSpawnTransform);
                    Button button = obj.GetComponent<Button>();
                    TMP_Text text = obj.GetComponentInChildren<TMP_Text>();

                    text.text = System.IO.Path.GetFileNameWithoutExtension(themeKey);

                    button.onClick.AddListener(() => _difficultObjToActivate.SetActive(true));
                    button.onClick.AddListener(() => _themeObjToDeactivate.SetActive(false));
                    button.onClick.AddListener(() => SetThemeKey(themeKey));
                }
            }
            else
            {
                Debug.LogError("Не удалось загрузить список тем из Addressables.");
            }
        };
    }
    public void LoadScene(string sceneId, int difficultIndex)
    {
        SceneLoader.LoadScene(sceneId);
        PlayerPrefs.SetInt("GameGridSize", difficultIndex);
        Debug.Log("Difficult level " + difficultIndex);
    }

    private void SetThemeKey(string key)
    {
        PlayerPrefs.SetString("GameWordKey", key);
        Debug.Log("Выбрана тема: " + key);
    }

    private void InitializateDifficultButtons()
    {
        for(int i = 0; i < _difficultButtons.Length; i++)
        {
            int index = i;
            _difficultButtons[i].onClick.AddListener(() => LoadScene("GameScene", index));
        }
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

}
