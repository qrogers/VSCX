using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public Map map;
    public UnitHandler unitHandler;

    public void UnitAction(Unit unit) {
        List<Tile> moves = new List<Tile>();
        GetUnitMoves(unit, moves);
        List<Tile> movePath = new List<Tile>();
        List<Unit> playerUnits = unitHandler.GetPlayerUnitsCopy();
        if(playerUnits.Count > 0) {
            if(moves.Count > 0) {
                Unit nearestUnit = GetNearestUnit(unit);
                Tile destination = GetNearestTile(map.GetTileByPosition(nearestUnit.GetZ(), nearestUnit.GetX()), moves);
                Tile pathTile = destination;
                while(pathTile.travelPath.travelpath != null) {
                    movePath.Add(pathTile);
                    pathTile = pathTile.travelPath.travelpath;
                }
                movePath.Add(map.GetTileByPosition(unit.GetZ(), unit.GetX()));
                map.MoveUnit(unit, destination, movePath);
                foreach(Tile tile in moves) {
                    tile.ClearTravel();
                }
            }
            List<Tile> attacks = GetAttacks(unit);
            if(attacks.Count > 0) {
                foreach(Tile tile in attacks) {
                    if(tile.GetUnit() != null) {
                        map.AttackUnit(unit, tile);
                        break;
                    }
                }
            }
        }

    }

    private Unit GetNearestUnit(Unit unit) {
        Unit nearestUnit = null;
        List<Unit> allPlayerUnits = unitHandler.GetPlayerUnitsCopy();
        float smallestDistance = float.MaxValue;
        foreach(Unit playerUnit in allPlayerUnits) {
            float newDistance = Vector3.Distance(unit.transform.position, playerUnit.transform.position);
            if(newDistance < smallestDistance) {
                smallestDistance = newDistance;
                nearestUnit = playerUnit;
            }
        }
        return nearestUnit;
    }

    private Tile GetNearestTile(Tile tile, List<Tile> tileSet) {
        Tile nearestTile = null;
        float smallestDistance = float.MaxValue;
        foreach(Tile setTile in tileSet) {
            float newDistance = Vector3.Distance(tile.transform.position, setTile.transform.position);
            if(newDistance < smallestDistance) {
                smallestDistance = newDistance;
                nearestTile = setTile;
            }
        }
        return nearestTile;

    }

    public void GetUnitMoves(Unit unit, List<Tile> moves) {
        Tile startTile = map.GetTileByPosition(unit.GetZ(), unit.GetX());
        int mp = startTile.GetMovePoints();
        new MoveGhost(unit.GetMovePoints() + mp, map, unit.GetX(), unit.GetZ(), moves, startTile, unit.GetFaction());
        startTile.travelPath.travelpath = null;
        moves.Add(startTile);
    }

    private List<Tile> GetAttacks(Unit unit) {
        List<Tile> attacks = new List<Tile>();
        new AttackGhost(unit.GetAttackRange() + 1, map, unit.GetX(), unit.GetZ(), attacks);
        attacks.RemoveAll(tile => tile.GetUnit() == null);
        attacks.RemoveAll(tile => tile.GetUnit().GetFaction() == unit.GetFaction());
        return attacks;
    }

}
