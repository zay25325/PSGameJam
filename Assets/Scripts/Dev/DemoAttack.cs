using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TileManager.Instance.AddShape(AttackShape.Cleave);
    }
}
