using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GameBackground : MonoBehaviour
{
    [SerializeField] private Image _backGround;

    private void Awake()
    {
        int savedIndex = PlayerPrefs.GetInt("CurrentBackground", 0);

        Addressables.LoadResourceLocationsAsync("Backgrounds", typeof(Sprite)).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (savedIndex < handle.Result.Count)
                {
                    var location = handle.Result[savedIndex];

                    Addressables.LoadAssetAsync<Sprite>(location).Completed += spriteHandle =>
                    {
                        if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            _backGround.sprite = spriteHandle.Result;
                        }
                        else
                        {
                            Debug.LogError("Не удалось загрузить фон.");
                        }
                    };
                }
                else
                {
                    Debug.LogWarning("Сохранённый индекс выходит за пределы списка спрайтов.");
                }
            }
            else
            {
                Debug.LogError("Не удалось получить локации фонов.");
            }
        };
    }
}
