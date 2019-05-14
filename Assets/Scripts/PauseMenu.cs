using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;

    public GameObject menu;
    public GameObject winMenu;
    public GameObject pauseMenu;
    public GameObject deadMenu;
    Player player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void Pause()
    {
        menu.SetActive(true);
        Time.timeScale = 0;
        paused = true;
        
    }
     
    public void Resume()
    {
        menu.SetActive(false);
        Time.timeScale = 1;
        paused = false;
    }

    public void MainMenu()
    {
        Resume();
        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if (!player.Alive())
        {
            menu.SetActive(true);
            deadMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }

        if (player.Won())
        {
            menu.SetActive(true);
            winMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Resume();
            } else
            {
                Pause();
            }

        }
    }
}
