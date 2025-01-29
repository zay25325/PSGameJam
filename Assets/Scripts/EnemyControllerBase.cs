using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
File : EnemyController.cs
Project : PROG3126 - Hackathon
Programmer: Vincent Marshall
First Version: 1/28/2025
*/

public class EnemyControllerBase:MonoBehaviour {
    [SerializeField] TileManager board = TileManager.Instance;

    [SerializeField] CharacterInfo info;

    [System.NonSerialized] private Vector2Int[] ValidMoves = { };

    // This is the enemy's intended target, either for a move or an attack
    public Vector2Int TargetTile;

    // public IntentType = Move/Attack;
    // public Intended Attack = AttackShape;

    // Start is called before the first frame update
    void Start() {
        this.info = this.gameObject.GetComponent<CharacterInfo>();
        this.Think();
    }

    // Update is called once per frame
    void Update() {

    }

    // Implement this for each enemy type
    public void Think() {
        this.ValidMoves = GetValidMoves(this.info.Speed,false);
    }

    // Thie function is in charge of getting all possible movement actions that we can take
    // if a tile is targeted by a move action, or occupied by a character or obstacle, the position is considered invalid.
    // if avoiddamage is checked, then it will also deem tiles targeted by an attack as invalid
    public Vector2Int[] GetValidMoves(int moveRange,bool avoiddamage) {

        List<Vector2Int> output = new();

        foreach(Vector2Int position in GetAllMoves(this.TilePosition(),moveRange)) {
            if(TileManager.Instance.IsTileOccupied(position)) continue; //skip tiles with characters on them.

            //this might be expensive, but we need to make sure enemy moves don't overlap
            foreach(var move in TileManager.Instance.Moves) {
                if(move.MoveTo == position) continue;
            }

            //check if there is an open path to their desired position
            if(GetPathFromTo(this.TilePosition(),position,moveRange) == false) continue;

            if(avoiddamage) {
                // TODO: check attack ranges.
            }

            output.Add(position);
        }

        return output.ToArray();
    }

    public Vector2Int[] GetAllMoves(Vector2Int start,int moveRange) {

        List<Vector2Int> output = new List<Vector2Int> { };

        // x goes from min to max
        for(int x = -moveRange;x <= moveRange;x++) {
            // y goes from min to max, while x+y is never greater than the max move range, which will cover all squares
            for(int y = -moveRange+Mathf.Abs(x);Mathf.Abs(y)+Mathf.Abs(x) <= moveRange;y++) {
                // ignore the very center tile
                if(!(x == 0 && y == 0)) output.Add(new Vector2Int(x,y) + start);
                //Debug.Log($"Registering {x},{y} as a valid movement tile");
            }
        }

        return output.ToArray();
    }

    private void OnDrawGizmos() {
        
        //only draw these in playmode because otherwise we DROWN in errors lol
        if( Application.isPlaying) {
            Gizmos.color = new Color(0f,0f,1f,0.5f);
            if(this.ValidMoves != null) {
                foreach(var position in GetValidMoves(this.info.Speed, false)) {
                    Gizmos.DrawCube(TileManager.TileToPosition(position), Vector3.one*0.5f);
                }
            }
            Gizmos.color = new Color(1f,0f,0f,0.5f);
            Gizmos.DrawCube(
                TileManager.TileToPosition(GetMoveClosest(
                        TilePosition(),
                        TileManager.PositionToTile(TileManager.Instance.GetPlayerCharacter().transform.position),
                        this.info.Speed)),
                Vector3.one
            );
        }
    }

    //returns my position as a vec2
    private Vector2Int TilePosition() {
        return TileManager.PositionToTile(this.transform.position);
    }

    // Walks towards a destination, returning a list of moves to reach the path
    // if the path is blocked, return false
    // brute forces and returns once ANY path is found
    public static bool GetPathFromTo(Vector2Int start,Vector2Int target,int moves) {

        // termination conditions

        if(start == target) {
            return true;
        }
        if(moves <= 0) {
            return false;
        }

        // try to move towards the target at first
        Vector2Int move;

        int movex = Mathf.Clamp(start.x-target.x,-1,1);
        int movey = Mathf.Clamp(start.y-target.y,-1,1);

        // pick one direction to go
        if(movex != 0) {
            move = new Vector2Int(movex,0);
        } else {
            move = new Vector2Int(0,movey);
        }

        for(int i = 0;i < 4;i++) {
            // try each direction

            // if the tile is not occupied, try to find a path from that one (recursive search)
            if(!TileManager.Instance.IsTileOccupiedIgnoreCharacters(start+move)) {
                //if we can make a successful move, go ahead
                if(GetPathFromTo(start+move,target,moves-1)) {
                    return true; //found a path, will unwind the stack and return out a true
                }
            }
            // if that tile is blocked, try another direction
            // rotates move 90 degrees counterclockwise using the Perpendicular function
            // there is no perp function for Vector2Int, cry about it
            Vector2 a = Vector2.Perpendicular(new(move.x,move.y));
            move = new Vector2Int((int)a.x,(int)a.y);
        }

        return false;
    }

    // stupid linear distance check, may not be very good
    // gets the closest available move to the target
    public Vector2Int GetMoveClosest(Vector2Int start, Vector2Int target, int moves) {
        Vector2Int closest = start;

        foreach(Vector2Int position in GetValidMoves(moves, false)) {
            if(Vector2.SqrMagnitude(position - target) < Vector2.SqrMagnitude(closest - target)) {
                closest = position;
            }
        }

        return closest;
    }
}
