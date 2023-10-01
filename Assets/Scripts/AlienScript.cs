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
    static Vector2 KNOCK_TIMER_RANGE = new(8, 10);
    static Vector2 REPOSITION_TIMER_RANGE = new(5, 15);
    static Vector2 BLINK_TIMER_RANGE = new(1, 5);
    static float BLINK_TIME = .1f;
    static float FLOAT_STRENGTH = .05f;

    static float INTRO_WAIT_SECONDS_AWAKEN = 1;// DEBUG: 6;
    static float INTRO_WAIT_SECONDS_APPEAR = 1;//2;

    public Transform cameraTransform;
    public Transform bobAnchor;
    public Transform[] sclarae, irises;
    public Transform mouthTransform;
    public AudioSource sfxSourceKnock, sfxSourceSpeech, sfxSourceWhisper;
    public AudioClip[] sfxClipsKnock;
    public VOScriptableObject[] voIntroTalking, voIntroWaitingToProgress;

    RoomScript currentRoom;
    AlienState state;
    float stateTimer;
    float knockTimer;
    float blinkTimer, repositionTimer;
    Vector3 targetPosition;
    Vector3 vTranslate, vRotate;
    Vector3 lookOverride;
    Vector3 vGrin;

    Queue<VOScriptableObject> voQueue;
    public VOScriptableObject voActive;

    void Start() {
        if (instance == null) {
            instance = this;
        }
        // Start at max distance, in the middle of the XY range.
        float zStart = Z_RANGE.y;
        float zMid = (Z_RANGE.x + Z_RANGE.y) / 2;
        float xMid = zStart * (X_RANGE_PER_Z.x + X_RANGE_PER_Z.y) / 2;
        float yTarget = zStart * Mathf.Lerp(Y_RANGE_PER_Z.x, Y_RANGE_PER_Z.y, .8f);
        transform.localPosition = new Vector3(xMid, Y_RANGE_PER_Z.x * zStart, zStart);
        targetPosition = new Vector3(xMid, yTarget, zMid);
        foreach (Transform t in sclarae) {
            t.localScale = new Vector3(1, 0, 1);
        }
        SetRepositionTimer();
        voQueue = new();
    }
    void SetRepositionTimer() {
        repositionTimer = Util.SampleRangeVector(REPOSITION_TIMER_RANGE);
    }
    public void ChangeState(AlienState newState) {
        state = newState;
        stateTimer = 0;
    }
    public void SetCurrentRoom(RoomScript roomScript) {
        currentRoom = roomScript;
        voQueue.Clear();
        lookOverride = Vector3.zero;
        if (!currentRoom.isIntroRoom) {
            state = AlienState.Main;
        }
    }

    void Update() {
        stateTimer += Time.deltaTime;
        if (state == AlienState.IntroWaitingToAwaken) {
            if (stateTimer > INTRO_WAIT_SECONDS_AWAKEN) {
                ChangeState(AlienState.IntroKnocking);
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
            if (stateTimer > INTRO_WAIT_SECONDS_APPEAR) {
                ChangeState(AlienState.IntroTalking);
                voQueue = new Queue<VOScriptableObject>(voIntroTalking);
                SFXSpeak();
            }
        } else if (state == AlienState.IntroTalking) {
            if (voActive == null && voQueue.Count == 0) {
                ChangeState(AlienState.IntroWaitingToProgress);
            }
        }
        UpdateMain();
        if (state == AlienState.EndGrin) {
            mouthTransform.localScale = Vector3.SmoothDamp(mouthTransform.localScale, Vector3.one, ref vGrin, 1f);
        }
    }

    void UpdateMain() {
        // Choose look target.
        Vector3 lookTarget = (lookOverride == Vector3.zero) ? cameraTransform.position : lookOverride;
        // Rotation and iris movement.
        Quaternion lookRotation = Quaternion.LookRotation(transform.position - lookTarget);
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
        } else if (state == AlienState.EndElevator) {
            float z = Z_RANGE.x;
            targetPosition = new(0, Y_RANGE_PER_Z.x * z, z);
        } else if (state == AlienState.EndLooming) {
            targetPosition = new(0, 20, 7);
        }
        Vector3 offsetTargetPosition = targetPosition;
        float roomX = Mathf.RoundToInt(cameraTransform.position.x / 8) * 8;
        offsetTargetPosition.x += roomX;
        if (state <= AlienState.EndElevator) {
            float xOffset = 1 * (roomX - lookTarget.x);
            xOffset = Mathf.Clamp(xOffset, -4, 4);
            float yOffset = lookTarget.y;
            offsetTargetPosition.x += xOffset;
            offsetTargetPosition.y -= yOffset * .66f;
            offsetTargetPosition.z += lookTarget.z * .1f;
        }
        if (state >= AlienState.IntroAppearing) {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, offsetTargetPosition, ref vTranslate, 2, 8);
        }
        bobAnchor.localPosition = new Vector3(
            Mathf.Sin(Time.time),
            Mathf.Sin(Time.time * 2.1f),
            state <= AlienState.EndElevator ? Mathf.Sin(Time.time * .5f) : 0
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
            blinkScale = Mathf.Lerp(0, blinkScale, stateTimer * 4);
        }
        foreach (Transform t in sclarae) {
            t.localScale = new Vector3(1, blinkScale, 1);
        }
    }

    void SFXKnock() {
        sfxSourceKnock.PlayOneShot(sfxClipsKnock[Random.Range(0, sfxClipsKnock.Length)], 2);
    }
    void SFXSpeak() {
        if (voQueue.Count == 0) {
            return;
        }
        VOScriptableObject vo = voQueue.Dequeue();
        sfxSourceSpeech.PlayOneShot(vo.voiceClip);
        sfxSourceWhisper.PlayOneShot(vo.whisperClip);
        voActive = vo;
        HandleScriptActions(vo.startActions);
        float length = Mathf.Max(vo.voiceClip.length, vo.whisperClip.length);
        Invoke("ClearVOActive", length - 1);
    }
    void ClearVOActive() {
        if (voActive != null) {
            HandleScriptActions(voActive.endActions);
        }
        if (voQueue.Count > 0) {
            Invoke("SFXSpeak", 2 + voActive.wait);
        }
        voActive = null;
    }
    void HandleScriptActions(VOScriptAction[] actions) {
        foreach (VOScriptAction action in actions) {
            if (action == VOScriptAction.Replay) {
                voQueue.Enqueue(voActive);
            } else if (action == VOScriptAction.OpenDoor) {
                currentRoom.OpenDoor();
            } else if (action == VOScriptAction.LookAtDoor) {
                lookOverride = currentRoom.transform.position + new Vector3(6, 2, -6);
            } else if (action == VOScriptAction.LookAtPlayer) {
                lookOverride = Vector3.zero;
            } else if (action == VOScriptAction.Grin) {
                ChangeState(AlienState.EndGrin);
            }
        }
    }
}

public enum AlienState
{
    IntroWaitingToAwaken,
    IntroKnocking,
    IntroAppearing,
    IntroTalking,
    IntroWaitingToProgress,
    Main,
    EndElevator,
    EndLooming,
    EndGrin,
}