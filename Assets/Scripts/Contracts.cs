using System.Collections;
using System.Collections.Generic;
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

[System.Serializable]
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
        for (int i = 0; i < allContracts.contracts.Length; i++)
        {
            int randomIndex = Random.Range(0, allContracts.contracts.Length);
            Contract randomContract = allContracts.contracts[randomIndex];
            CreateContractCard(randomContract);
        }
        Debug.Log("Displayed random contracts");
        //Contract randomContract = allContracts.contracts[randomIndex];
    }

    private void CreateContractCard(Contract contract)
    {
        VisualElement contractCard = root.Q("contract-card");
        if (contractCard == null)
        {
            Debug.LogError("Could not find contract-card template!");
            return;
        }

        Label contractTitle = contractCard.Q<Label>("contract-title");
        contractTitle.text = contract.contractTitle;

        Label contractDescription = contractCard.Q<Label>("contract-description");
        contractDescription.text = contract.description;

        Label contractExpPoints = contractCard.Q<Label>("contract-experience");
        contractExpPoints.text = $"Experience: {contract.expPoints}";

        Label contractDifficulty = contractCard.Q<Label>("contract-difficulty");
        contractDifficulty.text = contract.difficulty;

        //this works :p
        //VisualElement contractCard = new();
        //contractCard.AddToClassList("contract-card");

        //Label contractTitle = new(contract.contractTitle);
        //contractTitle.AddToClassList("contract-title");

        //Label contractDescription = new(contract.description);
        //contractDescription.AddToClassList("contract-description");

        //Label contractExpPoints = new($"Experience: {contract.expPoints}");
        //contractExpPoints.AddToClassList("contract-experience");

        //Label contractDifficulty = new(contract.difficulty);
        //contractDifficulty.AddToClassList("contract-difficulty");

        //// add into the card itself
        //contractCard.Add(contractTitle);
        //contractCard.Add(contractDescription);
        //contractCard.Add(contractExpPoints);
        //contractCard.Add(contractDifficulty);

        //// add the card into the container for display
        //contractListContainer.Add(contractCard);
    }
}
