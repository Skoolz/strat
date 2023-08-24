using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType {
    Melee,
    Range,
}

public class UnitStats:Asset
{
    public Dictionary<string,object>dict = new Dictionary<string, object>();
    public List<string>abilities = new List<string>();
    public List<string>activeAbilities = new List<string>();
    public int moveRange;
    public int attackRange;
    public float moveSpeed = 0.05f;
    public int health;
    public Range<int> damage;
    public int initiative = 1;
    public UnitType unitType;
    public SelectFormType attackForm;
}
