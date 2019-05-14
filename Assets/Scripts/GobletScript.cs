using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GobletScript : MonoBehaviour
{
    public GameObject star;
    float countdown = 3;
    bool collected = false;

    void Update()
    {
        if (collected)
        {
            if (countdown >= 0)
            {
                countdown -= Time.deltaTime;
            } else
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D trigger)
    {

        if (trigger.gameObject.name == "Player")
        {
            collected = true;
            Debug.Log("collected");
            Instantiate(star, gameObject.transform.position + new Vector3(0, 0, -1), Quaternion.identity);
            // Destroy(gameObject);

            // add more rooms to the dungeon for the next stage
            PlayerPrefs.SetInt("noRooms", PlayerPrefs.GetInt("noRooms") + 5);
            PlayerPrefs.SetInt("floorNo", PlayerPrefs.GetInt("floorNo") + 1);
        }
    }
}
