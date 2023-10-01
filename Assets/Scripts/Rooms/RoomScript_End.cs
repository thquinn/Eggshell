using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript_End : MonoBehaviour
{
    static float ELEVATOR_RIDE_LENGTH = 10;

    public FootprintTriggerScript trigger;
    public GameObject[] elevatorParts;
    public Transform[] ceilingPivots;
    public GameObject diningParent, clocheMock;
    public ClocheAnimationScript clocheAnimation;
    public SpriteRenderer dimmerRenderer;

    Vector3 elevatorTargetPosition;
    EndPhase phase;
    Vector3 vElevator, vCentering;
    float vCeilingPivotTheta;
    float vDimmerAlpha;
    bool swapped, alienLoom;

    void Start() {
        elevatorTargetPosition = trigger.transform.localPosition + new Vector3(0, 7.5f, 0);
        diningParent.SetActive(false);
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
            AlienScript.instance.ChangeState(AlienState.EndElevator);
        }
    }
    void UpdateElevator() {
        Transform playerTransform = PlayerScript.instance.transform;
        trigger.transform.localPosition = Vector3.SmoothDamp(trigger.transform.localPosition, elevatorTargetPosition, ref vElevator, ELEVATOR_RIDE_LENGTH);
        playerTransform.position = Vector3.SmoothDamp(playerTransform.position, trigger.transform.position, ref vCentering, .2f);
        float playerY = playerTransform.position.y;
        if (!swapped && playerY > 4.25f) {
            diningParent.SetActive(true);
            clocheMock.SetActive(false);
            swapped = true;
        }
        if (playerY > 4.25f) {
            Color c = dimmerRenderer.color;
            c.a = Mathf.SmoothDamp(c.a, 0, ref vDimmerAlpha, .5f);
            dimmerRenderer.color = c;
        }
        if (!alienLoom && playerY > 5.5f) {
            AlienScript.instance.ChangeState(AlienState.EndLooming);
            alienLoom = true;
        }
        if (!clocheAnimation.enabled && playerY > 6.25f) {
            clocheAnimation.enabled = true;
        }
        float ceilingPivotTheta = Mathf.SmoothDamp(ceilingPivots[0].localRotation.eulerAngles.x, 90, ref vCeilingPivotTheta, 5f);
        for (int i = 0; i < ceilingPivots.Length; i++) {
            ceilingPivots[i].localRotation = Quaternion.Euler(ceilingPivotTheta, i * 180, 0);
        }
    }
}

enum EndPhase
{
    WaitingForTrigger,
    Elevator,
}