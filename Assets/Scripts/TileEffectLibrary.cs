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
    public Dictionary<TileEffectKey, GameObject> TileEffects;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            TileEffects = new Dictionary<TileEffectKey, GameObject>()
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
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }
}
