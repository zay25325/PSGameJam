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
    public int ContractsCompleted = 0;
    public int Money = 0;

    public ContractData SelectedContract = null;

    public bool HasReadIntro = false;

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

    public void AddContractRewards(ContractData contract)
    {
        HP += contract.HPIncrease;
        Speed += contract.SpeedIncrease;
        foreach (AttackKeys key in contract.NewAttacks)
        {
            Attacks.Add(key);
        }
        Money += contract.pay;
    }
}
