using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadScene(string key)
    {
        Addressables.LoadSceneAsync(key, LoadSceneMode.Single);
    }
}

