using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayUnitInfo(Unit unit) {
        if(unit == null) {
            text.text = "Null";
        } else {
            text.text = unit.unitName + "\n" + unit.GetHealthCurrent().ToString();
        }
    }
}
