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
                
                //we could avoid moving to a square we are already on, but that creates other problems.
                //if(!(x == 0 && y == 0)) output.Add(new Vector2Int(x,y) + start);
                output.Add(new Vector2Int(x,y) + start);
                //Debug.Log($"Registering {x},{y} as a valid movement tile");
            }
        }

        return output.ToArray();
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
        Vector2Int move = GetOrthoDir(start, target);

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
    // for enemy advancing behaviour
    public Vector2Int GetMoveClosest(Vector2Int target, Vector2Int[] moves) {

        Vector2Int closest = new Vector2Int(99999,99999); //some absurdly far away value

        foreach(var move in moves) {
            if(Vector2.SqrMagnitude(move - target) < Vector2.SqrMagnitude(closest - target)) {
                closest = move;
            }
        }

        return closest;
    }

    // gets the position farthest from the target from a list of moves
    //
    public Vector2Int GetMoveFarthest(Vector2Int target, Vector2Int[] moves) {
        Vector2Int farthest = target;

        foreach(var move in moves) {
            if(Vector2.SqrMagnitude(move - target) > Vector2.SqrMagnitude(farthest - target)) {
                farthest = move;
            }
        }

        return farthest;
    }
    
    // from a list of positions, filters out positions that have a line of sight to a target
    //
    public Vector2Int[] GetMovesWithLOSTo(Vector2Int target, Vector2Int[] moves, int range) {

         List<Vector2Int> output = new List<Vector2Int> { };

        foreach(Vector2Int position in moves) {
            if(LineOfSight(position, target, range)) output.Add(position); 
        }

        return output.ToArray();
    }

    // check if theres a wall in the way
    // 
    public bool LineOfSight(Vector2Int start, Vector2Int target, int range) {
        Vector3 beam = (TileManager.TileToPosition(target) - TileManager.TileToPosition(start));
        Vector3 beamDir = beam.normalized;

        // Basically
        // take the direction from source to target
        // for each step, check if there's a wall. if there is, los is blocked
        // else, check for the target. if the tile position is the same, then that is a hit

        if(range == -1) range = 100;

        for(float i = 0; i < range; i+= 0.2f) {
            
            Vector2Int currentTile = TileManager.PositionToTile(TileManager.TileToPosition(start) + beamDir*i);

            if(TileManager.Instance.IsTileOccupiedIgnoreCharacters(currentTile)) {
                //blocked
                return false;
            } else if (currentTile == target) {
                // the ray has reached its target
                return true;
            }
        }

        // the target is out of range
        return false;
    }

    //
    // gets a unit vector in direction of target
    // clamped to one of 4 cardinal directions. prefers x over y 
    // used for getting the "best" starting direction
    //
    public static Vector2Int GetOrthoDir(Vector2Int start, Vector2Int target) {
        Vector2Int move;
        int movex = Mathf.Clamp(target.x-start.x,-1,1);
        int movey = Mathf.Clamp(target.y-start.y,-1,1);
        if(Mathf.Abs(movex) >= Mathf.Abs(movey)) {
            move = new Vector2Int(movex,0);
        } else {
            move = new Vector2Int(0,movey);
        }
        return move;
    }


    //
    // this function takes an attack and sees if it will connects with a target cell
    //
    public bool CheckAttack(Vector2Int start, Vector2Int target, AttackShape attack) {
        switch(attack.TargetType) {

            //simple line of sight check
            case AttackShape.Target.Ranged:
                return(LineOfSight(start, target, 100)); //placeholder range of basically infinite

            // check one direction
            case AttackShape.Target.Self:
                foreach(var tile in AttackShapeBuilder.AttackAt(attack, AttackShapeBuilder.Direction.Right, start).AttackTiles) {
                    if(tile.Position == target) return true;
                }
                return false;

            // check all 4 directions, starting with the "best" one
            case AttackShape.Target.Touch:
                 Vector2Int move = GetOrthoDir(start, target);

                // try each direction, check each target cell
                for(int i = 0; i<4;i++) {
                    foreach(var tile in AttackShapeBuilder.AttackAt(attack, AttackShapeBuilder.VecToDir[move], start).AttackTiles) {
                        if(tile.Position == target) return true;
                    }
                    //rotate counterclockwise until we get a hit
                    Vector2 a = Vector2.Perpendicular(new(move.x,move.y));
                    move = new Vector2Int((int)a.x,(int)a.y);
                }
                return false;

            default: return false;
        }
    }


    //
    // ======== DEBUG VISUALS ========
    //
    private void OnDrawGizmosSelected() {
        
        var Green = new Color(0f,1f,0f,0.5f);; 
        var Blue = new Color(0f,0f,1f,0.5f);
        var Red = new Color(1f,0f,0f,0.5f);
        var Cyan =  new Color(1f,1f,0f,0.5f);
        var Yellow = new Color(1f,1f,0f,0.5f);


        //only draw these in playmode because otherwise we DROWN in nullreference errors lol
        if( Application.isPlaying) {

            var playerpos = TileManager.Instance.GetPlayerCharacter().transform.position;
            // show all moves
            Gizmos.color = Blue;
            if(this.ValidMoves != null) {
                foreach(var position in GetValidMoves(this.info.Speed, false)) {
                    Gizmos.DrawCube(TileManager.TileToPosition(position), Vector3.one*0.5f);
                }
            }

            // show move closer
            Gizmos.color = Red;
            Gizmos.DrawCube(
                TileManager.TileToPosition(GetMoveClosest(
                        TileManager.PositionToTile(playerpos),
                        GetValidMoves(this.info.Speed, false))),
                Vector3.one
            );

            //show move farther
            Gizmos.color = Yellow;
            Gizmos.DrawCube(
                TileManager.TileToPosition(GetMoveFarthest(
                        TileManager.PositionToTile(playerpos),
                        GetValidMoves(this.info.Speed, false))),
                Vector3.one
            );

            int range = 6;

            // show line of sight to player
            if(LineOfSight(this.TilePosition(), TileManager.PositionToTile(playerpos), range)) {
                Gizmos.color = Cyan;
            } else {
                Gizmos.color = Yellow;
            }
            Gizmos.DrawRay(TileManager.TileToPosition(this.TilePosition()),(playerpos-TileManager.TileToPosition(this.TilePosition())).normalized*range);

            //show the farthest move with line of sight
            Gizmos.color = new Color(0f,1f,1f,0.5f);
            Gizmos.DrawCube(
                TileManager.TileToPosition(
                    GetMoveFarthest(
                        TileManager.PositionToTile(playerpos),
                        GetMovesWithLOSTo(
                            TileManager.PositionToTile(playerpos),
                            GetValidMoves(this.info.Speed, false),
                            range
                        )
                    )
                ),
                Vector3.one
            );

            // check if the cleave attack will connect
            if(CheckAttack(this.TilePosition(), TileManager.PositionToTile(playerpos), AttackShape.AttackDictionary[AttackShape.AttackKeys.Cleave])) {
                Gizmos.color = Green;
            } else {
                Gizmos.color = Yellow;
            }
            // draw attack hitbox in direction of player
            foreach(var tile in AttackShapeBuilder.AttackAt(
                AttackShape.AttackDictionary[AttackShape.AttackKeys.Cleave],
                AttackShapeBuilder.VecToDir[GetOrthoDir(this.TilePosition(), TileManager.PositionToTile(playerpos))],
                this.TilePosition()
                ).AttackTiles )
            {
                Gizmos.DrawCube(TileManager.TileToPosition(tile.Position), Vector3.one);
            }


        }
    }
}
