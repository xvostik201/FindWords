using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class InitLoader : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";

    void Start()
    {
        Addressables.LoadSceneAsync(menuSceneName, LoadSceneMode.Single).Completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Menu scene loaded from Addressables");
        }
        else
        {
            Debug.LogError("Failed to load scene from Addressables");
        }
    }
}
