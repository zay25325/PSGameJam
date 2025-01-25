using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] Tilemap intentMap;
    [SerializeField] Tile intentTile;

    List<AttackShape> shapes = new List<AttackShape>();

    public static TileManager Instance;


    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIntentGlow();
    }

    public void ClearShapes() // This is just for testing and probably shouldn't be used. maybe after ever turn though
    {
        shapes.Clear();
        intentMap.ClearAllTiles();
    }

    public void AddShape(AttackShape shape)
    {
        shapes.Add(shape);
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
}
