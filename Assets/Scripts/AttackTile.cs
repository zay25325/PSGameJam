using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTile
{
    public Vector2Int Position;
    public int Damage;

    public AttackTile(Vector2Int pos, int damage)
    {
        Position = pos;
        Damage = damage;
    }

    public AttackTile(AttackTile tile) : this(tile.Position, tile.Damage) { }
}
