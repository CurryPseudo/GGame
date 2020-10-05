using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.Universal;
using System.Reflection;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Tilemap))]
public class TilemapShadowCasterBuilder : MonoBehaviour
{
#if UNITY_EDITOR
    void TileMapOnChange(Tilemap inTilemap, Tilemap.SyncTile[] tiles)
    {
        if (inTilemap == GetComponent<Tilemap>())
        {
            UpdateShadowCasters();
        }
    }
#endif
    void OnEnable()
    {
        var tilemap = GetComponent<Tilemap>();
#if UNITY_EDITOR
        Tilemap.tilemapTileChanged += TileMapOnChange;
#endif
        UpdateShadowCasters();
    }
    void OnDisable()
    {
#if UNITY_EDITOR
        Tilemap.tilemapTileChanged -= TileMapOnChange;
#endif
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);

        }
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
        var composite = GetComponent<CompositeCollider2D>();
        for (int i = 0; i < composite.pathCount; i++)
        {
            GameObject go = new GameObject("ShadowCaster");
            go.transform.SetParent(transform);
            var caster = go.AddComponent<ShadowCaster2D>();
            caster.selfShadows = true;
            var path = new Vector2[composite.GetPathPointCount(i)];
            composite.GetPath(i, path);
            var pathV3 = new Vector3[path.Length];
            for (int j = path.Length - 1; j >= 0; j--)
            {
                pathV3[j] = path[j];
            }
            SetShadowCasterPath(caster, pathV3);
            ShadowCasterReset(caster);
        }
    }
    public static Vector3[] GetShadowCasterPath(ShadowCaster2D shadowCaster)
    {
        FieldInfo field = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        return (Vector3[])field.GetValue(shadowCaster);
    }
    public static void SetShadowCasterPath(ShadowCaster2D shadowCaster, Vector3[] shadowPath)
    {
        FieldInfo field = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(shadowCaster, shadowPath);
    }
    public static void ShadowCasterReset(ShadowCaster2D shadowCaster)
    {
        FieldInfo field = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(shadowCaster, null);
        MethodInfo method = typeof(ShadowCaster2D).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(shadowCaster, new object[] { });
        method = typeof(ShadowCaster2D).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(shadowCaster, new object[] { });
    }
}
