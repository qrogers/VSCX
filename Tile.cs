using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum Slant { None, North, South, East, West }

public struct TravelPath {
    public Tile travelpath;
    public int count;
}

public class Tile : MapObject {

    public Material base_material;

    private Color color;
    private int movePoints;
    private int defenseMod;

    public TravelPath travelPath;

    //public int moveRecord;

    private Unit unit;

    private Material material;

    //private Slant slant;

    public void RecieveData(TileData data, Vector3 position, Vector3 coordinates) {
        this.position = position;
        this.coordinates = coordinates;
        color = new Color();
        ColorUtility.TryParseHtmlString(data.color, out color);
        movePoints = data.movepoints;
        defenseMod = data.defensemod;
        //this.slant = slant;
    }

    // Use this for initialization
    protected override void Start() {
        base.Start();
        material = (Material)Instantiate(base_material);
        material.color = color;
        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        materials[0] = material;
        gameObject.GetComponent<MeshRenderer>().materials = materials;
        //travelPath = new TravelPath();
        //switch(slant) {
        //    case Slant.North:
        //        transform.Rotate(new Vector3(0.0f, 0.0f, 0.0f));
        //        break;
        //    case Slant.South:
        //        transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        //        break;
        //    case Slant.East:
        //        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
        //        break;
        //    case Slant.West:
        //        transform.Rotate(new Vector3(0.0f, 270.0f, 0.0f));
        //        break;
        //    case Slant.None:
        //        break;
        //}
    }

    void Awake() {
        unit = null;
    }

    public void SetColor(Color color) {
        material.color = color;
        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        materials[0] = material;
        gameObject.GetComponent<MeshRenderer>().materials = materials;
    }

    public void ResetColor() {
        material.color = color;
        Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
        materials[0] = material;
        gameObject.GetComponent<MeshRenderer>().materials = materials;
    }

    public void ClearTravel() {
        travelPath.count = 0;
        travelPath.travelpath = null;
    }

    public int GetMovePoints() {
        return movePoints;
    }

    public int GetDefenseMod() {
        return defenseMod;
    }

    public void SetUnit(Unit unit) {
        this.unit = unit;
    }

    public Unit GetUnit() {
        return unit;
    }

}
