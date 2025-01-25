using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackShape;

public class DemoAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TileManager.Instance.AddShape(AttackShape.AttackDictionary[AttackKeys.Cleave]);
    }
}
