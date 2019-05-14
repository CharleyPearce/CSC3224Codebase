using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateScript : MonoBehaviour
{
    TileGenerator generator;

    void Start()
    {
        if (!OptionsScript.debugMode)
        {
            gameObject.SetActive(false);
        }

        generator = GameObject.Find("Generator").GetComponent<TileGenerator>();
    }

    void Update()
    {
        gameObject.GetComponent<UnityEngine.UI.Text>().text = "FPS: " + (int)(1/Time.deltaTime) + "\nLoadtime: " + generator.getLoadTime() + "ms";
        gameObject.GetComponent<UnityEngine.UI.Text>().transform.position = new Vector2(100, Screen.height - 300);
    }
}
