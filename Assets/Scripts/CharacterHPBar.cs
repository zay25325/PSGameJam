/*
File : CharacterHPBar.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHPBar : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] CharacterInfo character;

    private void OnEnable()
    {
        character.OnHPChanged.AddListener(UpdateHPBar);
        UpdateHPBar();
    }

    private void OnDisable()
    {
        character.OnHPChanged.RemoveListener(UpdateHPBar);
    }

    private void UpdateHPBar()
    {
        hpBar.maxValue = character.MaxHP;
        hpBar.value = character.HP;
    }
}
