/*
File : TileEffectLibrary.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEffectLibrary : MonoBehaviour
{
    public enum TileEffectKey
    {
        Wheel, //TileAnimation
        Arcane,
        BoltFire,
        Claw,
        Flame,
        FlamingGround,
        Lightning,
        Orb,
        SpinDark,
        SpiralPoison,
    }

    public static TileEffectLibrary Instance;

    public Dictionary<TileEffectKey, GameObject> TileAnimations;
    [Header("Tile Animations")]
    [SerializeField] GameObject wheel;
    [SerializeField] GameObject Arcane;
    [SerializeField] GameObject BoltFlame;
    [SerializeField] GameObject Claw;
    [SerializeField] GameObject Flame;
    [SerializeField] GameObject FlamingGround;
    [SerializeField] GameObject Lightning;
    [SerializeField] GameObject Orb;
    [SerializeField] GameObject SpinDark;
    [SerializeField] GameObject SpiralPoison;


    public Dictionary<TileEffectKey, Sprite> TileUIs;
    [Header("UI")]
    [SerializeField] Sprite wheelUI;
    [SerializeField] Sprite ArcaneUI;
    [SerializeField] Sprite BoltFlameUI;
    [SerializeField] Sprite ClawUI;
    [SerializeField] Sprite FlameUI;
    [SerializeField] Sprite FlamingGroundUI;
    [SerializeField] Sprite LightningUI;
    [SerializeField] Sprite OrbUI;
    [SerializeField] Sprite SpinDarkUI;
    [SerializeField] Sprite SpiralPoisonUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
            TileAnimations = new Dictionary<TileEffectKey, GameObject>()
            {
                {TileEffectKey.Wheel, wheel },
                {TileEffectKey.Arcane, Arcane },
                {TileEffectKey.BoltFire, BoltFlame },
                {TileEffectKey.Claw, Claw },
                {TileEffectKey.Flame, Flame },
                {TileEffectKey.FlamingGround, FlamingGround },
                {TileEffectKey.Lightning, Lightning },
                {TileEffectKey.Orb, Orb },
                {TileEffectKey.SpinDark, SpinDark },
                {TileEffectKey.SpiralPoison, SpiralPoison },
            };

            TileUIs = new Dictionary<TileEffectKey, Sprite>()
            {
                {TileEffectKey.Wheel, wheelUI },
                {TileEffectKey.Arcane, ArcaneUI },
                {TileEffectKey.BoltFire, BoltFlameUI },
                {TileEffectKey.Claw, ClawUI },
                {TileEffectKey.Flame, FlameUI },
                {TileEffectKey.FlamingGround, FlamingGroundUI },
                {TileEffectKey.Lightning, LightningUI },
                {TileEffectKey.Orb, OrbUI },
                {TileEffectKey.SpinDark, SpinDarkUI },
                {TileEffectKey.SpiralPoison, SpiralPoisonUI },
            };
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
