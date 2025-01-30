/*
File : TileEffectLibrary.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/30/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackShape;

public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;

    public int HP = 10;
    public int Speed = 2;

    public List<AttackKeys> Attacks;

    public int EnemyCount = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
