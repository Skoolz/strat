using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleMap : MonoBehaviour
{
    [SerializeField] public Tilemap map {
        get; private set;
    }
    public Vector2Int bounds_min {get; private set;}
    public Vector2Int bounds_max {get; private set;}

    public static BattleMap instance;

    public BaseMapObject[,] occupiedMap;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        map = gameObject.GetComponent<Tilemap>();

        map.CompressBounds();

        bounds_min = new Vector2Int(map.cellBounds.min.x,map.cellBounds.min.y);
        bounds_max = new Vector2Int(map.cellBounds.max.x-1,map.cellBounds.max.y-1);

        int sizeX = bounds_max.x-bounds_min.x;
        int sizeY = bounds_max.y-bounds_min.y;

        occupiedMap = new BaseMapObject[sizeX+1,sizeY+1];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMoveable(Vector3Int tile) {
        return TileManager.instance.GetTileData(map.GetTile(tile)).walkable &&
        IsOccupiedTile(tile) == false;
    }

    public bool IsInBounds(Vector3Int pos) {
        return pos.x >= bounds_min.x && pos.x <= bounds_max.x
                    && pos.y >= bounds_min.y && pos.y <= bounds_max.y;
    }

    public bool IsOccupiedTile(Vector3Int tile) {
        return GetOccupiedTile(tile) != null;
    }

    public void OccupyTile(BaseMapObject baseMapObject) {
        Vector2Int pos = TileToOccupied(baseMapObject.tile);
        occupiedMap[pos.x,pos.y] = baseMapObject;

        var neighbours = TileManager.instance.GetNeighbours(baseMapObject.tile);

        foreach(var neighbour in neighbours) {
            GetOccupiedTile(neighbour)?.UpdateNeighbourTile(baseMapObject.tile);
        }
    }

    public void DeoccupyTile(BaseMapObject baseMapObject) {
        Vector2Int pos = TileToOccupied(baseMapObject.tile);
        occupiedMap[pos.x,pos.y] = null;

        var neighbours = TileManager.instance.GetNeighbours(baseMapObject.tile);

        foreach(var neighbour in neighbours) {
            GetOccupiedTile(neighbour)?.UpdateNeighbourTile(baseMapObject.tile);
        }
    }

    public Vector2Int TileToOccupied(Vector3Int tile) {
        return new Vector2Int(tile.x-bounds_min.x,tile.y-bounds_min.y);
    }

    public BaseMapObject GetOccupiedTile(Vector3Int tile) {
        Vector2Int pos = TileToOccupied(tile);
        return occupiedMap[pos.x,pos.y];
    }
}
