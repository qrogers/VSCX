using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionState { None, Move, Attack, Menu }

public class ActionHandler : MonoBehaviour {

    public BattleUI battleui;
    public BattleCursor battleCursor;
    public Map map;

    private Unit selectedUnit;
    private SelectionState state;

    private List<Tile> activeTiles;
    private List<Tile> movePath;

	// Use this for initialization
	void Start () {
        selectedUnit = null;
        state = SelectionState.None;
        activeTiles = new List<Tile>();
        movePath    = new List<Tile>();
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetButtonDown("Click")) {
            switch(state) {
                case SelectionState.None:
                    selectedUnit = battleCursor.GetCurrentTile().GetUnit();
                    if(selectedUnit != null && selectedUnit.GetReady()) {
                        TransitionStateToMove();
                    }
                    break;

                case SelectionState.Move:
                    if(activeTiles.Contains(battleCursor.GetCurrentTile())) {
                        Tile pathTile = battleCursor.GetCurrentTile();
                        
                        while(pathTile.travelPath.travelpath != null) {
                            movePath.Add(pathTile);
                            pathTile = pathTile.travelPath.travelpath;
                        }
                        movePath.Add(map.GetTileByPosition(selectedUnit.GetZ(), selectedUnit.GetX()));

                        map.MoveUnit(selectedUnit, battleCursor.GetCurrentTile(), movePath);
                        TransitionStateToAttack();
                    } else {
                        TransitionStateToNone();
                    }
                    break;

                case SelectionState.Attack:
                    if(activeTiles.Contains(battleCursor.GetCurrentTile()) && battleCursor.GetCurrentTile().GetUnit() != null) {
                        map.AttackUnit(selectedUnit, battleCursor.GetCurrentTile());
                        TransitionStateToNone();
                    } else {
                        TransitionStateToNone();
                    }
                    break;
            }
        }
    }

    private void TransitionStateToNone() {
        ClearColorChanges();
        activeTiles.Clear();
        selectedUnit = null;
        movePath.Clear();
        state = SelectionState.None;
    }

    private void TransitionStateToMove() {
        ClearColorChanges();
        activeTiles.Clear();
        DisplayMove(selectedUnit, activeTiles);
        state = SelectionState.Move;
    }

    private void TransitionStateToAttack() {
        ClearColorChanges();
        activeTiles.Clear();
        DisplayAttack(selectedUnit, activeTiles);
        state = SelectionState.Attack;
    }

    private void ClearColorChanges() {
        foreach(Tile tile in activeTiles) {
            tile.ResetColor();
            tile.travelPath = new TravelPath();
        }
    }

    private void DisplayMove(Unit unit, List<Tile> moveTiles) {
        Tile startTile = map.GetTileByPosition(unit.GetZ(), unit.GetX());
        int mp = startTile.GetMovePoints();
        new MoveGhost(unit.GetMovePoints() + mp, map, unit.GetX(), unit.GetZ(), moveTiles, startTile);
        startTile.travelPath.travelpath = null;
        moveTiles.Add(battleCursor.GetCurrentTile());
        foreach(Tile tile in activeTiles) {
            tile.SetColor(new Color(0.4f, 0.0f, 0.4f));
        }
    }

    private void DisplayAttack(Unit unit, List<Tile> moveTiles) {
        new AttackGhost(unit.GetAttackRange() + 1, map, unit.GetX(), unit.GetZ(), moveTiles);
        moveTiles.RemoveAll(tile => tile == battleCursor.GetCurrentTile());
        foreach(Tile tile in activeTiles) {
            tile.SetColor(new Color(0.9f, 0.1f, 0.1f));
        }
        battleCursor.GetCurrentTile().ResetColor();
    }

    public SelectionState GetState() {
        return state;
    }

}

public class MoveGhost {

    public MoveGhost(int movePoints, Map map, int x, int z, List<Tile> tiles, Tile pTile) {
        Tile tile = map.GetTileByPosition(z, x);
        if(tile != null) {
            int mp = movePoints - tile.GetMovePoints() - (tile.GetY() - pTile.GetY());
            if(mp >= tile.travelPath.count) {
                tile.travelPath.count = mp;
                tile.travelPath.travelpath = pTile;
                if(mp >= 0 && tile.GetUnit() == null && !tiles.Contains(tile)) {
                    tiles.Add(tile);
                }
                if(mp > 0) {
                    new MoveGhost(mp, map, x + 1, z, tiles, tile);
                    new MoveGhost(mp, map, x - 1, z, tiles, tile);
                    new MoveGhost(mp, map, x, z + 1, tiles, tile);
                    new MoveGhost(mp, map, x, z - 1, tiles, tile);
                }
            }
        }
    }

}

public class AttackGhost {

    public AttackGhost(int attackRange, Map map, int x, int z, List<Tile> tiles) {
        Tile tile = map.GetTileByPosition(z, x);
        if(tile != null) {
            int ar = attackRange - 1;
            if(ar >= 0) {
                tiles.Add(tile);
            }
            if(ar > 0) {
                new AttackGhost(ar, map, x + 1, z, tiles);
                new AttackGhost(ar, map, x - 1, z, tiles);
                new AttackGhost(ar, map, x, z + 1, tiles);
                new AttackGhost(ar, map, x, z - 1, tiles);
            }
        }
    }

}