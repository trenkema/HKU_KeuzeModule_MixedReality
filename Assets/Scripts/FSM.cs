using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FSM : MonoBehaviour
{
    public Animator animator;
    public float waitBeforeLoading = 0f;

    public void StartGame(string _levelName)
    {
        animator.SetTrigger("StartGame");
        StartCoroutine(LoadAsyncScene(_levelName));
    }

    IEnumerator LoadAsyncScene(string _sceneName)
    {
        yield return new WaitForSeconds(waitBeforeLoading);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);
        asyncLoad.allowSceneActivation = true;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
