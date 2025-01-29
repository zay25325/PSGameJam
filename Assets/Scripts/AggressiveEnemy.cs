using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
File : AggressiveEnemy.cs
Project : PROG3126 - Hackathon
Programmer: Vincent Marshall
First Version: 1/29/2025

    This class derives from the enemycontroller
    It is a basic intent script that will try to move towards the player
    It will try to attack the player as soon as it gets close to them
*/


public class AggressiveEnemy : EnemyControllerBase
{
    

    public override IntentType GetIntent() {

        bool canAttack = false;
        AttackShape validAttack = null;

        Vector2Int playerPos = TileManager.Instance.GetPlayerLocation();

        // choose the first one of our attacks that is in range
        foreach(AttackShape.AttackKeys key in this.info.Attacks) {
            var shape = AttackShape.AttackDictionary[key];
            if(CheckAttack(this.GetPosition(), playerPos, shape)) {
                validAttack = shape;
                canAttack = true;
                break;
            }
        }

        if(canAttack) {
            this.intentType = IntentType.Attack;
            this.targetTile = playerPos;
            this.preparedAttack = validAttack;
        } else {
            // move as close to the player as possible
            this.intentType = IntentType.Move;

            Vector2Int[] moveOptions = GetValidMoves(this.info.Speed,false);
            if(moveOptions.Length > 0) {
                this.targetTile = GetMoveClosest(playerPos, moveOptions);
            } else {
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
