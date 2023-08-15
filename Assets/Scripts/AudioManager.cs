    using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _audioSource.PlayOneShot(_clip);
        }
    }
}