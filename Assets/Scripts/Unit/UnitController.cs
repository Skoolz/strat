using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Linq;

public class UnitController : MonoBehaviour
{
    public static UnitController instance {get; private set;}
    public Unit selectedUnit;
    public Dictionary<Vector3Int,ReachTile> range;
    public Tilemap navMap;
    public Tilemap targetMap;
    public List<Vector3Int>currentPath = new List<Vector3Int>();
    public PathTile returnTile;

    [SerializeField] private Tile reachableTile = null;
    [SerializeField] private Tile pathTile = null;
    [SerializeField] private Tile nextAllowTile = null;
    [SerializeField] private Tile targetTile = null;
    [SerializeField] private Tile attackFormTile = null;

    Vector3Int selectedTile;
    Dictionary<Vector3Int,Unit>targets = new Dictionary<Vector3Int, Unit>();
    Dictionary<Vector3Int,ReachTile>attackRange = new Dictionary<Vector3Int, ReachTile>();

    public ActiveAbilityManager activeAbilityManager;

    private void Awake() {
        instance = this;
    }

    public void Clear() {
        navMap.ClearAllTiles();
        targetMap.ClearAllTiles();
        currentPath.Clear();
        targets.Clear();
    }

    public void SelectUnit(Unit unit) {
        if(this.selectedUnit != null) {
            UnSelect();
        }
        this.selectedUnit = unit;
        Clear();

        SetReachableTiles();
        ShowAttackRange(selectedUnit.tile);
        
        selectedUnit.OnPathEnd+=SetReachableTiles;
        selectedUnit.OnPathEnd+=() => ShowAttackRange(selectedUnit.tile);
        selectedUnit.OnChange+=OnUnitChange;
        MapObjectSelector.instance.Disable(true);

        activeAbilityManager.Init(selectedUnit);

    }

    public void UnSelect() {
        selectedUnit.OnPathEnd -= () => ShowAttackRange(selectedUnit.tile);
        selectedUnit.OnPathEnd -= SetReachableTiles;
        selectedUnit.OnChange-=OnUnitChange;
        this.selectedUnit = null;
        Clear();
        MapObjectSelector.instance.Disable(false);
    }

    public void SetReachableTiles() {
        Clear();
        range = RangeFinder.GetReachableTiles(this.selectedUnit.tile,this.selectedUnit.moveRange);
        foreach (var pos in range.Values) {
            if(pos.action) continue;
            navMap.SetTile(pos.tile,reachableTile);
        }

    }

    void Update() {

        if(EventSystem.current.IsPointerOverGameObject()) return;


        if(selectedUnit == null) return;


        navMap.SetTile(this.selectedUnit.tile,pathTile);

        if(selectedUnit.isMoving == true) {
            return;
        }

        if(Input.GetMouseButtonDown(0)) {

            if(attackRange.ContainsKey(selectedTile) == false) {
                return;
            }

            if(selectedUnit.IsCasting) {
                var tiles = GetAttackForm(selectedTile);
                foreach(Unit target in new List<Unit>(targets.Values)) {
                    if(tiles.Contains(target.tile)) {
                        selectedUnit.AttackEnemy(targets[target.tile]);
                        target.Highlight(false);
                    }
                }
                UnSelect();
                return;
            }

            if(targets.ContainsKey(selectedTile)) {
                selectedUnit.AttackEnemy(targets[selectedTile]);
                targets[selectedTile].Highlight(false);
                UnSelect();
                return;
            }

            if(currentPath.Count>0) {
                selectedUnit.Move(returnTile);
                Clear();
            }
        }

        if(selectedTile == SelectTile.instance.GetSelectedTile()) {
            return;
        }
        selectedTile = SelectTile.instance.GetSelectedTile();
        if(range.ContainsKey(selectedTile) &&
           selectedTile != selectedUnit.tile) {
            SetReachableTiles();
            bool completed = false;
            var tile = PathFinder.Find(selectedUnit.tile,selectedTile,range.Keys.ToList(),out completed);
            int needSteps = tile.counter;
            returnTile = new PathTile(tile.tile,null);
            if(completed) {
                while(tile != null) {
                    currentPath.Add(tile.tile);
                    navMap.SetTile(tile.tile,pathTile);
                    tile = tile.prev;
                    if(tile!= null) {
                        returnTile = new PathTile(tile.tile,returnTile);
                    }
                }
                
            }
        }
        else {
            SetReachableTiles();
        }
        ShowAttack();
    }

    public Vector3Int GetTile() {
        if(range.ContainsKey(selectedTile) && selectedUnit.attack == false) {
            return selectedTile;
        }
        return selectedUnit.tile;
    }

    public void ShowAttack() {
        Vector3Int tile = GetTile();
        ShowAttackRange(tile);
        FindTargets(attackRange.Keys.ToList());
        ShowAttackForm(selectedTile);
    }

    void ShowAttackRange(Vector3Int tile) {
        attackRange = RangeFinder.GetReachableTiles(tile,selectedUnit.GetUnitAttackRange(),true);
        foreach(var attackTile in attackRange.Values) {
            if(attackTile.action) continue;
            targetMap.SetTile(attackTile.tile,nextAllowTile);
        }
        ShowAttackForm(selectedTile);
    }

    List<Vector3Int>GetAttackForm(Vector3Int origin) {
        var tiles = new List<Vector3Int>();
        foreach(Vector3Int offset in SelectForm.forms[selectedUnit.attackForm].GetOffsets(origin)) {
            Vector3Int tile = origin+offset;
            if(BattleMap.instance.IsInBounds(tile) && attackRange.ContainsKey(tile)) tiles.Add(tile);
        }
        return tiles;
    }

    void ShowAttackForm(Vector3Int origin) {
        var tiles = GetAttackForm(origin);
        foreach(var attackTile in tiles) {
            targetMap.SetTile(attackTile,attackFormTile);
        }

        foreach(var target in targets.Keys) {
            if(tiles.Contains(target)) {
                targets[target].Highlight(true);
            }
            else {
                targets[target].Highlight(false);
            }
        }
    }

    void FindTargets(List<Vector3Int>tiles) {
        targets.Clear();
        var list = TargetFinder.Find(selectedUnit,tiles);
        foreach(var target in list) {
            targets.Add(target.tile,target);
            targetMap.SetTile(target.tile,targetTile);
        }
    }

    void OnUnitChange(Unit unit) {
        Clear();
        SetReachableTiles();
        ShowAttack();
    }


}
