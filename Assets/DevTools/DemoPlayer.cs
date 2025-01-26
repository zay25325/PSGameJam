using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackShapeBuilder;

public class DemoPlayer : MonoBehaviour
{
    int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position += new Vector3(0, 1);
            TileManager.Instance.EndTurn();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += new Vector3(0, -1);
            TileManager.Instance.EndTurn();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(1, 0);
            TileManager.Instance.EndTurn();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0);
            TileManager.Instance.EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            index = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            index = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            index = 2;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 relativePos = mousePos - transform.position;

            Direction direction;
            if (Mathf.Abs(relativePos.x) > Mathf.Abs(relativePos.y))
            {
                if (relativePos.x >= 0f)
                    direction = Direction.Right;
                else
                    direction = Direction.Left;
            }
            else
            {
                if (relativePos.y >= 0f)
                    direction = Direction.Up;
                else
                    direction = Direction.Down;
            }

            //Debug.Log(direction);
            AttackShape attack = AttackShapeBuilder.AttackAt(AttackShape.AttackDictionary[AttackShape.AttackKeys.Cleave],
                direction, new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y)));
            attack.ChainAttack = AttackShape.AttackDictionary[AttackShape.AttackKeys.XLightning];

            TileManager.Instance.AddPlayerAttack(attack);
            TileManager.Instance.EndTurn();
        }
    }
}
