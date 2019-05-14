using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float waitTime;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        effector = GetComponent<PlatformEffector2D>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonUp("Crouch"))
        {
            waitTime = 0.3f;
            effector.rotationalOffset = 0;
        }

        if (Input.GetButton("Crouch"))
        {
            if (waitTime <= 0)
            {
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
                // effector.rotationalOffset = 180f;

                waitTime = 0.3f;
            } else
            {
                waitTime -= Time.deltaTime;
            }
        } else {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
        }
    }
}
