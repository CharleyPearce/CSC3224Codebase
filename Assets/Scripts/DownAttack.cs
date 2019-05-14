using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttack : MonoBehaviour
{
    Player player;
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (player.GetDownAttack())
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {

                col.gameObject.GetComponent<Character>().Damage(1);
                gameObject.GetComponentInParent<Player>().applyForce(new Vector2(0, 1) * 1000);
            }
        }
    }
}
