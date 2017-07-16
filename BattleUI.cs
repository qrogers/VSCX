using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public MenuController menuController;

    //public Texture borderTexture;

    //public Material borderMaterial;

    //private string text;

    void OnGUI() {
        menuController.RenderMenus();
    }

    //public void DisplayUnitInfo(Unit unit) {
    //    if(unit == null) {
    //        text = "Null";
    //    } else {
    //        text = unit.unitName + "\n" + unit.GetHealthCurrent().ToString();
    //    }
    //}
}
