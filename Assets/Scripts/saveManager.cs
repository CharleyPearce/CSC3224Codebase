using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("floorNo", 0);
        PlayerPrefs.SetInt("noRooms", 5);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
