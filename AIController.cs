using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public Map map;

	// Use this for initialization
	void Start () {
		
	}

    public void UnitAction(Unit unit) {
        List<Tile> moves = GetMoves(unit);
        map.MoveUnit(unit, )
    }

    private List<Tile> GetMoves(Unit unit) {
        List<Tile> moves = new List<Tile>();
        Tile startTile = map.GetTileByPosition(unit.GetZ(), unit.GetX());
        int mp = startTile.GetMovePoints();
        new MoveGhost(unit.GetMovePoints() + mp, map, unit.GetX(), unit.GetZ(), moves, startTile);
        return moves;
    }

}
