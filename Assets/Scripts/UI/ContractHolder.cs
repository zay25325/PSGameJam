using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static AttackShape;

public class ContractHolder : MonoBehaviour
{
    [SerializeField] List<int> validScenes;
    [SerializeField] List<ContractUI> contracts;

    enum RewardType
    {
        HP = 0,
        Speed = 1,
        Pay = 2,
        Attack = 3
    }

    private Dictionary<RewardType, int> RewardCosts = new Dictionary<RewardType, int>() 
    {
        { RewardType.HP, 1},
        { RewardType.Speed, 3},
        { RewardType.Pay, 1},
        { RewardType.Attack, 3},
    };

    private void Start()
    {
        // Add rewards
        if (SaveDataManager.Instance.SelectedContract != null)
        {
            SaveDataManager.Instance.ContractsCompleted++;
            SaveDataManager.Instance.AddContractRewards(SaveDataManager.Instance.SelectedContract);
        }

        foreach (ContractUI contract in contracts)
        {
            ContractData data = new ContractData();
            data.EnemyCount = Random.Range(2, 7) + (SaveDataManager.Instance.ContractsCompleted / 4);

            // Calculate rewards
            int rewardAmount = data.EnemyCount;
            while (rewardAmount > 0)
            {
                // calcuate which rewards we can still give the player
                List<RewardType> RewardKeys = new List<RewardType>();
                foreach (RewardType reward in RewardCosts.Keys)
                {
                    if (RewardCosts[reward] <= rewardAmount)
                    {
                        RewardKeys.Add(reward);
                    }
                }

                // limit the player to 5 attacks
                if (RewardKeys.Contains(RewardType.Attack) && SaveDataManager.Instance.Attacks.Count >= 5)
                {
                    RewardKeys.Remove(RewardType.Attack);
                }
                // limit the player to 8 spedd
                if (RewardKeys.Contains(RewardType.Speed) && SaveDataManager.Instance.Speed >= 8)
                {
                    RewardKeys.Remove(RewardType.Attack);
                }

                //
                RewardType rewardType = RewardKeys[Random.Range(0, RewardKeys.Count)];
                rewardAmount -= RewardCosts[rewardType];
                switch (rewardType)
                {
                    case RewardType.HP:
                        data.HPIncrease++;
                        break;
                    case RewardType.Speed:
                        data.SpeedIncrease++;
                        break;
                    case RewardType.Attack:
                        List<AttackKeys> UniqueAttacks = new List<AttackKeys>(AttackShape.AttackDictionary.Keys);
                        foreach (AttackKeys key in SaveDataManager.Instance.Attacks)
                        {
                            // UniqueAttacks will always contain every key in save data attacks
                            UniqueAttacks.Remove(key);
                        }
                        data.NewAttacks.Add(UniqueAttacks[Random.Range(0, UniqueAttacks.Count)]);
                        break;
                    case RewardType.Pay:
                        data.pay++;
                        break;
                }
            }

            contract.Data = data;
        }
    }

    public void SelectContract(int index)
    {
        SaveDataManager.Instance.SelectedContract = contracts[index].Data;
        SaveDataManager.Instance.EnemyCount = contracts[index].Data.EnemyCount;
        SceneManager.LoadScene(validScenes[Random.Range(0, validScenes.Count)]);
    }
}

public class ContractData
{
    public int EnemyCount;
    public int HPIncrease;
    public int SpeedIncrease;
    public List<AttackKeys> NewAttacks = new List<AttackKeys>();
    public int pay;
}