using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActiveAbility : Asset
{
    public event Action<ActiveAbility> OnCast;
    public bool IsActivated {
        get {
            return unit.activeAbility == this; 
        }
    }
    public int cooldown;
    public int currentCooldown;

    public Unit unit;

    public virtual void Init(Unit unit) {
        this.unit = unit;
    }

    public virtual void Copy(ActiveAbility activeAbility) {
        this.id = activeAbility.id;
        this.cooldown = activeAbility.cooldown;

    }

    public virtual void Activate() {

    }

    public virtual void Deactivate() {

    }

    public virtual void Cast() {
        SetCooldown();
        if(OnCast!=null) OnCast(this);
    }

    public virtual void Update() {
        if(currentCooldown>0) currentCooldown-=1;
    }

    public virtual void SetCooldown() {
        this.currentCooldown = cooldown;
    }

    public virtual bool CanActivate() {
        return this.currentCooldown == 0;
    }
}

public class ArcherActiveAbility:ActiveAbility {
    public override void Activate() {
        unit.SetAttackForm(SelectFormType.twohex);
        unit.AddAbility("archer_ability");
    }

    public override void Deactivate() {
        unit.SetAttackForm(unit.stats.attackForm);
        unit.RemoveAbility("archer_ability");
    }

    public override void Cast() {
        base.Cast();
    }

    public override bool CanActivate() {
        return base.CanActivate() && (unit as RangeUnit).distBlocked == false;
    }
}

public class CannonActiveAbility:ActiveAbility {
    public override void Init(Unit unit) {
        base.Init(unit);
        this.currentCooldown = cooldown;
    }

    public override void Activate() {
        (unit as RangeUnit).distAttackRange = 6;
    }

    public override void Deactivate() {
        (unit as RangeUnit).distAttackRange = (int)unit.stats.dict["dist_attack_range"];
    }
}


public class ActiveAbilityLibrary : AssetLibrary<ActiveAbility> {
    public override void Init() {
        ActiveAbility ability = new ArcherActiveAbility() {
            id = "archer_active_ability",
            cooldown = 3,
        };
        this.Add(ability);

        ability = new CannonActiveAbility() {
            id = "cannon_active_ability",
            cooldown = 6,
        };
        this.Add(ability);
    }

    public ActiveAbility Create(string id) {
        ActiveAbility ability = this.Get(id);
        ActiveAbility newAbility = Activator.CreateInstance(ability.GetType()) as ActiveAbility;
        newAbility.Copy(ability);
        return newAbility;
    }
}