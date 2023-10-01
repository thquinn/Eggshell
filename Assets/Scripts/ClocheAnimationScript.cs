using Assets.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClocheAnimationScript : MonoBehaviour
{
    static Quaternion LIFT_ROTATION = Quaternion.Euler(0, 0, 60);

    Vector3 initialPosition, midPosition, finalPosition;
    Quaternion midRotation;
    float timer;
    Vector3 vLift;
    Vector3 vPos, vEulers;

    void Start() {
        initialPosition = transform.position;
        midPosition = initialPosition + new Vector3(-50, 30, 0);
        midRotation = Quaternion.Euler(0, 0, -20);
        finalPosition = initialPosition + new Vector3(-30, 0, 0);
    }

    void Update() {
        timer += Time.deltaTime;
        if (timer < 2) {
            Vector3 pos = transform.position;
            Vector3 eulers = transform.rotation.eulerAngles;
            transform.parent.localRotation = Util.SmoothDampQuaternion(transform.parent.localRotation, LIFT_ROTATION, ref vLift, 1f);
            vPos = transform.position - pos;
            vEulers = transform.rotation.eulerAngles - vEulers;
        } else if (timer < 5) {
            transform.position = Vector3.SmoothDamp(transform.position, midPosition, ref vPos, 1.25f);
            transform.rotation = Util.SmoothDampQuaternion(transform.rotation, midRotation, ref vEulers, 1.25f);
        } else {
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref vPos, 1.5f);
            transform.rotation = Util.SmoothDampQuaternion(transform.rotation, Quaternion.identity, ref vEulers, 1.5f);
        }
    }
}
