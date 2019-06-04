using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFootsteps : MonoBehaviour
{
    private AudioSource aus2;
    public AudioClip stepSound;

    // Start is called before the first frame update
    void Start()
    {
        aus2 = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlayStaticFootsteps()
    {
        aus2.PlayOneShot(stepSound);
    } 

}
