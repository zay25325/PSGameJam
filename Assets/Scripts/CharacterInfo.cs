/*
File : CharacterInfo.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] int maxHP = 10;
    [SerializeField] int hp = 10;
    [SerializeField] int speed = 3;
    [SerializeField] List<AttackShape.AttackKeys> attacks = new List<AttackShape.AttackKeys>();

    // player has null, since they don't have an attached enemy ai
    [SerializeField] EnemyControllerBase ai = null;


    [HideInInspector] public UnityEvent OnHPChanged = new UnityEvent();

    public int MaxHP
    {
        get => maxHP;
        set
        {
            maxHP = value;
            OnHPChanged.Invoke();
        }
    } 

    public int HP
    {
        get => hp;
        set
        {
            hp = value;
            OnHPChanged.Invoke();
        }
    }

    public int Speed { get => speed; set => speed = value; }
    public List<AttackShape.AttackKeys> Attacks { get => attacks; set => attacks = value; }
    //public EnemyAI AI { get => ai; set => ai = value; }
}
