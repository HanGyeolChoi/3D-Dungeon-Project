﻿using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float startValue;
    public float maxValue;
    public float passiveValue;
    public Image ui;

    private void Start()
    {
        curValue = startValue ;
    }

    private void Update()
    {
        ui.fillAmount = curValue / maxValue;
    }

    public void Add(float value)
    {
        curValue = Mathf.Clamp(curValue + value, 0, maxValue);
    }
}