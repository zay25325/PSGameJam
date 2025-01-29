/*
File : CharacterInfo.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/26/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    [SerializeField] CharacterInfo character;
    public abstract void GetAction();
}
