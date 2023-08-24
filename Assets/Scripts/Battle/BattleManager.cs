using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance {get; private set;}
    public Side SideA {get; private set;}
    public Side SideB {get; private set;}

    public AssetManager assetManager;

    public Round CurrentRound;

    public GameObject gameOver;

    public GameObject mainUI;
    public GameObject startUI;
    

    private void Awake() {
        instance = this;
    }
    
    private void Start()
    {
        assetManager = new AssetManager();
        assetManager.Init();
    }

    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) {
            GameOver();
        }
    }

    public void StartGame() {
        mainUI.SetActive(true);
        startUI.SetActive(false);

        InitBattle();
    }

    public void InitBattle() {
        SideA = new Side(true);
        SideB = new Side(false);

        CreateUnit(new Vector2Int(-9,0),SideA,PrefabLibrary.instance.archer,40);
        CreateUnit(new Vector2Int(-9,1),SideA,PrefabLibrary.instance.knight,20);
        CreateUnit(new Vector2Int(-8,0),SideA,PrefabLibrary.instance.knight,20);
        CreateUnit(new Vector2Int(-9,-1),SideA,PrefabLibrary.instance.knight,20);
        CreateUnit(new Vector2Int(-9,-2),SideA,PrefabLibrary.instance.cannon,1);

        CreateUnit(new Vector2Int(9,1),SideB,PrefabLibrary.instance.skeleton,50);
        CreateUnit(new Vector2Int(9,-1),SideB,PrefabLibrary.instance.zombie,1);


        CurrentRound = new Round();
        CurrentRound.Create(SideA,SideB);

    }

    public Unit CreateUnit(Vector2Int pos,Side side, GameObject prefab, int count) {
        var newUnit_gm = GameObject.Instantiate(prefab);
        var unit = newUnit_gm.GetComponent<Unit>();
        unit.Init(count);
        side.AddUnit(unit);
        unit.SetTile(new Vector3Int(pos.x,pos.y,0),unit.side.left);
        unit.OnDead+=UnitDead;
        return unit;
    }

    public void Battle(Unit attack, Unit defender) {
        var range = attack.CalculateDamage(true);
        float damage = defender.CalculateRecieveDamage(Random.Range((float)range.min,range.max),true);
        defender.GetHit(damage);

        foreach(Ability ability in attack.GetAbilities(AbilityType.AttackUnit)) {
            ability.Apply(attack,defender);
        }

        attack.attack = false;
        if(attack.IsCasting) {
            attack.Cast();
        }
    }

    public Range<int> CalculateDamageRange(Unit attack, Unit defender) {
        var range = attack.CalculateDamage(false);
        

        int min = (int)Mathf.Floor(defender.CalculateRecieveDamage(range.min,false)/defender.stats.health);
        int max = (int)Mathf.Ceil(defender.CalculateRecieveDamage(range.max,false)/defender.stats.health);


        min = Mathf.Clamp(min,0,defender.size);
        max = Mathf.Clamp(max,0,defender.size);
        return new Range<int>(min,max);
    }

    public void Turn() {
        if(CurrentRound.IsEnd) {
            NewRound();
            return;
        }
        CurrentRound.Turn();
    }

    public void NewRound() {

        foreach(var unit in new List<Unit>(SideA.units)) {
            unit.Rest();
        }
        foreach(var unit in new List<Unit>(SideB.units)) {
            unit.Rest();
        }

        CurrentRound = new Round();
        CurrentRound.Create(SideA,SideB);
    }

    public void UnitDead(Unit unit) {
        unit.side.RemoveUnit(unit);
        CurrentRound.DeleteUnit(unit);

        if(SideA.units.Count == 0 || SideB.units.Count == 0) {
            GameOver();
        }
    }

    public void Clear() {
        var units = SideA.units;
        units.AddRange(SideB.units);

        foreach(Unit unit in units) {
            Destroy(unit.gameObject);
        }
    }

    public void GameOver() {
        Clear();
        startUI.SetActive(true);
        mainUI.SetActive(false);
    }
}
