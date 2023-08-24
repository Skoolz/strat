using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using System.Reflection;
using System.Linq;


public class Unit : BaseMapObject
{
    public UnitUI unitUI;
    public event Action<Unit> OnChange;
    public event Action OnHighlight;
    public Material highlightAllyMaterial;

    public UnitStats stats {get; private set;}
    public Side side {get; private set;}

    public string statId;
    public int moveRange;
    public int attackRange;


    public event Action OnMoveEnd;
    public event Action OnPathEnd;

    public PathTile currentPath;

    public bool isMoving = false;

    public bool attack = false;
    [NonSerialized] public int size = 10;
    public float health {get; private set;}

    public bool alive = true;
    
    public event Action<Unit>OnDead;

    private Dictionary<string,Ability>abilities;
    private Dictionary<string,ActiveAbility>activeAbilities;

    public SelectFormType attackForm {get; private set;}

    public ActiveAbility activeAbility;

    public Sequence currentTween;

    public bool IsCasting {
        get {
            return activeAbility != null;
        }
    }

    public virtual void Init(int size) {
        stats = BattleManager.instance.assetManager.statsLibrary.Get(statId);
        this.LoadAbilities();
        this.LoadActiveAbilities();

        this.moveRange = stats.moveRange;
        this.attackRange = stats.attackRange;
        this.attackForm = stats.attackForm;
        OnMoveEnd+=NextMove;

        this.SetHealth(stats.health*size);
    }

    public void LoadAbilities() {
        abilities = new Dictionary<string, Ability>();

        foreach (string abilityId in stats.abilities) {
            var newAbility = BattleManager.instance.assetManager.abilityLibrary.Create(abilityId);
            abilities.Add(newAbility.id,newAbility);
        }
    }

    public void LoadActiveAbilities() {
        activeAbilities = new Dictionary<string, ActiveAbility>();

        foreach(string abilityId in stats.activeAbilities) {
            var ability = BattleManager.instance.assetManager.activeAbilityLibrary.Create(abilityId);
            ability.Init(this);
            activeAbilities.Add(ability.id,ability);
        }
    }

    public void SetSide(Side side) {
        if(this.side != null) {
            this.side.RemoveUnit(this);
        }
        this.side = side;
    }

    public void Move(PathTile path) {
        currentPath = path.prev;
        NextMove();
        isMoving = true;
    }

    public void NextMove() {
        if(currentPath == null) {
            OnPathEnd();
            isMoving = false;
            return;
        }
        this.spriteRenderer.flipX = this.tile.x < currentPath.tile.x;
        StartCoroutine(MoveTo(currentPath.tile));
        currentPath = currentPath.prev;
    }

    public IEnumerator MoveTo(Vector3Int tile) {
        float moveProgress = 0;
        Vector3 tilePos = TileManager.instance.TileToWorld(tile);
        Vector3 startPos = gameObject.transform.position;
        while(moveProgress < 0.99) {
            Vector3 pos = Vector3.Lerp(startPos,tilePos,moveProgress);
            gameObject.transform.position = pos;
            yield return new WaitForSeconds(0.01f);
            moveProgress += stats.moveSpeed;
            moveProgress = Mathf.Clamp(moveProgress,0,1.0f);
        }
        this.SetTile(tile,this.tile.x<tile.x);
        moveRange-=1;
        OnMoveEnd();
    }

    public bool IsEnemy(Unit unit) => this.side != unit.side;
    public bool IsEnemy(Side side) => this.side != side;

    public override void Highlight(bool highlight)
    {
        if(highlight == isHighlighted) return;
        this.isHighlighted = highlight;
        if(highlight == false) {
            this.spriteRenderer.material = defaultMaterial;
        }
        else {
            this.spriteRenderer.material = IsEnemy(BattleManager.instance.CurrentRound.currentUnit) ? highlightedMaterial : highlightAllyMaterial;
        }
        if(OnHighlight!=null) OnHighlight();
    }

    public void AttackEnemy(Unit unit) {
        attack = true;
        BattleManager.instance.Battle(this,unit);
        
    }

    public void GetHit(float damage) {
        int prevSize = size;
        ModifyHealth(-damage);

        currentTween = DOTween.Sequence().Append(spriteRenderer.DOColor(Color.red,0)).
        Append(transform.DOShakePosition(1)).
        Append(spriteRenderer.DOColor(Color.white,0));



        foreach(Ability ability in GetAbilities(AbilityType.GetDamage)) {
            ability.Apply(this,prevSize-size);
        }
    }

    public void ModifyHealth(float value) {
        SetHealth(health+value);
    }

    private void SetHealth(float value) {
        health = value;
        if(health<0) {
            health = 0;
            Kill();
        }
        size = (int)Mathf.Ceil(health/stats.health);
        if(OnChange != null) OnChange(this);
    }

    public virtual Range<int> CalculateBaseDamage() {
        return new Range<int>(stats.damage.min*size,stats.damage.max*size);
    }

    public virtual Range<int> CalculateDamage(bool apply = false) {
        Range<int>baseDamage = CalculateBaseDamage();

        float damagePercent = 0;
        foreach(Ability ability in GetAbilities(AbilityType.Attack)) {
            if(apply) damagePercent+= (float)ability.Apply(this);
            else damagePercent+= (float)ability.Show(this);
        }
        return new Range<int>((int)Math.Round(baseDamage.min*(1+damagePercent)),(int)Mathf.Round(baseDamage.max*(1+damagePercent)));
    }

    public virtual int GetUnitAttackRange() {
        return this.stats.attackRange;
    }

    public override void SetTile(Vector3Int tile, bool rightDirection = false)
    {
        base.SetTile(tile, rightDirection);
        ChangeTile();
    }

    public virtual void ChangeTile() {

    }

    public virtual void Rest(bool resetAbilities = true) {
        this.moveRange = this.stats.moveRange;
        this.SetAttackForm(this.stats.attackForm);

        foreach(ActiveAbility activeAbility in activeAbilities.Values) {
            activeAbility.Update();
        } 

        if(resetAbilities) {
            foreach (string abilityId in abilities.Keys.ToList()) {
                if(abilities[abilityId].resetable) {
                    if(abilities[abilityId].Reset() == false) {
                        abilities.Remove(abilityId);
                    }
                }
            }
        }
    }

    public void Kill() {
        if(currentTween != null) currentTween.Kill(true);
        DOTween.Sequence().Join(transform.DOShakeRotation(1)).
        Join(spriteRenderer.material.DOColor(new Color(1,1,1,0),1)).AppendCallback(Destroy);
        alive = false;
    }

    public override void Destroy()
    {
        base.Destroy();
        if(OnDead != null) OnDead(this);
    }

    public float CalculateRecieveDamage(float damage, bool apply) {
        float damageReducer = 0;
        foreach(var ability in GetAbilities(AbilityType.PreDamage)) {
            if(apply == false) damageReducer+=(float)ability.Show(this,damage);
            else damageReducer+=(float)ability.Apply(this,damage);
        }
        damage = damage*(1-damageReducer);
        return damage;
    }

    public bool EndRound() {
        bool nextTurn = true;

        foreach(Ability ability in GetAbilities(AbilityType.EndRound)) {
            nextTurn = nextTurn && (bool)ability.Apply(this);
        }
        return nextTurn;
    }

    public List<Ability>GetAbilities(AbilityType abilityType, bool castable = true) {
        List<Ability>returnAbilities = new List<Ability>();
        foreach(Ability ability in abilities.Values) {
            if(ability.abilityType == abilityType && (ability.CanCast(this) || castable == false)) returnAbilities.Add(ability);
        }
        return returnAbilities;
    }

    public List<ActiveAbility>GetActiveAbilities() {
        return this.activeAbilities.Values.ToList();
    }

    public void UseAbility(string id) {
        abilities[id].Apply(this);
    }

    public void SetAttackForm(SelectFormType selectForm) {
        this.attackForm = selectForm;
        this.OnChange(this);
    }

    public void AddAbility(Ability ability) {
        abilities.Add(ability.id,ability);
    }
    
    public void AddAbility(string id) {
        var ability = BattleManager.instance.assetManager.abilityLibrary.Create(id);
        AddAbility(ability);
    }

    public void RemoveAbility(string id) {
        if(abilities.ContainsKey(id)) abilities.Remove(id);
    }
    
    public void SelectActiveAbility(string id) {
        if(activeAbilities.ContainsKey(id) == false) return;
        DeselectActiveAbility();
        activeAbility = activeAbilities[id];
        activeAbility.Activate();
    }

    public void DeselectActiveAbility() {
        if(activeAbility == null) return;

        activeAbility.Deactivate();
        activeAbility = null;
    }

    public void Cast() {
        if(activeAbility == null) return;
        activeAbility.Cast();
        DeselectActiveAbility();
    }
}
