using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MapObject {

    public ParticleSystem attackEffect;

    public virtual string unitName { get { return "unit"; } }

    private Tile currentTile;
    private Faction faction;

    protected int movePoints;
    protected int attackRange;
    protected int healthMax;
    protected int healthCurrent;
    protected int attack;
    protected bool ready;

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
        //ready = false;
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

    public void Attack() {
        attackEffect.Play();
    }

    public void RecieveAttack(int damage) {
        healthCurrent -= damage;
        ParticleSystem fx = (ParticleSystem)Instantiate(attackEffect);
        fx.transform.position = transform.position;
        fx.transform.Rotate(new Vector3(-90.0f, 0.0f, 0.0f));
        fx.Play();
        if(healthCurrent <= 0) {
            Kill();
        }
    }

    private void Kill() {
        unitHandler.KillUnit(this);
        Destroy(gameObject);
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

    public bool GetReady() {
        return ready;
    }

    public int GetHealthCurrent() {
        return healthCurrent;
    }

    public Faction GetFaction() {
        return faction;
    }

}
