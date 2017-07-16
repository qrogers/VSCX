using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCursor : MonoBehaviour {

    public GameObject mapObject;
    public Camera battleCamera;
    public BattleUI battleUI;
    public ActionHandler actionHandler;
    public AudioSource moveAudio;

    private Tile currentTile;
    private Map map;

    private int inputDelay = 0;
    private int scaleCounter = 0;
    private int scaleDirection = 1;
    private float scaleMultiplier = 1.001f;

    // Use this for initialization
    void Start() {
        map = mapObject.GetComponent<Map>();
        currentTile = map.GetTileByPosition(5, 5);
    }
	
	// Update is called once per frame
	void Update () {
        inputDelay -= 1;
        if(actionHandler.MoveCursor() && inputDelay <= 0) {
            inputDelay = 6;
            Vector3 cameraLocation = battleCamera.transform.position;
            try {
                moveAudio.Play();
                if(Input.GetButton("Left")) {
                    currentTile = map.NextTile(currentTile, Direction.Left);
                    battleCamera.transform.position = new Vector3(cameraLocation.x - 1, cameraLocation.y, cameraLocation.z);
                } else if(Input.GetButton("Right")) {
                    currentTile = map.NextTile(currentTile, Direction.Right);
                    battleCamera.transform.position = new Vector3(cameraLocation.x + 1, cameraLocation.y, cameraLocation.z);
                } else if(Input.GetButton("Up")) {
                    currentTile = map.NextTile(currentTile, Direction.Up);
                    battleCamera.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, cameraLocation.z + 1);
                } else if(Input.GetButton("Down")) {
                    currentTile = map.NextTile(currentTile, Direction.Down);
                    battleCamera.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, cameraLocation.z - 1);
                }
                Vector3 tilePosition = currentTile.GetComponent<Transform>().position;
                transform.position = new Vector3(tilePosition.x, tilePosition.y + 2, tilePosition.z);
                //battleUI.DisplayUnitInfo(currentTile.GetUnit());
            } catch(OffMapException) { }

        }
        
        scaleCounter += scaleDirection;
        if(scaleCounter >= 180) {
            scaleDirection = -1;
            scaleMultiplier = 0.999f;
        } else if(scaleCounter <= 0) {
            scaleDirection = 1;
            scaleMultiplier = 1.001f;
        }

        GetComponentsInChildren<Transform>()[1].Rotate(new Vector3(0.0f, 0.0f, 0.3f));
        Vector3 scale = GetComponentsInChildren<Transform>()[2].localScale;
        GetComponentsInChildren<Transform>()[2].localScale = new Vector3(scale.x * scaleMultiplier, scale.y * scaleMultiplier, scale.z);
    }

    public void MoveToTile(Tile tile) {
        currentTile = tile;
        Vector3 tilePosition = currentTile.GetComponent<Transform>().position;
        Vector3 movement = tilePosition - transform.position;
        transform.position = new Vector3(tilePosition.x, tilePosition.y + 2, tilePosition.z);
        //Vector3 cameraLocation = battleCamera.transform.position;
        battleCamera.transform.position = new Vector3(battleCamera.transform.position.x + movement.x, battleCamera.transform.position.y, battleCamera.transform.position.z + movement.z);
        //battleUI.DisplayUnitInfo(currentTile.GetUnit());
    }

    public Tile GetCurrentTile() {
        return currentTile;
    }

}
