using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}


    private void OnEnable()
    {
        // ui tool kit
        var root = GetComponent<UIDocument>().rootVisualElement;

        var playBtn = root.Q<Button>("PlayBtn");

        playBtn.clicked += () =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        };
    }
}
