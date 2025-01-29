using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using static AttackShapeBuilder;
using static AttackShape;

public class DemoPlayer : MonoBehaviour
{
    [SerializeField] private GameObject moveTileGO;
    List<GameObject> moveTiles = new List<GameObject>(); // for large numbers of the same object it is better to enable/disable then to create/delete them
    [SerializeField] CharacterInfo character;

    [SerializeField] GameObject combatButtonPrefab;
    [SerializeField] Transform permHolder;
    [SerializeField] Transform tempHolder;

    [SerializeField] Toggle moveToggle;
    List<CombatButton> permCombatButtons = new List<CombatButton>();
    List<CombatButton> tempCombatButtons = new List<CombatButton>();
    List<AttackShape.AttackKeys> tempAttacks = new List<AttackShape.AttackKeys>();
    List<CombatButton> activeAttacks = new List<CombatButton>();




    // Start is called before the first frame update
    void Start()
    {
        foreach(AttackKeys attack in character.Attacks)
        {
            GameObject combatButtonGO = GameObject.Instantiate(combatButtonPrefab, permHolder);
            CombatButton combatButton = combatButtonGO.GetComponent<CombatButton>();
            combatButton.Initialize(attack, false, this);
            permCombatButtons.Add(combatButton);
        }
    }

    public void ClickedWorldSpace()
    {
        if (activeAttacks.Count > 0)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 relativePos = mousePos - character.transform.position;

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
            List<AttackShape> ChainAttacks = new List<AttackShape>();
            for (int i = 0; i < activeAttacks.Count; i++)
            {
                ChainAttacks.Add(new AttackShape(AttackShape.AttackDictionary[activeAttacks[i].attack]));
                ChainAttacks[i].Caster = character;
                if (i != 0)
                {
                    ChainAttacks[i - 1].ChainAttack = ChainAttacks[i];
                }
            }

            Vector3 position;
            if (ChainAttacks[0].TargetType == Target.Ranged)
            {
                position = mousePos;
            }
            else
            {
                position = character.transform.position;
            }

            TileManager.Instance.AddPlayerAttack(ChainAttacks[0], direction, position);
            TileManager.Instance.EndTurn();
        }
    }

    public void MoveTileSelected(Vector3 moveTo)
    {
        MoveAction move = new MoveAction(character.transform.position, moveTo, character);

        TileManager.Instance.AddPlayerMove(move);
        TileManager.Instance.EndTurn();
        PlaceMoveTiles();
    }

    public void ClickedCombatButton(CombatButton button, bool isOn)
    {
        if (isOn)
        {
            activeAttacks.Add(button);
        }
        else
        {
            if (activeAttacks.Contains(button))
            {
                activeAttacks.Remove(button);
            }
        }

        ClearMoveTiles();
    }

    private void ClearCombatButtons()
    {
        foreach (CombatButton button in activeAttacks)
        {
            button.Deactivate();
        }
        activeAttacks.Clear();
    }

    public void ClickedMoveButton(bool isOn)
    {
        if (isOn)
        {
            ClearCombatButtons();
            PlaceMoveTiles();
        }
        else
        {
            ClearMoveTiles();
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

    private void ClearMoveTiles()
    {
        foreach (GameObject tile in moveTiles)
        {
            tile.SetActive(false);
        }
        moveToggle.isOn = false;
    }

    private GameObject CreateMoveTile()
    {
        GameObject moveTileObj = GameObject.Instantiate(moveTileGO, character.transform);
        PlayerMoveTile moveTile = moveTileObj.GetComponent<PlayerMoveTile>();
        moveTile.playerUI = this;
        return moveTileObj;
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