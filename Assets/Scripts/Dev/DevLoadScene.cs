using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevLoadScene : MonoBehaviour
{
    [SerializeField] int index = 1;
    public void LoadScene()
    {
        SceneManager.LoadScene(index);
    }
}
