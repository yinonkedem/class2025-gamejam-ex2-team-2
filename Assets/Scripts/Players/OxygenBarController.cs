using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBarController : MonoBehaviour
{
    [SerializeField] private Slider slider;


    public void updateBar(float currentOxygen, float maxOxygen)
    {
        slider.value = currentOxygen / maxOxygen;
    }
    
    public void PreventFlip()
    {
        // Ensure the oxygen bar's x-scale is always positive
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x); // Force x scale to be positive
        transform.localScale = localScale;
    }
    
    
    

}
