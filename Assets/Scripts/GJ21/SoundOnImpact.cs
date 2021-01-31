using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnImpact : MonoBehaviour
{
    AudioSource Source => GetComponent<AudioSource>();
    private void OnCollisionEnter(Collision collision)
    {
        Source.Play();
    }
}
