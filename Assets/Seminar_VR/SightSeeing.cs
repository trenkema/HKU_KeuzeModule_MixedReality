using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightSeeing : MonoBehaviour
{
    [SerializeField] SightSeeingManager sightSeeingManager;

    bool collided = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!collided)
        {
            if (other.tag == "Player")
            {
                collided = true;

                sightSeeingManager.NextSightSeeing();
            }
        }
    }
}
