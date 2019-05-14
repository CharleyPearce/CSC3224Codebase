using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart
{

}

public class HealthBarContoller : MonoBehaviour
{
    public GameObject heart;

    public Sprite heartFull;
    public Sprite heartThreeQuarters;
    public Sprite heartHalf;
    public Sprite heartQuarter;
    public Sprite heartEmpty;

    List<GameObject> heartsList = new List<GameObject>();

    Player player;


    void Update()
    {
       
    }

    void Start()
    {

        player = GameObject.Find("Player").GetComponent<Player>();

        for (int i = 0; i < player.maxHealth; i++)
        {
            Vector3Int pos = new Vector3Int(i, 0, 10);
            heartsList.Add(Instantiate(heart, transform.position + pos, Quaternion.identity, gameObject.transform));
            // heartsList[i].transform.localScale = new Vector3(1, 1, 1);
        }

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        if (heartsList.Count != player.getMaxHealth())
        {
            heartsList.Clear();
            for (int i = 0; i < player.maxHealth; i++)
            {
                Vector3Int pos = new Vector3Int(i, 0, 10);
                heartsList.Add(Instantiate(heart, transform.position + pos, Quaternion.identity, gameObject.transform));
            }
        }

        for (int i = 0; i < player.health - 1; i++)
        {
            heartsList[i].GetComponent<SpriteRenderer>().sprite = heartFull;
        }

        int healthRemainder = (int)(100 * (player.health - (int)player.health));

        if (healthRemainder == 0)
        {
            heartsList[(int)player.health - 1].GetComponent<SpriteRenderer>().sprite = heartFull;
        }
        else if (healthRemainder < 25)
        {
            heartsList[(int)player.health].GetComponent<SpriteRenderer>().sprite = heartEmpty;
        }
        else if (healthRemainder < 50)
        {
            heartsList[(int)player.health].GetComponent<SpriteRenderer>().sprite = heartQuarter;
        }
        else if (healthRemainder < 75)
        {
            heartsList[(int)player.health].GetComponent<SpriteRenderer>().sprite = heartHalf;
        }
        else if (healthRemainder < 100)
        {
            heartsList[(int)player.health].GetComponent<SpriteRenderer>().sprite = heartThreeQuarters;
        }
        else
        {
            heartsList[(int)player.health].GetComponent<SpriteRenderer>().sprite = heartFull;
        }

        for (int i = (int)player.health; i < player.maxHealth; i++)
        {

            heartsList[i].GetComponent<SpriteRenderer>().sprite = heartEmpty;
        }
    }
}
