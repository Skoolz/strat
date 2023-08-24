using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side
{
    public bool AI {get; private set;}= false;
    public List<Unit>units {get; private set;} = new List<Unit>();

    public bool left {get; private set;}

    public Side(bool left) {
        this.left = left;
    }

    public void AddUnit(Unit unit) {
        units.Add(unit);
        unit.SetSide(this);
    }
    
    public void RemoveUnit(Unit unit) {
        units.Remove(unit);
    }
}
