using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveTile : MonoBehaviour
{
    public DemoPlayer playerUI;

    private void OnMouseDown()
    {
        playerUI.MoveTileSelected(transform.position);
    }
}
