using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract
{ // individual contract
    public string title;
    public string description;
    public string experience;
    public string difficulty;
}

[System.Serializable]
public class ContractList
{// this is the list of contracts pulled from the JSON file
    public Contract[] contracts;
}


public class Contracts : MonoBehaviour
{



    private Contract allContracts;
    private List<Contract> contractList = new();


    // Start is called before the first frame update
    void Start()
    {
        LoadContracts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // load and finalize the contracts before displaying
    private void LoadContracts()
    {

    }

    // display the contracts after loading
    private void SetupContractUI()
    {

    }
}
