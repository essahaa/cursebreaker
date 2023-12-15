using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private GameObject loadingScreen;
    public GameObject loadingScreenPrefab;

    private void Start()
    {
        GameObject[] loaders = GameObject.FindGameObjectsWithTag("SceneLoader");

        if (loaders.Length > 1)
        {
            // Another loader already exists, destroy this one
            Destroy(gameObject);
        }
        else
        {
            // Make the object this script is attached to, not destroy on scene load
            DontDestroyOnLoad(gameObject);
        }

        GameObject[] loadingScreens = GameObject.FindGameObjectsWithTag("LoadingScreen");

        if (loadingScreens.Length <= 0)
        {
            loadingScreen = Instantiate(loadingScreenPrefab, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(loadingScreen);
        }
    }

    public void LoadScene(string sceneToLoad)
    {
        // Display the loading screen
        loadingScreen.SetActive(true);

        // Start an asynchronous operation to load the target scene
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        yield return new WaitForSeconds(0.1f);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        // Wait until the asynchronous operation is complete
        while (!asyncOperation.isDone)
        {
            // Update your loading screen with progress (e.g., update a progress bar)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            yield return null; // This line is essential for the coroutine to work properly
        }
        loadingScreen.SetActive(false);
    }
}

