using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { Player, Enemy }

public class UnitHandler : MonoBehaviour {

    public Unit tankPrefab;

    private ArrayList units;

    private Dictionary<int, Unit> unitDictionary;

    private Map map;

    // Use this for initialization
    void Awake() {
        units = new ArrayList();
        unitDictionary = new Dictionary<int, Unit>();
        unitDictionary.Add(1, tankPrefab);
    }

    public void SpawnUnits(int[] allyUnits, int[] enemyUnits, int width, int height, Map map) {
        this.map = map;
        for(int i = 0; i < allyUnits.Length; i++) {
            if(allyUnits[i] > 0) {
                Unit newUnit = (Unit)Instantiate(unitDictionary[allyUnits[i]]);
                int x = i % width;
                int z = Mathf.FloorToInt(i / width);
                Tile tile = map.GetTileByPosition(z, x);
                int y = tile.GetY();
                newUnit.Initialize(new Vector3(x - (width / 2), y + 0.70f, (height / 2) - z), new Vector3(x, y, z), Faction.Player, this);
                tile.SetUnit(newUnit);
            }
        }

        for(int i = 0; i < enemyUnits.Length; i++) {
            if(enemyUnits[i] > 0) {
                Unit newUnit = (Unit)Instantiate(unitDictionary[enemyUnits[i]]);
                int x = i % width;
                int z = Mathf.FloorToInt(i / width);
                Tile tile = map.GetTileByPosition(z, x);
                int y = tile.GetY();
                newUnit.Initialize(new Vector3(x - (width / 2), y + 0.7f, (height / 2) - z), new Vector3(x, y, z), Faction.Enemy, this);
                tile.SetUnit(newUnit);
            }
        }
    }

    public void KillUnit(Unit unit) {
        units.Remove(unit);
        map.KillUnit(unit);
    }

}
