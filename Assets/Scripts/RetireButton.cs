using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetireButton : MonoBehaviour
{
    [SerializeField] int costOfRetirement = 10;
    [SerializeField] Button retireButton;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveDataManager.Instance.Money < costOfRetirement)
        {
            retireButton.interactable = false;
        }
    }
}
