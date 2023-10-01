using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript_End : MonoBehaviour
{
    static float ELEVATOR_RIDE_LENGTH = 20;

    public FootprintTriggerScript trigger;
    public GameObject[] elevatorParts;
    public Transform[] ceilingPivots;
    public GameObject diningParent, clocheMock;

    Vector3 elevatorTargetPosition;
    EndPhase phase;
    Vector3 vElevator, vCentering;
    float vCeilingPivotTheta;
    bool swapped;

    void Start() {
        elevatorTargetPosition = trigger.transform.localPosition + new Vector3(0, 7.5f, 0);
    }

    void Update() {
        if (phase == EndPhase.WaitingForTrigger) {
            UpdateWaitingForTrigger();
        } else if (phase == EndPhase.Elevator) {
            UpdateElevator();
        }
    }

    void UpdateWaitingForTrigger() {
        if (trigger.triggered) {
            PlayerScript.instance.rb.isKinematic = true;
            foreach (GameObject go in elevatorParts) {
                go.transform.parent = trigger.transform;
            }
            phase = EndPhase.Elevator;
        }
    }
    void UpdateElevator() {
        Transform playerTransform = PlayerScript.instance.transform;
        trigger.transform.localPosition = Vector3.SmoothDamp(trigger.transform.localPosition, elevatorTargetPosition, ref vElevator, ELEVATOR_RIDE_LENGTH);
        playerTransform.position = Vector3.SmoothDamp(playerTransform.position, trigger.transform.position, ref vCentering, .2f);
        if (!swapped && playerTransform.position.y > 4.25f) {
            diningParent.SetActive(true);
            clocheMock.SetActive(false);
            swapped = true;
        }
        float ceilingPivotTheta = Mathf.SmoothDamp(ceilingPivots[0].localRotation.eulerAngles.x, 90, ref vCeilingPivotTheta, 5f);
        foreach (Transform t in ceilingPivots) {
            t.localRotation = Quaternion.Euler(ceilingPivotTheta, 0, 0);
        }
    }
}

enum EndPhase
{
    WaitingForTrigger,
    Elevator,
}