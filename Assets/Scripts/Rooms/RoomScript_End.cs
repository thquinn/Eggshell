using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript_End : MonoBehaviour
{
    static float ELEVATOR_RIDE_LENGTH = 10;
    static float CLOCHE_DIM_FACTOR = .66f;

    public FootprintTriggerScript trigger;
    public VOScriptableObject[] elevatorVOs;
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
    Color initAmbientSky, initAmbientEquator, initAmbientGround;
    Color dimmedAmbientSky, dimmedAmbientEquator, dimmedAmbientGround;
    float tAmbient, vAmbient;

    void Start() {
        elevatorTargetPosition = trigger.transform.localPosition + new Vector3(0, 7.5f, 0);
        diningParent.SetActive(false);
        initAmbientSky = RenderSettings.ambientSkyColor;
        dimmedAmbientSky = Color.Lerp(initAmbientSky, Color.black, CLOCHE_DIM_FACTOR);
        initAmbientEquator = RenderSettings.ambientEquatorColor;
        dimmedAmbientEquator = Color.Lerp(initAmbientEquator, Color.black, CLOCHE_DIM_FACTOR);
        initAmbientGround = RenderSettings.ambientGroundColor;
        dimmedAmbientGround = Color.Lerp(initAmbientGround, Color.black, CLOCHE_DIM_FACTOR);
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
            AlienScript.instance.EnqueueVO(elevatorVOs);
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
        if (!alienLoom && playerY > 5.5f) {
            AlienScript.instance.ChangeState(AlienState.EndLooming);
            alienLoom = true;
        }
        if (!clocheAnimation.enabled && playerY > 7.25f) {
            clocheAnimation.enabled = true;
        }
        float ceilingPivotTheta = Mathf.SmoothDamp(ceilingPivots[0].localRotation.eulerAngles.x, 90, ref vCeilingPivotTheta, 5f);
        for (int i = 0; i < ceilingPivots.Length; i++) {
            ceilingPivots[i].localRotation = Quaternion.Euler(ceilingPivotTheta, i * 180, 0);
        }
        // Dimming effects.
        if (clocheAnimation.enabled) {
            tAmbient = Mathf.SmoothDamp(tAmbient, 0, ref vAmbient, .1f);
        }
        else if (playerY > 4.25f) {
            tAmbient = Mathf.SmoothDamp(tAmbient, 1, ref vAmbient, 6f);
            // Get rid of the fake dimmer.
            Color c = dimmerRenderer.color;
            c.a = Mathf.SmoothDamp(c.a, 0, ref vDimmerAlpha, .5f);
            dimmerRenderer.color = c;
        }
        RenderSettings.ambientSkyColor = Color.Lerp(initAmbientSky, dimmedAmbientSky, tAmbient);
        RenderSettings.ambientEquatorColor = Color.Lerp(initAmbientEquator, dimmedAmbientEquator, tAmbient);
        RenderSettings.ambientGroundColor = Color.Lerp(initAmbientGround, dimmedAmbientGround, tAmbient);
    }
}

enum EndPhase
{
    WaitingForTrigger,
    Elevator,
}