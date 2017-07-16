using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct MapData {
    public int width;
    public int height;
    public int[] tiles;
    public int[] elevation;
    public int[] ally_units;
    public int[] enemy_units;
}

public struct TileTable {
    public Dictionary<int, TileData> tiledata;
    public TileData plain;
    public TileData hill;
    public TileData fort;
}

public struct TileData {
    public string color;
    public int defensemod;
    public int movepoints;
}

public enum Direction { Left, Right, Up, Down };

public class OffMapException : UnityException {}

public class Map : MonoBehaviour {

    //public TurnHandler turnHandler;
    public Transform unitHandler;

    //Tile Prefab
    public Tile basetile;
    //public Tile baseslant;

    private TileTable tiletable;

    private Tile[,] map;

    private float xPositonOffset;
    private float zPositonOffset;

    // Use this for initialization
    void Awake() {
        BuildTileData();

        string rawmapdata = System.IO.File.ReadAllText("Assets/mapdata/map0.json");
        MapData mapdata = JsonUtility.FromJson<MapData>(rawmapdata);
        map = new Tile[mapdata.width, mapdata.height];
        for (int i = 0; i < mapdata.tiles.Length; i++) {
            //Slant slant;
            //switch(mapdata.slants[i]) {
            //    case 7:
            //        slant = Slant.North;
            //        break;
            //    case 2:
            //        slant = Slant.South;
            //        break;
            //    case 9:
            //        slant = Slant.East;
            //        break;
            //    case 4:
            //        slant = Slant.West;
            //        break;
            //    case 0:
            //        slant = Slant.None;
            //        break;
            //    default:
            //        slant = Slant.None;
            //        break;
            //}
            Tile newTile = (Tile)Instantiate(basetile);
            //if(slant == Slant.None) {
            //    newTile = (Tile)Instantiate(basetile);
            //} else {
            //    newTile = (Tile)Instantiate(baseslant);
            //}
            TileData tiletype = tiletable.tiledata[mapdata.tiles[i]];
            xPositonOffset = mapdata.width / 2;
            zPositonOffset = mapdata.height / 2;
            int x = i % mapdata.width;
            int y = mapdata.elevation[i];
            int z = Mathf.FloorToInt(i / mapdata.width);
            newTile.RecieveData(tiletype, CoordinatesToPosition(new Vector3(x, y, z)), new Vector3(x, y, z));
            map[z, x] = newTile;
        }
    }

    private Vector3 CoordinatesToPosition(Vector3 position) {
        return new Vector3(position.x - xPositonOffset, position.y, zPositonOffset - position.z);
    }

    void Start() {
        string rawmapdata = System.IO.File.ReadAllText("Assets/mapdata/map0.json");
        MapData mapdata = JsonUtility.FromJson<MapData>(rawmapdata);
        unitHandler.GetComponent<UnitHandler>().SpawnUnits(mapdata.ally_units, mapdata.enemy_units, mapdata.width, mapdata.height, this);
    }

    public void MoveUnit(Unit unit, Tile tile, List<Tile> movePath) {
        int x = tile.GetX();
        int y = tile.GetY();
        int z = tile.GetZ();
        GetTileByPosition(unit.GetZ(), unit.GetX()).SetUnit(null);
        tile.SetUnit(unit);
        unit.Move(CoordinatesToPosition(new Vector3(x, y + 1, z)), new Vector3(x, y, z), movePath);
    }

    public void AttackUnit(Unit unit, Tile tile) {
        Unit defender = tile.GetUnit();
        if(defender.GetFaction() != unit.GetFaction()) {
            tile.GetUnit().RecieveAttack(unit.GetAttack() - defender.GetDefense() - tile.GetDefenseMod());
        }
    }

    public void KillUnit(Unit unit) {
        GetTileByPosition(unit.GetZ(), unit.GetX()).SetUnit(null);
    }

    private void BuildTileData() {
        tiletable.tiledata = new Dictionary<int, TileData>();

        tiletable.plain.color = "#111111";
        tiletable.plain.defensemod = 0;
        tiletable.plain.movepoints = 1;
        tiletable.tiledata.Add(0, tiletable.plain);

        tiletable.hill.color = "#333333";
        tiletable.hill.defensemod = 0;
        tiletable.hill.movepoints = 2;
        tiletable.tiledata.Add(1, tiletable.hill);

        tiletable.fort.color = "#555555";
        tiletable.fort.defensemod = 1;
        tiletable.fort.movepoints = 3;
        tiletable.tiledata.Add(2, tiletable.fort);

        tiletable.fort.color = "#5555CC";
        tiletable.fort.defensemod = 1;
        tiletable.fort.movepoints = 10;
        tiletable.tiledata.Add(3, tiletable.fort);
    }

    public Tile NextTile(Tile tile, Direction direction) {
        int x = tile.GetX();
        int z = tile.GetZ();
        if(direction == Direction.Left) {
            if(x - 1 < 0) {
                throw new OffMapException();
            } else {
                return map[z, x - 1];
            }
        } else if(direction == Direction.Right) {
            if(x + 1 >= map.GetLength(0)) {
                throw new OffMapException();
            } else {
                return map[z, x + 1];
            }
        } else if(direction == Direction.Up) {
            if(z - 1 < 0) {
                throw new OffMapException();
            } else {
                return map[z - 1, x];
            }
        } else if(direction == Direction.Down) {
            if(z + 1 >= map.GetLength(0)) {
                throw new OffMapException();
            } else {
                return map[z + 1, x];
            }
        } else {
            return tile;
        }
        
    }

    public Tile GetTileByPosition(int z, int x) {
        try {
            return map[z, x];
        } catch(System.IndexOutOfRangeException) {
            return null;
        }
    }

}
