/*
File : EffectClearer.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectClearer : MonoBehaviour
{
    public void ClearEffect()
    {
        GameObject.Destroy(gameObject);
    }
}
