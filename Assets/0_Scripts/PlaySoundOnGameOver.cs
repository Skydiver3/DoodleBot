using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnGameOver : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    
    private void OnEnable()
    {
        BigBot.EndLevel += OnEndLevel;
    }
    private void OnDisable()
    {
        BigBot.EndLevel -= OnEndLevel;
    }

    private void OnEndLevel()
    {
        backgroundMusic.Stop();
        GetComponent<AudioSource>().Play();
        this.enabled = false;
    }
}
