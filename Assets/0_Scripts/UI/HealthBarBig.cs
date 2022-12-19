using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBig : MonoBehaviour
{
    [SerializeField] private List<Sprite> lifeSprites;
    
    private void OnEnable()
    {
        BigBot.BigBotHealthUpdate += ShowHealthSprite;
    }

    private void OnDisable()
    {
        BigBot.BigBotHealthUpdate -= ShowHealthSprite;
    }

    private void ShowHealthSprite(int spriteNr)
    {
        GetComponent<Image>().sprite = lifeSprites[spriteNr];
    }
}
