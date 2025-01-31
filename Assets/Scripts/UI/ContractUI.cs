using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContractUI : MonoBehaviour
{
    [SerializeField] private TMP_Text targetsText;
    [SerializeField] private Transform rewardLayoutGroup;
    [SerializeField] GameObject ContractRewardPrefab;

    [SerializeField] Sprite HPSprite;
    [SerializeField] Sprite SpeedSprite;
    [SerializeField] Sprite paySprite;

    private ContractData data;
    public ContractData Data
    {
        get => data;
        set => InitializeData(value);
    }

    private void InitializeData(ContractData data)
    {
        this.data = data;

        targetsText.text = $"Targets: {data.EnemyCount}";

        if (data.HPIncrease > 0)
        {
            CreateRewardUI(HPSprite, $"+{data.HPIncrease} HP");
        }
        if (data.SpeedIncrease > 0)
        {
            CreateRewardUI(SpeedSprite, $"+{data.SpeedIncrease} Speed");
        }
        if (data.pay > 0)
        {
            CreateRewardUI(paySprite, $"+{data.pay} money");
        }

        foreach(AttackShape.AttackKeys key in data.NewAttacks)
        {
            CreateRewardUI(TileEffectLibrary.Instance.TileUIs[AttackShape.AttackDictionary[key].TileAnimation], $"+{key} gene");
        }

    }

    private void CreateRewardUI(Sprite sprite, string text)
    {
        GameObject rewardObj = GameObject.Instantiate(ContractRewardPrefab, rewardLayoutGroup);
        ContractRewardUI rewardUI = rewardObj.GetComponent<ContractRewardUI>();
        rewardUI.icon.sprite = sprite;
        rewardUI.tmpText.text = text;
    }
}
