using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public static List<Vector3Int>offsets = new List<Vector3Int>() {
        new Vector3Int(-1,0,0),
        new Vector3Int(1,0,0),
        new Vector3Int(0,-1,0),
        new Vector3Int(0,1,0),
    };

    public static List<Vector3Int>odd_off = new List<Vector3Int>() {
        new Vector3Int(1,-1,0),
        new Vector3Int(1,1,0)
    };

    public static List<Vector3Int>even_off = new List<Vector3Int>() {
        new Vector3Int(-1,1,0),
        new Vector3Int(-1,-1,0)
    };


    public Grid Map;
    public static TileManager instance;
    private Dictionary<TileBase,TileData>tileData;
    
    void Awake() {
        instance = this;
    }

    void Start()
    {
        Map = gameObject.GetComponent<Grid>();

        var resources = Resources.LoadAll<TileData>("Titles/Data");
        tileData = new Dictionary<TileBase, TileData>();

        foreach (var resource in resources) {
            tileData.Add(resource.tile,resource);
        }

        Debug.Log(tileData.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 TileToWorld(Vector3Int pos) {
        return Map.CellToWorld(pos);
    }

    public float Distance(Vector3Int pos1, Vector3Int pos2) {
        return math.abs(pos1.x-pos2.x) + math.abs(pos1.y-pos2.y);
    }

    public TileData GetTileData(TileBase tile) {
        return tileData[tile];
    }

    public List<Vector3Int>GetNeighbours(Vector3Int pos) {
        List<Vector3Int>neighbours = new List<Vector3Int>();
        foreach (var off in offsets) {
            if(BattleMap.instance.IsInBounds(pos+off)) neighbours.Add(pos+off);
        }
        var add_off = pos.y % 2 == 0 ? even_off : odd_off;

        foreach(var off in add_off) {
            if(BattleMap.instance.IsInBounds(pos+off)) neighbours.Add(pos+off);
        }
        return neighbours;
    }
}
