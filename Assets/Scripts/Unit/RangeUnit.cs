using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUnit : Unit
{
    public bool canShoot {get; private set;} = true;
    public bool distBlocked = false;

    public int distAttackRange;

    public override void Init(int size) {
        base.Init(size);
        distAttackRange = (int)stats.dict["dist_attack_range"];
    }

    public override Range<int> CalculateBaseDamage() {
        if(distBlocked == true) return new Range<int>(stats.damage.min*size,stats.damage.max*size);
        var rangeDamage = this.stats.dict["dist_damage_range"] as Range<int>;
        return new Range<int>(rangeDamage.min*size,rangeDamage.max*size); 
    }

    public override void UpdateNeighbourTile(Vector3Int tile) {
        UpdateDist();
    }

    public bool IsNearEnemy() {
        var neighbours = TileManager.instance.GetNeighbours(this.tile);

        foreach(var neighbour in neighbours) {
            var _object = BattleMap.instance.GetOccupiedTile(neighbour);

            if(_object != null && _object is Unit && this.IsEnemy(_object as Unit)) {
                return true;
            }
        }

        return false;
    }

    public override int GetUnitAttackRange() {
        if(distBlocked) {
            return this.stats.attackRange;
        }
        return (int)distAttackRange;
    }

    public void UpdateDist() {
        distBlocked = IsNearEnemy();
    }

    public override void ChangeTile() {
        UpdateDist();
    }
}
