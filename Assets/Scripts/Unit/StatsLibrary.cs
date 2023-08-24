using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsLibrary : AssetLibrary<UnitStats>
{
    public override void Init() {
        UnitStats stats = new UnitStats() {
            id = "knight",
            moveRange = 5,
            attackRange = 1,
            health = 20,
            damage = new Range<int>(6,10),
            initiative = 4,
            unitType = UnitType.Melee,
            abilities = new List<string>() {
                "knight_damage_ability",
                "knight_add_round_ability"
            },
            attackForm = SelectFormType.tile,

        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "archer",
            moveRange = 6,
            attackRange = 1,
            health = 10,
            initiative = 6,
            unitType = UnitType.Range,
            damage = new Range<int>(1,6),
            dict = new Dictionary<string, object>() {
                {"dist_damage_range",new Range<int>(6,15)},
                {"dist_attack_range",6}
            },
            activeAbilities = new List<string>() {
                "archer_active_ability"
            },
            attackForm = SelectFormType.tile,
        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "skeleton",
            moveRange = 3,
            attackRange = 1,
            health = 6,
            initiative = 6,
            unitType = UnitType.Melee,
            damage = new Range<int>(1,6),
            abilities = new List<string>() {
                "skeleton_ability"
            },
            attackForm = SelectFormType.tile,
        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "skeleton_d_0",
            moveRange = 3,
            attackRange = 1,
            health = 1,
            initiative = 1,
            unitType = UnitType.Melee,
            damage = new Range<int>(1,1),
            attackForm = SelectFormType.tile,
        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "skeleton_d_1",
            moveRange = 3,
            attackRange = 1,
            health = 3,
            initiative = 3,
            unitType = UnitType.Melee,
            damage = new Range<int>(1,3),
            attackForm = SelectFormType.tile,
        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "skeleton_d_2",
            moveRange = 3,
            attackRange = 1,
            health = 4,
            initiative = 4,
            unitType = UnitType.Melee,
            damage = new Range<int>(1,4),
            attackForm = SelectFormType.tile,
        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "zombie",
            moveRange = 2,
            attackRange = 1,
            health = 3000,
            initiative = 4,
            unitType = UnitType.Melee,
            damage = new Range<int>(0,0),
            attackForm = SelectFormType.tile,
            abilities = new List<string>() {
                "zombie_ability"
            }
        };
        this.Add(stats);

        stats = new UnitStats() {
            id = "cannon",
            moveRange = 1,
            attackRange = 1,
            health = 500,
            initiative = 1,
            unitType = UnitType.Range,
            damage = new Range<int>(0,0),
            attackForm = SelectFormType.tile,
            activeAbilities = new List<string>() {
                "cannon_active_ability"
            },
            dict = new Dictionary<string, object>() {
                {"dist_damage_range",new Range<int>(1,2000)},
                {"dist_attack_range",0}
            },
        };
        this.Add(stats);
    }
}
