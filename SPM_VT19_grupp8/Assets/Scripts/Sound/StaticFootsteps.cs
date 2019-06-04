using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    private AudioSource source;
    public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlayStaticFootsteps()
    {
        source.PlayOneShot(clip);
    } 

}
