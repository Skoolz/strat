using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Round 
{
    public List<Unit> units;
    public int currentTurn {get; private set;}

    public Unit currentUnit {
        get {
            return units[currentTurn];
        }
    }

    public bool IsEnd {
        get {
            return currentTurn+1 >= units.Count;
        }
    }

    public void Create(Side sideA, Side sideB) 
    {
        units = new List<Unit>();
        currentTurn = 0;
        List<Unit> unitsA = new List<Unit>(sideA.units);
        List<Unit> unitsB = new List<Unit>(sideB.units);

        unitsA = unitsA.OrderByDescending(x => x.stats.initiative).ToList();
        unitsB = unitsB.OrderByDescending(x => x.stats.initiative).ToList();

        int iteratorA = 0;
        int iteratorB = 0;
        bool lastAddedFromA = false;

        while (iteratorA < unitsA.Count || iteratorB < unitsB.Count)
        {
            if (iteratorA >= unitsA.Count)
            {
                units.Add(unitsB[iteratorB]);
                iteratorB+=1;
                continue;
            }

            if (iteratorB >= unitsB.Count)
            {
                units.Add(unitsA[iteratorA]);
                iteratorA+=1;
                continue;
            }

            if (unitsA[iteratorA].stats.initiative > unitsB[iteratorB].stats.initiative || 
                (unitsA[iteratorA].stats.initiative == unitsB[iteratorB].stats.initiative && !lastAddedFromA))
            {
                units.Add(unitsA[iteratorA]);
                lastAddedFromA = true;
                iteratorA+=1;
            }
            else
            {
                units.Add(unitsB[iteratorB]);
                lastAddedFromA = false;
                iteratorB+=1;
            }
        }

        SelectUnit();
    }

    public void SelectUnit() {
        UnitController.instance.SelectUnit(currentUnit);
    }

    public void Turn() {
        if(currentUnit.EndRound() == false) {
            SelectUnit();
            return;
        }
        this.currentTurn+=1;
        SelectUnit();
    }

    public void DeleteUnit(Unit unit) {
        units.Remove(unit);
        if(currentTurn >= units.Count) {
            BattleManager.instance.Turn();
        }
    }
}