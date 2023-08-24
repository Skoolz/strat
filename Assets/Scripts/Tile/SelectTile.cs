using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class SelectTile : MonoBehaviour
{
    public static SelectTile instance {get; private set;}
    private Grid grid;
    [SerializeField] private Tilemap interactiveMap = null;
    [SerializeField] private Tilemap unitMap = null;
    [SerializeField] private Tilemap pathMap = null;
    [SerializeField] private Tile hoverTile = null;
    [SerializeField] private Tile reachableTile = null;
    [SerializeField] private Tile selectedTile = null;

    private bool needUpdate = true;


    private Vector3Int previousMousePos = new Vector3Int();


    public Unit testObject;
    public Vector3Int saved_tile;

    private void Awake() {
        instance = this;
    }

    void Start() {
        grid = gameObject.GetComponent<Grid>();
        interactiveMap = gameObject.GetComponentInChildren<Tilemap>();
    }


    void Update() {

        // Mouse over -> highlight tile
        Vector3Int mousePos = GetMousePosition();
        if (!mousePos.Equals(previousMousePos) && needUpdate) {
            if(BattleMap.instance.IsInBounds(previousMousePos) == true) {
                interactiveMap.SetTile(previousMousePos, null); // Remove old hoverTile
            }
            if(BattleMap.instance.IsInBounds(mousePos) == true) {
                interactiveMap.SetTile(mousePos, hoverTile);
            }
            saved_tile = mousePos;

            previousMousePos = mousePos;

        }

        // Left mouse click -> add path tile
        //if (Input.GetMouseButton(0)) {
        //    pathMap.SetTile(mousePos, pathTile);
        //}

        // Right mouse click -> remove path tile
        //if (Input.GetMouseButton(1)) {
        //    pathMap.SetTile(mousePos, null);
        //}

    }

    Vector3Int GetMousePosition () {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var vec = grid.WorldToCell(mouseWorldPos);
        vec.z = 0;
        return vec;
    }

    public void ClearUnitMap() {
        unitMap.ClearAllTiles();
    }

    public Vector3Int GetSelectedTile() {
        return saved_tile;
    }
    
}
