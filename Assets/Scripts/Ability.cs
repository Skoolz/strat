using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;

public enum AbilityType {
    PreDamage,
    GetDamage,
    EndRound,
    Attack,
    AttackUnit,
    Passive,
}

public class Ability:Asset
{
    public AbilityType abilityType;
    public bool resetable;
    public bool IsActive = true;
    

    public virtual bool CanCast(Unit unit) {
        return true;
    }

    public virtual object Apply(Unit unit, params object[]addParams) {
        return null;
    }

    public virtual object Show(Unit unit, params object[]addParams) {
        return null;
    }

    public void Copy(Ability ability) {
        this.id = ability.id;
        this.abilityType = ability.abilityType;
        this.resetable = ability.resetable;
        this.IsActive = ability.IsActive;
    }

    public virtual bool Reset() {
        return true;
    }
}


public class KnightDamageAbility : Ability {
    
    public int count = 3;
    
    public override object Show(Unit unit, params object[]addParams) {
        if(count == 3) {
            return 0.99f;
        }
        else if(count==2) {
            return 0.66f;
        }
        else if(count == 1) {
            return 0.33f;
        }
        return 0;
    }

    public override object Apply(Unit unit, params object[]addParams) {
        float value = (float)Show(unit);
        count-=1;
        return value;
    }

    public override bool CanCast(Unit unit) {
        return count>0;
    }
    
}


public class KnightAddRoundAbility : Ability {

    public bool secondTurn = false;

    public override object Apply(Unit unit, params object[]addParams) {
        if(UnityEngine.Random.Range(0.0f,1.0f) <= 0.45f) {
            secondTurn = true;
            unit.Rest(false);
            return false;
        }
        return true;
    }

    public override bool CanCast(Unit unit) {
        return secondTurn == false;
    }
}


public class ArcherAbility:Ability {

    public override object Apply(Unit unit, params object[]addParams) {
        
        if(unit.attack == false) {
            this.IsActive = true;
            Attack(unit);
            return null;
        }
        return Show(unit);
    }
    public void Attack(Unit unit) {
        unit.attack = true;
        unit.SetAttackForm(SelectFormType.twohex);
    }

    public override object Show(Unit unit,params object[] addParams) {
        return -0.5f;
    }

    public override bool CanCast(Unit unit) {
        return IsActive;
    }

    public override bool Reset() {
        IsActive=false;
        return true;
    }

}

public class SkeletonAbility:Ability {

    int charge = 2;

    public override object Apply(Unit unit, params object[]addParams) {
        int changeSize = (int)addParams[0];
        SpawnSkeletonDerive(unit,changeSize);
        return null;
    }

    public void SpawnSkeletonDerive(Unit unit,int size) {
        var neighbours = TileManager.instance.GetNeighbours(unit.tile);
        neighbours = neighbours.Where(x=>BattleMap.instance.IsOccupiedTile(x) == false).ToList();

        if(neighbours.Count == 0) return;

        int randIndex = UnityEngine.Random.Range(0,neighbours.Count);
        var tile = neighbours[randIndex];

        GameObject skeleton_derive;
        if(charge == 2) {
            skeleton_derive = PrefabLibrary.instance.skeletonDeriveFull;
        }
        else if(charge == 1) {
            skeleton_derive = PrefabLibrary.instance.skeletonDeriveHalf;
        }
        else {
            skeleton_derive = PrefabLibrary.instance.skeletonDeriveEmpty;
        }
        BattleManager.instance.CreateUnit(new Vector2Int(tile.x,tile.y),unit.side,skeleton_derive,size);
        charge-=1;
    }

    public override bool Reset() {
        if(charge < 2) charge+=1;
        return true;
    }
}

public class ZombieAbility:Ability {
    public override object Apply(Unit unit, params object[]addParams) {
        Unit defender = addParams[0] as Unit;
        ControlUnit(unit,defender);
        return null;
    }

    public void ControlUnit(Unit zombie, Unit unit) {
        ZombieControlledAbility ability = BattleManager.instance.assetManager.abilityLibrary.Create("zombie_controlled") as ZombieControlledAbility;
        unit.AddAbility(ability);
        ability.Apply(unit,zombie);
    }
}

public class ZombieControlledAbility :Ability {
    public Unit zombie;
    public Side unitSide;
    public Unit unit;

    public override object Apply(Unit unit, params object[]addParams) {
        this.unit = unit;
        this.zombie = addParams[0] as Unit;
        unitSide = unit.side;
        zombie.side.AddUnit(unit);
        return null;
    }

    public override bool Reset() {
        unitSide.AddUnit(unit);
        return false;
    }
}

public class AbilityLibrary : AssetLibrary<Ability> {
    public override void Init() {
        Ability ability = new KnightDamageAbility() {
            id = "knight_damage_ability",
            abilityType = AbilityType.PreDamage,
        };
        this.Add(ability);

        ability = new KnightAddRoundAbility() {
            id = "knight_add_round_ability",
            abilityType = AbilityType.EndRound,
        };
        this.Add(ability);

        ability = new ArcherAbility() {
            id = "archer_ability",
            abilityType = AbilityType.Attack,
            IsActive = false,
            resetable = true,
        };
        this.Add(ability);

        ability = new SkeletonAbility() {
            id="skeleton_ability",
            abilityType = AbilityType.GetDamage,
            resetable = true
        };
        this.Add(ability);

        ability = new ZombieAbility() {
            id="zombie_ability",
            abilityType = AbilityType.AttackUnit,
        };
        this.Add(ability);

        ability = new ZombieControlledAbility() {
            id="zombie_controlled",
            abilityType = AbilityType.Passive,
            resetable = true
        };
        this.Add(ability);
    }

    public Ability Create(string id) {
        Ability ability = this.Get(id);
        Ability newAbility = Activator.CreateInstance(ability.GetType()) as Ability;
        newAbility.Copy(ability);
        return newAbility;
    }
}

