using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tank : Unit {

    public override string unitName { get { return "tank"; } }

    // Use this for initialization
    protected override void Start() {
        base.Start();
        movePoints = 4;
        attackRange = 1;
        healthMax = 10;
        healthCurrent = healthMax;
        attack = 2;
        defense = 1;

        attacks.Add("Basic Attack", Attack);
        abilities.Add("Attack", AttackMenu);
        abilities.Add("Power Up", PowerUp);
        abilities.Add("Wait", Wait);
    }

    private void PowerUp(Tile tile, Map map, ActionHandler actionHandler) {
        attack += 1;
        ready = false;
        actionHandler.UnitWait(this);
    }

}
