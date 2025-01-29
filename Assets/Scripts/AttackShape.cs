/*
File : AttackShape.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileEffectLibrary;
using static AttackShapeBuilder;

public class AttackShape
{
    public enum Target
    {
        Touch,
        Ranged
    }


    public Target TargetType;
    public Direction AttackDirection;
    public List<AttackTile> AttackTiles;
    public int ActivationCount;
    public TileEffectKey TileAnimation;
    public AttackShape ChainAttack;
    public CharacterInfo Caster;

    public AttackShape(Target targetType, List<AttackTile> tiles, int activationCount, AttackShape chainAttack, TileEffectKey tileAnimation)
    {
        TargetType = targetType;
        AttackTiles = tiles;
        ActivationCount = activationCount;
        TileAnimation = tileAnimation;
        ChainAttack = chainAttack;
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
        TileAnimation = cloneSource.TileAnimation;
        ChainAttack = cloneSource.ChainAttack;
        Caster = cloneSource.Caster;
    }


    public enum AttackKeys // string keys are evil
    {
        Cleave,
        XLightning,
        FireBolt,
        FlameSpray,
        FlameBall,
        LightningRay,
    }

    public static Dictionary<AttackKeys, AttackShape> AttackDictionary = new Dictionary<AttackKeys, AttackShape>()
    {
        // cleave
        { AttackKeys.Cleave, new AttackShape( Target.Touch,
        new List<AttackTile>()
        {
            new AttackTile(new Vector2Int(0, 1), 1), // up
            new AttackTile(new Vector2Int(1, 1), 1), // up-right
            new AttackTile(new Vector2Int(1, 0), 1), // right
            new AttackTile(new Vector2Int(1, -1), 1), // down-right
            new AttackTile(new Vector2Int(0, -1), 1), // down
        }, activationCount: 1, null, TileEffectKey.Claw) },

        // XLightning
        { AttackKeys.XLightning, new AttackShape( Target.Touch,
        new List<AttackTile>()
        {
            new AttackTile(new Vector2Int(0, 0), 1), // self

            new AttackTile(new Vector2Int(1, 1), 1), // up-right
            new AttackTile(new Vector2Int(1, -1), 1), // down-right
            new AttackTile(new Vector2Int(-1, -1), 1), // down-left
            new AttackTile(new Vector2Int(-1, 1), 1), // up-left

            new AttackTile(new Vector2Int(2, 2), 1), // up-right
            new AttackTile(new Vector2Int(2, -2), 1), // down-right
            new AttackTile(new Vector2Int(-2, -2), 1), // down-left
            new AttackTile(new Vector2Int(-2, 2), 1), // up-left
        }, activationCount: 1, null, TileEffectKey.Lightning) },
        
        // fire bolt
        { AttackKeys.FireBolt, new AttackShape( Target.Ranged,
        new List<AttackTile>()
        {
            new AttackTile(new Vector2Int(0, 0), 1), // target
        }, activationCount: 1, null, TileEffectKey.BoltFire) },

        // flame spray
        { AttackKeys.FlameSpray, new AttackShape( Target.Touch,
        new List<AttackTile>()
        {
            new AttackTile(new Vector2Int(1, 1), 1), // up-right
            new AttackTile(new Vector2Int(1, 0), 1), // right
            new AttackTile(new Vector2Int(1, -1), 1), // down-right

            new AttackTile(new Vector2Int(2, 2), 1), // up-right
            new AttackTile(new Vector2Int(2, 1), 1), // up-right
            new AttackTile(new Vector2Int(2, 0), 1), // right
            new AttackTile(new Vector2Int(2, -1), 1), // down-right
            new AttackTile(new Vector2Int(2, -2), 1), // down-right
        }, activationCount: 1, null, TileEffectKey.Flame) },

        // LightningRay
        { AttackKeys.LightningRay, new AttackShape( Target.Touch,
        new List<AttackTile>()
        {
            new AttackTile(new Vector2Int(1, 0), 1), // right
            new AttackTile(new Vector2Int(2, 0), 1), // right
            new AttackTile(new Vector2Int(3, 0), 1), // right
        }, activationCount: 1, null, TileEffectKey.Lightning) },

    };
}
