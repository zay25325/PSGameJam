/*
File : AttackShapeBuilder.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackShape;

public static class AttackShapeBuilder
{
    public enum Direction
    {
        Right, // All attacks MUST have a default orientation of Right
        Down,
        Left,
        Up,
    }

    // this is used in the enemy controller
    [System.NonSerialized] public static readonly Dictionary<Vector2Int, Direction> VecToDir = new (){
        {new Vector2Int(1,0), Direction.Right},
        {new Vector2Int(0,-1),Direction.Down},
        {new Vector2Int(-1,0),Direction.Left},
        {new Vector2Int(0,1), Direction.Up} 
    };

    public static AttackShape AttackAt(AttackShape attackShape, Direction direction, Vector2Int origin)
    {
        AttackShape attack = new AttackShape(attackShape);
        ToDirection(attack, direction);
        ToOrigin(attack, origin);
        return attack;
    }

    private static void ToDirection(AttackShape attackShape, Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                break; // right is default orientation
            case Direction.Down:
                foreach (AttackTile tile in attackShape.AttackTiles)
                {
                    tile.Position = new Vector2Int(tile.Position.y, -tile.Position.x);
                }
                break;
            case Direction.Left:
                foreach (AttackTile tile in attackShape.AttackTiles)
                {
                    tile.Position = new Vector2Int(-tile.Position.x, tile.Position.y);
                }
                break;
            case Direction.Up:
                foreach (AttackTile tile in attackShape.AttackTiles)
                {
                    tile.Position = new Vector2Int(tile.Position.y, tile.Position.x);
                }
                break;
        }
    }

    private static void ToOrigin(AttackShape attackShape, Vector2Int origin)
    {
        foreach (AttackTile tile in attackShape.AttackTiles)
        {
            tile.Position += origin;
        }
    }
}
