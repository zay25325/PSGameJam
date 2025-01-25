using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShape
{
    public enum Target
    {
        Self,
        Touch,
        Ranged
    }


    public Target TargetType;
    public List<AttackTile> AttackTiles;
    public int ActivationCount;

    public AttackShape(Target targetType, List<AttackTile> tiles, int activationCount)
    {
        TargetType = targetType;
        AttackTiles = tiles;
        ActivationCount = activationCount;
    }

    public AttackShape(AttackShape cloneSource)
    {
        TargetType = cloneSource.TargetType;

        List<AttackTile> tiles = new List<AttackTile>();
        foreach (AttackTile tile in cloneSource.AttackTiles)
        {
            tiles.Add(new AttackTile(tile));
        }

        AttackTiles = tiles;
        ActivationCount = cloneSource.ActivationCount;
    }


    public enum AttackKeys // string keys are evil
    {
        Cleave
    }

    public static Dictionary<AttackKeys, AttackShape> AttackDictionary = new Dictionary<AttackKeys, AttackShape>()
    {
        { AttackKeys.Cleave, new AttackShape(
        Target.Touch,
        new List<AttackTile>()
        {
            new AttackTile(new Vector2Int(0, 1), 1), // up
            new AttackTile(new Vector2Int(1, 1), 1), // up-right
            new AttackTile(new Vector2Int(1, 0), 1), // right
            new AttackTile(new Vector2Int(1, -1), 1), // down-right
            new AttackTile(new Vector2Int(0, -1), 1), // down
        }, 1) }
    };
}
