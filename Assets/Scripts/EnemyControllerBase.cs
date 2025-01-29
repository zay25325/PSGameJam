using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
File : EnemyController.cs
Project : PROG3126 - Hackathon
Programmer: Vincent Marshall
First Version: 1/28/2025
*/

public class EnemyControllerBase : MonoBehaviour
{
    [SerializeField] TileManager board = TileManager.Instance;

    [SerializeField] CharacterInfo info;

    [System.NonSerialized] private Vector2Int[] ValidMoves = {};

    // Start is called before the first frame update
    void Start()
    {
        this.info = this. gameObject.GetComponent<CharacterInfo>();
        this.Think();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Implement this for each enemy type
    public void Think() {
        this.ValidMoves = GetAllMoves(TileManager.PositionToTile(this.transform.position), this.info.Speed);
    }

    // Thie function is in charge of getting all possible movement actions that we can take
    // if a tile is targeted by a move action, or occupied by a character or obstacle, the position is considered invalid.
    // if avoiddamage is checked, then it will also deem tiles targeted by an attack as invalid
    public Vector2Int[] GetValidMoves(int moveRange, bool avoiddamage) {
        
        List<Vector2Int> output = new ();

        foreach(Vector2Int position in GetAllMoves(this.TilePosition(), this.info.Speed)) {
            if(TileManager.Instance.IsTileOccupied(position)) continue; //skip tiles with characters on them.
            
            //this might be expensive, but we need to make sure enemy moves don't overlap
            foreach(var move in TileManager.Instance.Moves) {
                if(move.MoveTo == position) continue;
            }
        }

        return output.ToArray();
    }

    public Vector2Int[] GetAllMoves(Vector2Int start, int moveRange) {

        List<Vector2Int> output = new List<Vector2Int>{};

        // x goes from min to max
        for(int x = -moveRange; x <= moveRange; x++) {
            // y goes from min to max, while x+y is never greater than the max move range, which will cover all squares
            for(int y = -moveRange+Mathf.Abs(x); Mathf.Abs(y)+Mathf.Abs(x) <= moveRange; y++) {
                // ignore the very center tile
                if(!(x == 0 && y == 0)) output.Add(new Vector2Int(x,y) + start);
                Debug.Log($"Registering {x},{y} as a valid movement tile");
            }
        }

        return output.ToArray();
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(0f,0f,1f,0.5f);
        if(this.ValidMoves != null) {
            foreach(var position in ValidMoves) {
                Gizmos.DrawCube(TileManager.TileToPosition(position), new Vector3(0.5f,0.5f,0));
            }
        }
    }

    //returns my position as a vec2
    private Vector2Int TilePosition() {
        return TileManager.PositionToTile(this.transform.position);
    }
}
