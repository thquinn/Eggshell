using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public GameObject prefabNextRoom;
    public DoorScript door;
    public PanelScript panelFloor, panelCeiling;
    public bool usesPanelFloor, usesPanelCeiling;

    public GameObject[] reflectedObjects;
    public Material materialReflection;

    [HideInInspector] public RoomScript prevRoomScript, nextRoomScript;
    bool occupied;

    void Start() {
        Transform reflection = new GameObject("Reflection").transform;
        reflection.localPosition = transform.localPosition;
        foreach (GameObject g in reflectedObjects) {
            GameObject r = Instantiate(g, reflection);
            Destroy(r.GetComponent<Collider>());
            r.transform.position = g.transform.position;
            r.GetComponent<MeshRenderer>().sharedMaterial = materialReflection;
        }
        reflection.localScale = new Vector3(1, 1, -1);
        reflection.parent = transform;
        Invoke("OpenPanels", .5f);
    }
    public void OpenPanels() {
        if (usesPanelFloor) {
            panelFloor.Open();
        }
        if (usesPanelCeiling) {
            panelCeiling.Open();
        }
    }
    public void OpenDoor() {
        if (nextRoomScript == null) {
            Destroy(prevRoomScript?.prevRoomScript?.gameObject);
            nextRoomScript = Instantiate(prefabNextRoom).GetComponent<RoomScript>();
            nextRoomScript.prevRoomScript = this;
            nextRoomScript.transform.position = transform.position + new Vector3(8, 0, 0);
        }
        door.Open();
    }
    public void CloseDoor() {
        door.Close();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            panelFloor.Open();
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            panelCeiling.Open();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && occupied) {
            OpenDoor();
        }
    }

    private void OnTriggerEnter(Collider other) {
        occupied = true;
    }
    private void OnTriggerExit(Collider other) {
        occupied = false;
        door.Close();
    }
}
