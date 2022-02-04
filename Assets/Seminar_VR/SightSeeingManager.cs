using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightSeeingManager : MonoBehaviour
{
    [SerializeField] GameObject[] sightSeeingArrows;

    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip successSound;

    int index = 0;

    private void Start()
    {
        foreach (var arrow in sightSeeingArrows)
        {
            arrow.SetActive(false);
        }

        sightSeeingArrows[0].SetActive(true);
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
        Debug.Log("Next Transition");
    }
}
