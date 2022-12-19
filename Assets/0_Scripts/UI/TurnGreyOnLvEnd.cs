using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TurnGreyOnLvEnd : MonoBehaviour
{
 ColorGrading     colorGradingLayer     = null;
 [SerializeField] private float fadeSpeed;
    private void OnEnable()
    {
        
        PostProcessVolume volume = gameObject.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out colorGradingLayer);
        BigBot.EndLevel += OnEndLevel;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        BigBot.EndLevel -= OnEndLevel;
    }

    private void OnEndLevel()
    {
        StartCoroutine(FadeToGrey());
        Debug.Log("end");
    }

    private IEnumerator FadeToGrey()
    {
        while (colorGradingLayer.saturation.value > -100)
        {
            colorGradingLayer.saturation.value -= fadeSpeed;
            yield return new WaitForFixedUpdate();
        }
    }
}
