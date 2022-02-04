using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject sceneOne;
    [SerializeField] GameObject sceneTwo;

    [SerializeField] GameObject player;
    [SerializeField] Transform newSpawnpoint;

    void Start()
    {
        sceneOne.SetActive(true);
        sceneTwo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut"))
        {
            StartCoroutine(waiter());
        }
    }

    public void FadeToLevel(int levelIndex)
    {
        player.transform.position = newSpawnpoint.position;
        player.transform.rotation = newSpawnpoint.rotation;

        animator.SetTrigger("FadeIn");

        sceneOne.SetActive(false);
        sceneTwo.SetActive(true);
        //Debug.Log("Switch World");
    }

    public void StartFade(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //FadeToLevel(1);
            animator.Play("FadeOut");
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(1);
        FadeToLevel(1);
    }
}
