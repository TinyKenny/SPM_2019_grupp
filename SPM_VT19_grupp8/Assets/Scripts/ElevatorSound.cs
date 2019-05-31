using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSound : MonoBehaviour
{

    private AudioSource source;
    public float delay;

    // public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.PlayDelayed(delay);
        // source.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
