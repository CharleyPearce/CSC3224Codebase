using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool debugMode = false;
    public AudioMixer audioMix;

    public void DebugMode(bool debug)
    {
        debugMode = debug;
    }

    public void SetVolume(float volume)
    {
        audioMix.SetFloat("volume", volume);
        Debug.Log(volume);
    }
}
