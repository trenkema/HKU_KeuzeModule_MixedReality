using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightSeeingManager : MonoBehaviour
{
    [SerializeField] GameObject[] sightSeeingArrows;

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip successSound;

    [SerializeField] Animator transitionAnimator;

    [SerializeField] GameObject[] scenes;

    [SerializeField] GameObject player;

    [SerializeField] Transform[] spawnPoints;

    int index = 0;

    int sceneIndex = 0;

    private void Start()
    {
        foreach (var arrow in sightSeeingArrows)
        {
            arrow.SetActive(false);
        }

        sightSeeingArrows[0].SetActive(true);

        foreach (var scene in scenes)
        {
            scene.SetActive(false);
        }

        scenes[0].SetActive(true);
    }

    public void NextSightSeeing()
    {
        audioSource.PlayOneShot(successSound);

        sightSeeingArrows[index].SetActive(false);

        index++;

        if (index <= sightSeeingArrows.Length - 1)
        {
            sightSeeingArrows[index].SetActive(true);
        }
        else
        {
            NextTransition();
        }
    }

    private void NextTransition()
    {
        transitionAnimator.Play("FadeOut");

        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(1f);

        scenes[sceneIndex].SetActive(false);

        sceneIndex++;

        scenes[sceneIndex].SetActive(true);

        player.transform.position = spawnPoints[sceneIndex].position;
        player.transform.rotation = spawnPoints[sceneIndex].rotation;

        transitionAnimator.SetTrigger("FadeIn");
    }
}
