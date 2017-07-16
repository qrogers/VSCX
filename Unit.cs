using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject {

    public delegate void Ability(Tile tile, Map map, ActionHandler actionHandler);

    public ParticleSystem attackEffect;
    public Texture healthTexture;

    public virtual string unitName { get { return "unit"; } }

    //private Tile currentTile;
    private Faction faction;

    protected int movePoints;
    protected int attackRange;
    protected int healthMax;
    protected int healthCurrent;
    protected int attack;
    protected int defense;
    protected bool ready;

    private Ability equippedAttack;

    protected Dictionary<string, Ability> abilities;
    protected Dictionary<string, Ability> attacks;

    private Tile tile;
    private Map map;
    private ActionHandler actionHandler;

    //private ActionHandler actionHandler;

    private bool moving;
    private List<Vector3> movePath;
    private Vector3 targetVector;

    private UnitHandler unitHandler;

    public void Initialize(Vector3 position, Vector3 coordinates, Faction faction, UnitHandler unitHandler) {
        this.position = position;
        this.coordinates = coordinates;
        this.faction = faction;
        this.unitHandler = unitHandler;
        ready = true;
        moving = false;
        transform.position = position;
        targetVector = transform.position;

        abilities = new Dictionary<string, Ability>();
        attacks = new Dictionary<string, Ability>();

        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        switch(this.faction) {
            case Faction.Player:
                materials[0].color = new Color(0.0f, 0.0f, 1.0f);
                gameObject.GetComponent<MeshRenderer>().materials = materials;
                break;
            case Faction.Enemy:
                materials[0].color = new Color(1.0f, 0.0f, 0.0f);
                gameObject.GetComponent<MeshRenderer>().materials = materials;
                break;
            default:
                return;
        }
    }

    void OnGUI() {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        screenPosition.y = Screen.height - screenPosition.y;
        GUI.color = new Color(1.5f - healthCurrent / (float)healthMax, healthCurrent / (float)healthMax - 0.0f, 0.2f, 0.8f);
        GUI.DrawTexture(new Rect(screenPosition.x - 20, screenPosition.y + 25, 40.0f * (healthCurrent / (float)healthMax), 5), healthTexture);
    }

    void Update() {
        if(moving) {
            if(targetVector == transform.position) {
                if(movePath.Count > 0) {
                    Vector3 targetPosition = movePath[0];
                    targetVector = new Vector3(targetPosition.x, targetPosition.y + 0.7f, targetPosition.z);
                    movePath.RemoveAt(0);
                } else {
                    moving = false;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, targetVector, 0.2f);
        }
    }

    public void Move(Vector3 position, Vector3 coordinates, List<Tile> movePath) {
        this.position = position;
        this.coordinates = coordinates;
        moving = true;
        ready = false;
        movePath.Reverse();
        this.movePath = new List<Vector3>();
        Vector3 previousVector = new Vector3(position.x, position.y, position.z);
        foreach(Tile tile in movePath) {
            if(previousVector.y > tile.transform.position.y) {
                this.movePath.Add(new Vector3(tile.transform.position.x, previousVector.y, tile.transform.position.z));
                this.movePath.Add(new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z));
            } else if(previousVector.y < tile.transform.position.y) {
                this.movePath.Add(new Vector3(previousVector.x, tile.transform.position.y, previousVector.z));
                this.movePath.Add(new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z));
            } else {
                this.movePath.Add(tile.transform.position);
            }
            previousVector = tile.transform.position;
        }
        this.movePath.RemoveAt(0);
    }

    protected void Attack(Tile tile, Map map, ActionHandler actionHandler) {
        map.AttackUnit(this, tile);
        //attackEffect.Play();
        //actionHandler.UnitAttack(this);
        //actionHandler.menuController.DeleteMenu();
    }

    protected void Wait(Tile tile, Map map, ActionHandler actionHandler) {
        ready = false;
        actionHandler.UnitWait(this);
    }

    protected void AttackMenu(Tile tile, Map map, ActionHandler actionHandler) {
        List<Option> options = new List<Option>();
        Option option;
        foreach(string ability in attacks.Keys) {
            option = new Option();
            option.name = ability;
            option.optionAction = ProcessMenuClick;
            options.Add(option);
        }
        actionHandler.menuController.CreateMenu(name, options, new Vector2(50 * Random.Range(1, 5), 50));
        this.tile = tile;
        this.map = map;
        this.actionHandler = actionHandler;
    }

    private void ProcessMenuClick(string action) {
        equippedAttack = attacks[action];
        //Attack(tile, map, actionHandler);
        actionHandler.UnitAttack(this);
        actionHandler.menuController.DeleteMenu();
        tile = null;
        map = null;
        actionHandler = null;
    }

    public void RecieveAttack(int damage) {
        healthCurrent -= damage;
        ParticleSystem fx = (ParticleSystem)Instantiate(attackEffect);
        fx.transform.position = transform.position;
        fx.transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        fx.Play();
        if(healthCurrent <= 0) {
            Kill();
        }
    }

    private void Kill() {
        unitHandler.KillUnit(this);
        Destroy(gameObject);
    }

    public Dictionary<string, Ability>.KeyCollection GetAbilities() {
        return abilities.Keys;
    }

    //public Dictionary<string, Ability>.KeyCollection GetAttacks() {
    //    return attacks.Keys;
    //}

    public void DoAbility(string ability, Tile tile, Map map, ActionHandler actionHandler) {
        abilities[ability](tile, map, actionHandler);
    }

    public void DoAttack(string attack, Tile tile, Map map, ActionHandler actionHandler) {
        attacks[attack](tile, map, actionHandler);
    }

    public Ability GetEquippedAttack() {
        return equippedAttack;
    }

    public int GetMovePoints() {
        return movePoints;
    }

    public int GetAttack() {
        return attack;
    }

    public int GetAttackRange() {
        return attackRange;
    }

    public int GetDefense() {
        return defense;
    }

    public bool GetReady() {
        return ready;
    }

    public void Ready() {
        ready = true;
    }

    public int GetHealthCurrent() {
        return healthCurrent;
    }

    public Faction GetFaction() {
        return faction;
    }

}
