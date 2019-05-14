using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAttack : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (player.GetUpAttack())
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {

                col.gameObject.GetComponent<Character>().Damage(1);
                col.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 1) * 1000);
            }
        }
    }
}
