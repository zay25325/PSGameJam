using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
File : AggressiveEnemy.cs
Project : PROG3126 - Hackathon
Programmer: Vincent Marshall
First Version: 1/29/2025

    This class derives from the enemycontroller
    This enemy will try to get a line of sight to the player, while keeping it's distance
    This enemy is intended to have ranged attacks
   
*/


public class TacticalEnemy : EnemyControllerBase
{
   
    // enemy's personal bubble will always attempt to move  when closer than this
    [SerializeField] private int minDistance = 4; //will retreat if player is this close
    [SerializeField] private int flankDistance = 2; // maximum distance we will shoot off target if player is obstructed

    protected override IntentType Think() {

        bool canAttack = false;
        AttackShape validAttack = null;
        Vector2Int playerPos = TileManager.Instance.GetPlayerLocation();
        AttackShape shape = AttackShape.AttackDictionary[this.info.Attacks[Random.Range(0,info.Attacks.Count)]]; // Goofy (get a random attack we have)

        bool isRetreating = false;


        //
        // Decide whether we want to move or attack
        //
        // are we too close? can't attack
        if((playerPos - this.GetPosition()).sqrMagnitude < minDistance*minDistance) {
            canAttack = false;
            isRetreating = true;
        } else {
            // always attack if player is in line of sight
            if(CheckAttack(this.GetPosition(), playerPos, shape, out validAttack)) {
                canAttack = true;
                this.preparedAttack = validAttack;
            } else {
                //try to attack within 3 tiles of the player
                Vector2Int[] attackOptions = GetAllMoves(playerPos,flankDistance);
                if(attackOptions.Length > 0) {
                    attackOptions = GetMovesWithLOSTo(this.GetPosition(),attackOptions,100);
                    if(attackOptions.Length > 0) {
                        this.targetTile = GetMoveClosest(playerPos,attackOptions);
                        this.preparedAttack = AttackShapeBuilder.AttackAt(shape, AttackShapeBuilder.VecToDir[GetOrthoDir(this.GetPosition(),playerPos)], this.targetTile);
                        canAttack = true;
                    } else {
                        canAttack = false; //reposition
                    }
                } else {
                    // we need to get a better angle
                    canAttack = false; //reposition
                }
            }
        }

        //
        // Decide where to move/attack
        //
        if(canAttack) {
            this.intentType = IntentType.Attack;
            this.targetTile = playerPos;

        } else if(isRetreating) {
            this.intentType = IntentType.Move;

            var moveOptions = GetValidMoves(this.info.Speed,false);
            if(moveOptions.Length > 0) {
                    // run away from the player
                    this.targetTile = GetMoveFarthest(playerPos,moveOptions);
            } else {
                //we are literally stuck, no move options
                this.targetTile = this.GetPosition();
            }

        } else { //is moving normally
            this.intentType = IntentType.Move;

            // find the closest square to US that has line of sight of the player
            Vector2Int[] moveOptions = GetValidMoves(this.info.Speed,false);
            if(moveOptions.Length > 0) {
                //find options with line of sight to player
                var moveOptionsLoS = GetMovesWithLOSTo(playerPos,moveOptions,100);
                if(moveOptionsLoS.Length > 0) {
                    // closest to us with los to player
                    this.targetTile = GetMoveClosest(this.GetPosition(),moveOptionsLoS);
                    this.preparedAttack = AttackShapeBuilder.AttackAt(shape, AttackShapeBuilder.VecToDir[GetOrthoDir(this.GetPosition(),playerPos)], this.GetPosition());
                } else {
                    //we can't gain line of sight so just wander towards the player
                    moveOptions = GetValidMoves(this.info.Speed/2,false);
                    this.targetTile = GetMoveClosest(playerPos,moveOptions);
                }
            } else {
                //we are literally stuck, no move options
                this.targetTile = this.GetPosition();
            }

        }
        return this.intentType;
    }


     private void OnDrawGizmosSelected() {
       
        if(Application.isPlaying) {
            this.GetIntent();

            var Green = new Color(0f,1f,0f,0.5f);; 
            var Blue = new Color(0f,0f,1f,0.5f);
            var Red = new Color(1f,0f,0f,0.5f);
            var Cyan =  new Color(1f,1f,0f,0.5f);
            var Yellow = new Color(1f,1f,0f,0.5f);

            if(this.intentType == IntentType.Move) {
                Gizmos.color = Blue;
                Gizmos.DrawCube(TileManager.TileToPosition(this.targetTile), Vector3.one);
            }

            if(this.intentType == IntentType.Attack) {
                Gizmos.color = Red;
                Gizmos.DrawCube(TileManager.TileToPosition(this.targetTile), Vector3.one);
                foreach(var tile in preparedAttack.AttackTiles) {
                    Gizmos.DrawCube(TileManager.TileToPosition(tile.Position), Vector3.one);
                }
            }
     
        }
     }
}
