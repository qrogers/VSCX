using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHandler : MonoBehaviour {

    public AIController aiController;
    public UnitHandler unitHandler;

    private Faction currentTurn;

	// Use this for initialization
	void Start () {
        currentTurn = Faction.Player;
	}
	
    public void CheckTurnEnd() {
        List<Unit> currentTurnUnits;
        switch(currentTurn) {
            case Faction.Player:
                currentTurnUnits = unitHandler.GetPlayerUnitsCopy();
                break;
            case Faction.Enemy:
                currentTurnUnits = unitHandler.GetEnemyUnitsCopy();
                break;
            default:
                return;
        }

        foreach(Unit unit in currentTurnUnits) {
            if(unit.GetReady()) {
                return;
            }
        }

        EndTurn();

    }

    public void ForceEndTurn() {
        EndTurn();
    }

    private void EndTurn() {
        switch(currentTurn) {
            case Faction.Player:
                currentTurn = Faction.Enemy;
                foreach(Unit unit in unitHandler.GetPlayerUnitsCopy()) {
                    unit.Ready();
                }
                foreach(Unit unit in unitHandler.GetEnemyUnitsCopy()) {
                    aiController.UnitAction(unit);
                }
                EndTurn();
                break;

            case Faction.Enemy:
                currentTurn = Faction.Player;
                foreach(Unit unit in unitHandler.GetEnemyUnitsCopy()) {
                    unit.Ready();
                }
                break;

            default:
                return;
        }
    }

}
