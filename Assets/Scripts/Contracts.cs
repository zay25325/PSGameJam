using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class Contract
{ // individual contract
    public string title;
    public string description;
    public int experience;
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
    [SerializeField] private UIDocument uiDocument;

    private VisualElement root;
    private VisualElement contractListContainer;
    private ContractList allContracts;  // Changed from Contract to ContractList

    void OnEnable()
    {
        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.Log("UI document root is null");
            return; 
        }

        contractListContainer = root.Q<VisualElement>("contract-list");
        LoadContracts();
        DisplayRandomContracts();
    }


    //void Update()
    //{

    //}

    private void LoadContracts()
    { // load all contracts from the file
        allContracts = JsonUtility.FromJson<ContractList>(contractJSONFile.text);
    }

    private void DisplayRandomContracts()
    {
        int randomIndex = Random.Range(0, allContracts.contracts.Length);
        Contract randomContract = allContracts.contracts[randomIndex];
        Debug.Log(randomContract.title);
    }


}
