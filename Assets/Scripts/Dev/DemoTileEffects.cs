/*
File : DemoTileEffects.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileEffectLibrary;

public class DemoTileEffects : MonoBehaviour
{
    List<TileEffectKey> effectList;
    int effect = 0;
    float timer = 0;

    private void Start()
    {
        effectList = new List<TileEffectKey>(TileEffectLibrary.Instance.TileAnimations.Keys);
        if (effectList.Count > 0)
        {
            GameObject.Instantiate(TileEffectLibrary.Instance.TileAnimations[effectList[0]], transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            timer = 0f;
            effect++;
            if (effect < effectList.Count)
            {
                GameObject.Instantiate(TileEffectLibrary.Instance.TileAnimations[effectList[effect]], transform);
            }
        }
    }
}
