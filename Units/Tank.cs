using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Unit {

    public override string unitName { get { return "tank"; } }

    // Use this for initialization
    protected override void Start() {
        base.Start();
        movePoints = 5;
        attackRange = 3;
        healthMax = 5;
        healthCurrent = healthMax;
        attack = 5;
    }

}
