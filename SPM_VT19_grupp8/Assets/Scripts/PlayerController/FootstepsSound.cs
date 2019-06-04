using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSound : MonoBehaviour
{
    [SerializeField] private AudioClip stepSound;
    private AudioSource aus;

    private void Awake()
    {
        aus = GetComponentInParent<AudioSource>();
    }


    public void PlayStaticFootsteps()
    {
        aus.PlayOneShot(stepSound);
    }
}
