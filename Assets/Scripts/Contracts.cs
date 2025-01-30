using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class Contract
{ // individual contract
    public string contractTitle;
    public string description;
    public int expPoints;
    public string difficulty;
}

[System.Serializable] // this can be reduced
public class ContractList
{// this is the list of contracts pulled from the JSON file
    public Contract[] contracts;
}


public class Contracts : MonoBehaviour
{
    [SerializeField] private TextAsset contractJSONFile;

    private VisualElement root;
    private VisualElement contractListContainer;
    private ContractList allContracts;  // Changed from Contract to ContractList

    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // visual elements pulled from the UI Toolkit file
        contractListContainer = root.Q<VisualElement>("contract-list");
        if (contractListContainer == null)
        {
            Debug.LogError("Could not find contract-container element!");
            return;
        }

        LoadContracts();
        DisplayRandomContracts();
    }

    //void Update()
    //{

    //}

    private void LoadContracts()
    { // load all contracts from the file
        allContracts = JsonUtility.FromJson<ContractList>(contractJSONFile.text);
        if (allContracts == null)
        {
            Debug.LogError("Could not load contracts from JSON file!");
            return;
        }
        Debug.Log("Loaded contracts from JSON");
    }

    private void DisplayRandomContracts()
    {
        int count = 1;
        for (int i = 0; i < allContracts.contracts.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allContracts.contracts.Length);
            Contract randomContract = allContracts.contracts[randomIndex];
            CreateContractCards(randomContract, count);
            count++;
        }
        Debug.Log("Displayed random contracts");
    }


    private void CreateContractCards(Contract contract, int count)
    {
        VisualElement contractCard = root.Q($"contract-card-{count}");
        if (contractCard == null)
        {
            Debug.LogError("Could not find contract-card template!");
            return;
        }

        Label contractTitle = contractCard.Q<Label>("contract-title");
        Label contractDescription = contractCard.Q<Label>("contract-description");
        Label contractExpPoints = contractCard.Q<Label>("contract-experience");
        Label contractDifficulty = contractCard.Q<Label>("contract-difficulty");

        if (contractTitle != null) 
            contractTitle.text = contract.contractTitle;

        if (contractDescription != null)
            contractDescription.text = contract.description;

        if (contractExpPoints != null)
            contractExpPoints.text = $"Experience: {contract.expPoints}";

        if (contractDifficulty != null)
            contractDifficulty.text = contract.difficulty;

        contractListContainer.Add(contractCard);
    }
}
