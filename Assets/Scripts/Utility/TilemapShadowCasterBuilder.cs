using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.Universal;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Tilemap))]
public class TilemapShadowCasterBuilder : MonoBehaviour
{
    void OnEnable()
    {
        var tilemap = GetComponent<Tilemap>();
        Tilemap.tilemapTileChanged += (inTilemap, _) =>
        {
            if (inTilemap == tilemap)
            {
                UpdateShadowCasters();
            }
        };
        UpdateShadowCasters();
    }
    IEnumerable<Vector2Int> AllTiles()
    {
        var tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            yield break;
        }
        var bounds = tilemap.cellBounds;
        var tiles = tilemap.GetTilesBlock(bounds);
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                var tile = tiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    yield return new Vector2Int(bounds.min.x + x, bounds.min.y + y);
                }
            }
        }

    }
    IEnumerable<Vector3> AllTilePositions()
    {
        var tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            yield break;
        }
        foreach (var tilePos in AllTiles())
        {
            yield return tilemap.CellToWorld(new Vector3Int(tilePos.x, tilePos.y, 0)) + tilemap.GetLayoutCellCenter();
        }

    }
    void UpdateShadowCasters()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);

        }
        foreach (var position in AllTilePositions())
        {
            GameObject go = new GameObject("ShadowCaster");
            go.transform.SetParent(transform);
            go.transform.position = position;
            var caster = go.AddComponent<ShadowCaster2D>();
            caster.selfShadows = true;
        }
    }

}
