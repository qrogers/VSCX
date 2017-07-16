using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionState { None, Move, Attack, Menu }

public class ActionHandler : MonoBehaviour {

    //public BattleUI battleui;
    public BattleCursor battleCursor;
    public Map map;
    public TurnHandler turnHandler;
    public AIController aiController;
    public UnitHandler unitHandler;
    public MenuController menuController;

    private Unit selectedUnit;
    private SelectionState state;

    private Tile oldLocation;

    private List<Tile> activeTiles;
    private List<Tile> movePath;

    private int inputDelay;

    // Use this for initialization
    void Start () {
        selectedUnit = null;
        state = SelectionState.None;
        activeTiles = new List<Tile>();
        movePath    = new List<Tile>();
        inputDelay = 0;
        oldLocation = null;
    }

    void Update() {
        inputDelay -= 1;
        if(state == SelectionState.Menu) {
            if(Input.GetButtonDown("Up")) {
                menuController.MoveMenu(false);
            } else if(Input.GetButtonDown("Down")) {
                menuController.MoveMenu(true);
            }
        }
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
                        oldLocation = map.GetTileByPosition(selectedUnit.GetZ(), selectedUnit.GetX());
                        map.MoveUnit(selectedUnit, battleCursor.GetCurrentTile(), movePath);
                        CreateMenu("Select Action");
                        TransitionStateToMenu();
                    } else {
                        TransitionStateToNone();
                    }
                    break;

                case SelectionState.Attack:
                    if(activeTiles.Contains(battleCursor.GetCurrentTile()) && battleCursor.GetCurrentTile().GetUnit() != null) {
                        selectedUnit.GetEquippedAttack()(battleCursor.GetCurrentTile(), map, this);
                        menuController.DeleteMenu();
                        TransitionStateToNone();
                    } else {
                        //TransitionStateToNone();
                    }
                    break;

                case SelectionState.Menu:
                    menuController.ClickMenu();
                    break;
            }

        } else if(Input.GetButtonDown("Unclick")) {
            switch(state) {
                case SelectionState.None:
                    break;

                case SelectionState.Move:
                    TransitionStateToNone();
                    break;

                case SelectionState.Attack:
                    map.MoveUnit(selectedUnit, oldLocation, movePath);
                    selectedUnit.Ready();
                    TransitionStateToMove();
                    break;

                case SelectionState.Menu:
                    menuController.DeleteMenu();
                    state = SelectionState.Attack;
                    break;
            }
        } else if(Input.GetButton("Next") && inputDelay <= 0) {
            inputDelay = 15;
            Unit nextUnit = null;
            Unit currentUnit = battleCursor.GetCurrentTile().GetUnit();
            List<Unit> units = unitHandler.GetPlayerUnitsCopy();
            if(units.Count > 0) {
                if(currentUnit == null) {
                    foreach(Unit unit in units) {
                        if(unit.GetReady()) {
                            nextUnit = unit;
                            break;
                        }
                    }
                } else {
                    int nextIndex = units.IndexOf(currentUnit) + 1;
                    if(nextIndex >= units.Count) {
                        nextIndex = 0;
                    }
                    int i = units.Count;
                    while(!units[nextIndex].GetReady() && i > 0) {
                        i--;
                        nextIndex++;
                        if(nextIndex >= units.Count) {
                            nextIndex = 0;
                        }
                    }
                    if(nextIndex >= units.Count) {
                        nextIndex = 0;
                    }
                    if(i == 0) {
                        nextUnit = null;
                    } else {
                        nextUnit = units[nextIndex];
                    }
                }
            }
            if(nextUnit != null) {
                battleCursor.MoveToTile(map.GetTileByPosition(nextUnit.GetZ(), nextUnit.GetX()));
            }
        } else if(Input.GetButtonDown("End") && inputDelay <= 0) {
            turnHandler.ForceEndTurn();
        }
    }

    public void CreateMenu(string name) {
        Dictionary<string, Unit.Ability>.KeyCollection optionList = selectedUnit.GetAbilities();
        List<Option> options = new List<Option>();
        Option option;
        foreach(string ability in optionList) {
            option = new Option();
            option.name = ability;
            option.optionAction = ProcessMenuClick;
            options.Add(option);
        }
        menuController.CreateMenu(name, options, new Vector2(50 * Random.Range(1, 5), 50));
        //TransitionStateToMenu();
    }

    private void ProcessMenuClick(string action) {
        selectedUnit.DoAbility(action, battleCursor.GetCurrentTile(), map, this);
        //menuController.DeleteMenu();
        //TransitionStateToNone();
    }

    public void UnitWait(Unit unit) {
        menuController.DeleteMenu();
        TransitionStateToNone();
    }

    public void UnitAttack(Unit unit) {
        TransitionStateToAttack();
    }

    public bool MoveCursor() {
        return state != SelectionState.Menu;
    }

    private void TransitionStateToNone() {
        ClearColorChanges();
        activeTiles.Clear();
        selectedUnit = null;
        movePath.Clear();
        turnHandler.CheckTurnEnd();
        state = SelectionState.None;
    }

    private void TransitionStateToMenu() {
        ClearColorChanges();
        activeTiles.Clear();
        state = SelectionState.Menu;
    }

    private void TransitionStateToMove() {
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
            tile.ClearTravel();
        }
    }

    private void DisplayMove(Unit unit, List<Tile> moveTiles) {
        ClearColorChanges();
        activeTiles.Clear();
        movePath.Clear();
        aiController.GetUnitMoves(unit, moveTiles);
        foreach(Tile tile in moveTiles) {
            tile.SetColor(new Color(0.4f, 0.0f, 0.4f));
        }
    }

    private void DisplayAttack(Unit unit, List<Tile> moveTiles) {
        new AttackGhost(unit.GetAttackRange() + 1, map, unit.GetX(), unit.GetZ(), moveTiles);
        moveTiles.RemoveAll(tile => tile == battleCursor.GetCurrentTile());
        foreach(Tile tile in moveTiles) {
            tile.SetColor(new Color(0.9f, 0.1f, 0.1f));
        }
        battleCursor.GetCurrentTile().ResetColor();
    }

    public SelectionState GetState() {
        return state;
    }

}

public class MoveGhost {

    public MoveGhost(int movePoints, Map map, int x, int z, List<Tile> tiles, Tile parentTile, Faction faction) {
        Tile tile = map.GetTileByPosition(z, x);
        if(tile != null) {
            int mp = movePoints - tile.GetMovePoints() - (tile.GetY() - parentTile.GetY());
            if(mp >= tile.travelPath.count && (tile.GetUnit() == null || tile.GetUnit().GetFaction() == faction)) {
                tile.travelPath.count = mp;
                tile.travelPath.travelpath = parentTile;
                if(mp >= 0 && tile.GetUnit() == null && !tiles.Contains(tile)) {
                    tiles.Add(tile);
                }
                if(mp > 0) {
                    new MoveGhost(mp, map, x + 1, z, tiles, tile, faction);
                    new MoveGhost(mp, map, x - 1, z, tiles, tile, faction);
                    new MoveGhost(mp, map, x, z + 1, tiles, tile, faction);
                    new MoveGhost(mp, map, x, z - 1, tiles, tile, faction);
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