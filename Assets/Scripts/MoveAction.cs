/*
File : MoveAction.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileManager;

public class MoveAction
{
    public Vector2Int MoveFrom;
    public Vector2Int MoveTo;
    public CharacterInfo Character;

    public MoveAction(Vector2Int from, Vector2Int to, CharacterInfo character)
    {
        MoveFrom = from;
        MoveTo = to;
        Character = character;
    }

    public MoveAction(Vector3 from, Vector3 to, CharacterInfo character) 
    {
        MoveFrom = PositionToTile(from);
        MoveTo = PositionToTile(to); 
        Character = character;
    }
}
