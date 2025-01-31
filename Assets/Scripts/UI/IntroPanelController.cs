using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPanelController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    private void Start()
    {
        if (SaveDataManager.Instance.HasReadIntro == false)
        {
            panel.SetActive(true);
            SaveDataManager.Instance.HasReadIntro = true;
        }
    }
}
