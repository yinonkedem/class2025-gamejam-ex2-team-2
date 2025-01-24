using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    [SerializeField] private Slider slider;


    public void updateBar(float currentOxygen, float maxOxygen)
    {
        slider.value = currentOxygen / maxOxygen;
    }
    
    public void updateBarColor(Color color)
    {
        slider.fillRect.GetComponent<Image>().color = color;
    }
    
    
    

}
