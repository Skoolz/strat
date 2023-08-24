using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TargetFinder
{
    public static List<Unit> Find(Unit unit, List<Vector3Int>tiles) {
        List<Unit>targetsPos = new List<Unit>();
        foreach(var pos in tiles) {
            var _object = BattleMap.instance.GetOccupiedTile(pos);
            if(_object != null && 
            _object is Unit &&
            unit.IsEnemy(_object as Unit)) {
                targetsPos.Add(_object as Unit);
            }
        }
        return targetsPos;
    }
}
