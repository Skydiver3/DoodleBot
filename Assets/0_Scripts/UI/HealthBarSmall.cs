using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarSmall : MonoBehaviour
{
    [SerializeField] private List<Sprite> lifeSprites;
    
    private void OnEnable()
    {
        SmallBot.SmallBotHealthUpdate += ShowHealthSprite;
    }

    private void OnDisable()
    {
        SmallBot.SmallBotHealthUpdate -= ShowHealthSprite;
    }

    private void ShowHealthSprite(int spriteNr)
    {
        GetComponent<Image>().sprite = lifeSprites[spriteNr];
    }
}
