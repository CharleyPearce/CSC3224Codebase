using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableHeart : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D trigger)
    {

        if (trigger.gameObject.name == "Player")
        {
            if (!trigger.gameObject.GetComponent<Player>().isFullHealth())
            {
                trigger.gameObject.GetComponent<Player>().heal(1f);
                Destroy(gameObject);
            }
        }
    }

}
