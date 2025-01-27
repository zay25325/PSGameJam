using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static AttackShapeBuilder;

public class DemoPlayer : MonoBehaviour
{
    [SerializeField] private GameObject moveTileGO;
    List<GameObject> moveTiles = new List<GameObject>(); // for large numbers of the same object it is better to enable/disable then to create/delete them
    [SerializeField] CharacterInfo character;
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
            AttackShape attack = AttackShape.AttackDictionary[AttackShape.AttackKeys.Cleave];
            attack.ChainAttack = AttackShape.AttackDictionary[AttackShape.AttackKeys.XLightning];
            TileManager.Instance.AddPlayerAttack(attack, direction, transform.position);
            TileManager.Instance.EndTurn();
        }
    }

    public void PlaceMoveTiles() // public for the editor
    {
        // create the needed number of move tiles.
        // the player's move speed is not likely to change mid battle
        int requiredMoveTiles = character.Speed * ((1 + character.Speed) / 2) * 4; // n * ((1 + n) / 2) = 1+2+...+n, x4 for the 4 directions

        if (moveTiles.Count < requiredMoveTiles)
        {
            for (int i = moveTiles.Count; i < requiredMoveTiles; i++)
            {
                moveTiles.Add(CreateMoveTile());
            }

            // order the tiles going clockwise
            /* speed 2 example, loop left to right, bottom to top, skipping the center
            _ _ X _ _
            _ X X X _
            X X O X X
            _ X X X _
            _ _ X _ _
            */
            int tileIndex = 0;
            for (int x = -character.Speed; x <= character.Speed; x++)
            {
                int maxY = character.Speed - Mathf.Abs(x);
                for (int y = maxY; y > 0; y--)
                {
                    moveTiles[tileIndex].transform.localPosition = new Vector3(x, y); // note these are local positions, not global.
                    moveTiles[tileIndex + 1].transform.localPosition = new Vector3(x, -y);
                    tileIndex += 2;
                }

                if (x != 0) // do not have a move tile in the very center
                {
                    moveTiles[tileIndex].transform.localPosition = new Vector3(x, 0);
                    tileIndex++;
                }
            }
        }

        for (int i = 0; i < moveTiles.Count; i++)
        {
            if (i < requiredMoveTiles)
            {
                if (TileManager.Instance.IsTileOccupied(TileManager.PositionToTile(moveTiles[i].transform.position))) // global pos, not local
                {
                    moveTiles[i].SetActive(false);
                }
                else
                {
                    moveTiles[i].SetActive(true);
                }
            }
            else
            {
                moveTiles[i].SetActive(false);
            }
        }
    }

    private GameObject CreateMoveTile()
    {
        GameObject moveTile = GameObject.Instantiate(moveTileGO, transform);
        moveTile.GetComponent<PlayerMoveTile>().character = character;
        return moveTile;
    }

}


#if UNITY_EDITOR
[CustomEditor(typeof(DemoPlayer))]
public class DemoPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Create Move Tiles"))
        {
            var creator = (DemoPlayer)target;
            creator.PlaceMoveTiles();
        }
    }

}
#endif