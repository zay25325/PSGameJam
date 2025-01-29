/*
File : TileManager.cs
Project : PROG3126 - Hackathon
Programmer: Isaiah Bartlett
First Version: 1/24/2025
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static AttackShapeBuilder;

public class TileManager : MonoBehaviour
{
    [SerializeField] Tilemap intentMap;
    [SerializeField] Tilemap wallMap;
    [SerializeField] Tile intentTile;

    public List<MoveAction> Moves = new List<MoveAction>();
    public List<AttackShape> Attacks = new List<AttackShape>();
    public List<CharacterInfo> Characters = new List<CharacterInfo>();

    public static TileManager Instance;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Characters.AddRange(GetComponentsInChildren<CharacterInfo>());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIntentGlow();
    }

    public void ClearShapes() // This is just for testing and probably shouldn't be used. maybe after ever turn though
    {
        Moves.Clear();
        Attacks.Clear();
        intentMap.ClearAllTiles();
    }

    public void AddShape(AttackShape shape)
    {
        Attacks.Add(shape);
        UpdateIntentShape(shape);
    }

    private void UpdateIntentShape(AttackShape shape)
    {
        foreach (AttackTile tile in shape.AttackTiles)
        {
            intentMap.SetTile((Vector3Int)tile.Position, intentTile);
        }
    }

    private void UpdateIntentGlow()
    {
        float glowRate = 2;
        float alpha = (Mathf.Sin(Time.realtimeSinceStartup * glowRate) + 1) / 2;
        alpha = alpha * .8f + .1f;
        intentMap.color = new Color(intentMap.color.r, intentMap.color.g, intentMap.color.b, alpha);
    }

    public void EndTurn()
    {
        foreach(MoveAction move in Moves)
        {
            move.Character.transform.position = TileToPosition(move.MoveTo);
        }

        Dictionary<Vector2Int, CharacterInfo> tileToCharacter = new Dictionary<Vector2Int, CharacterInfo>();
        foreach (CharacterInfo character in Characters)
        {
            Vector2Int pos = PositionToTile(character.transform.position);
            tileToCharacter.Add(pos, character);
        }

        foreach(AttackShape attack in Attacks)
        {
            ProcessAttack(attack, tileToCharacter);
        }

        ClearShapes();
    }

    public void AddPlayerAttack(AttackShape attack, Direction direction, Vector3 position)
    {
        Attacks.Insert(0, AttackShapeBuilder.AttackAt(attack,
                direction, new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y))));
    }

    public void AddPlayerMove(MoveAction move)
    {
        Moves.Insert(0, move);
    }

    private void ProcessAttack(AttackShape attack, Dictionary<Vector2Int, CharacterInfo> tileToCharacter)
    {
        int timesActivated = 0;
        foreach (AttackTile tile in attack.AttackTiles)
        {
            AnimateTile(TileEffectLibrary.Instance.TileEffects[attack.TileAnimation], tile);
            if (tileToCharacter.ContainsKey(tile.Position))
            {
                tileToCharacter[tile.Position].HP -= tile.Damage;
                if (timesActivated < attack.ActivationCount && attack.ChainAttack != null)
                {
                    timesActivated++;
                    ProcessAttack(AttackShapeBuilder.AttackAt(attack.ChainAttack, attack.AttackDirection, tile.Position), tileToCharacter);
                }
            }
        }
    }

    private void AnimateTile(GameObject tileEffect, AttackTile tile)
    {
        GameObject go = GameObject.Instantiate(tileEffect, null);
        go.transform.position = TileToPosition(tile.Position);
    }

    public bool IsTileOccupied(Vector2Int pos)
    {
        if (wallMap.HasTile((Vector3Int)pos))
        {
            return true;
        }

        for (int i = 0; i < Characters.Count; i++)
        {
            if (PositionToTile(Characters[i].transform.position) == pos)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsTileOccupiedIgnoreCharacters(Vector2Int pos)
    {
        if (wallMap.HasTile((Vector3Int)pos))
        {
            return true;
        }

        return false;
    }

    public static Vector2Int PositionToTile(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
    }
    public static Vector3 TileToPosition(Vector2Int tilePos)
    {
        return new Vector3(tilePos.x + .5f, tilePos.y + .5f, 0);
    }
}
