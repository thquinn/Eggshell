using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienScript : MonoBehaviour
{
    public static AlienScript instance;

    static Vector2 Z_RANGE = new(8, 13);
    static Vector2 X_RANGE_PER_Z = new(-.3f, .3f);
    static Vector2 Y_RANGE_PER_Z = new(0, .9f);
    static Vector2 KNOCK_TIMER_RANGE = new(4, 6);
    static Vector2 REPOSITION_TIMER_RANGE = new(5, 15);
    static Vector2 BLINK_TIMER_RANGE = new(1, 5);
    static float BLINK_TIME = .1f;
    static float FLOAT_STRENGTH = .05f;

    public Transform cameraTransform;
    public Transform bobAnchor;
    public Transform[] sclarae, irises;
    public AudioSource sfxSourceKnock;
    public AudioClip[] sfxClipsKnock;

    AlienState state;
    float stateTimer;
    float knockTimer;
    float blinkTimer, repositionTimer;
    Vector3 targetPosition;
    Vector3 vTranslate, vRotate;

    void Start() {
        if (instance == null) {
            instance = this;
        }
        // Start at max distance, in the middle of the XY range.
        float zStart = Z_RANGE.y;
        float zMid = (Z_RANGE.x + Z_RANGE.y) / 2;
        float xMid = zStart * (X_RANGE_PER_Z.x + X_RANGE_PER_Z.y) / 2;
        float yMid = zStart * (Y_RANGE_PER_Z.x + Y_RANGE_PER_Z.y) / 2;
        transform.localPosition = new Vector3(xMid, Y_RANGE_PER_Z.x * zStart, zStart);
        targetPosition = new Vector3(xMid, yMid, zMid);
        foreach (Transform t in sclarae) {
            t.localScale = new Vector3(1, 0, 1);
        }
        SetRepositionTimer();
    }
    void SetRepositionTimer() {
        repositionTimer = Util.SampleRangeVector(REPOSITION_TIMER_RANGE);
    }
    void ChangeState(AlienState newState) {
        state = newState;
        stateTimer = 0;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            SFXKnock();
        }
        stateTimer += Time.deltaTime;
        if (state == AlienState.IntroWaitingToAwaken) {
            if (stateTimer > 6) {
                ChangeState(AlienState.IntroKnocking);
                knockTimer = Util.SampleRangeVector(KNOCK_TIMER_RANGE);
            }
        } else if (state == AlienState.IntroKnocking) {
            if (Vector3.Dot(cameraTransform.forward, Vector3.forward) < .66f) {
                stateTimer = 0; // Player isn't looking at the window.
                knockTimer -= Time.deltaTime;
            } else if (cameraTransform.position.z < -5) {
                stateTimer = 0; // Player is too far from the window.
                knockTimer -= Time.deltaTime;
            } else {
                knockTimer = Util.SampleRangeVector(KNOCK_TIMER_RANGE);
            }
            if (knockTimer <= 0) {
                SFXKnock();
                knockTimer = Util.SampleRangeVector(KNOCK_TIMER_RANGE);
            }
            if (stateTimer > 2) {
                ChangeState(AlienState.IntroAppearing);
            }
        } else if (state == AlienState.IntroAppearing) {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref vTranslate, .5f);
            if (stateTimer > 3) {
                ChangeState(AlienState.IntroTalking);
            }
        }
        UpdateMain();
    }

    void UpdateMain() {
        // Rotation and iris movement.
        Quaternion lookRotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        transform.localRotation = Util.SmoothDampQuaternion(transform.localRotation, lookRotation, ref vRotate, .33f);
        Vector3 lookDistance = (lookRotation * Quaternion.Inverse(transform.localRotation)).eulerAngles;
        if (lookDistance.x < -180) lookDistance.x += 360;
        if (lookDistance.x > 180) lookDistance.x -= 360;
        if (lookDistance.y < -180) lookDistance.y += 360;
        if (lookDistance.y > 180) lookDistance.y -= 360;
        Vector3 irisPosition = new Vector3(lookDistance.y * -.0066f, lookDistance.x * .0066f, -.01f);
        foreach (Transform t in irises) {
            t.localPosition = irisPosition;
        }
        // Position.
        if (state == AlienState.Main) {
            repositionTimer -= Time.deltaTime;
            if (repositionTimer <= 0) {
                float z = Util.SampleRangeVector(Z_RANGE);
                float x = Util.SampleRangeVector(X_RANGE_PER_Z) * z;
                float y = Util.SampleRangeVector(Y_RANGE_PER_Z) * z;
                targetPosition = new Vector3(x, y, z);
                SetRepositionTimer();
            }
        }
        Vector3 offsetTargetPosition = targetPosition;
        float roomX = Mathf.RoundToInt(cameraTransform.position.x / 8) * 8;
        offsetTargetPosition.x += roomX;
        offsetTargetPosition.x += 1 * (roomX - cameraTransform.position.x);
        offsetTargetPosition.z += .1f * cameraTransform.position.z;
        if (state >= AlienState.IntroAppearing) {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, offsetTargetPosition, ref vTranslate, 2, 8);
        }
        bobAnchor.localPosition = new Vector3(
            Mathf.Sin(Time.time),
            Mathf.Sin(Time.time * 2.1f),
             Mathf.Sin(Time.time * .5f)
        ) * FLOAT_STRENGTH;
        // Blink.
        if (state >= AlienState.IntroAppearing) {
            blinkTimer -= Time.deltaTime;
        }
        if (blinkTimer <= 0) {
            blinkTimer = Util.SampleRangeVector(BLINK_TIMER_RANGE);
        }
        float blinkT = blinkTimer < BLINK_TIME ? 1 - ((blinkTimer / BLINK_TIME) - BLINK_TIME / 2) * 2 : 1;
        float blinkScale = Mathf.Min(1, blinkT, .9f + .1f * Mathf.Sin(Time.time * .4f));
        if (state < AlienState.IntroAppearing) {
            blinkScale = 0;
        } else if (state == AlienState.IntroAppearing) {
            blinkScale = Mathf.Lerp(0, blinkScale, stateTimer * 2);
        }
        foreach (Transform t in sclarae) {
            t.localScale = new Vector3(1, blinkScale, 1);
        }
    }

    void SFXKnock() {
        sfxSourceKnock.PlayOneShot(sfxClipsKnock[Random.Range(0, sfxClipsKnock.Length)], 2);
    }
}

enum AlienState
{
    IntroWaitingToAwaken,
    IntroKnocking,
    IntroAppearing,
    IntroTalking,
    IntroWaitingToProgress,
    Main,
}