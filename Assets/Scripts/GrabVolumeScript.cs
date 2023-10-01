using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabVolumeScript : MonoBehaviour
{
    public static string TAG_GRABBED = "Grabbed";
    static float HOLD_POSITION_OFFSET_FORWARD = 1;

    Vector3 holdPosition;
    List<Rigidbody> withinVolume;
    List<SimonButtonScript> buttonsWithinVolume;
    Rigidbody grabbed;

    void Start() {
        withinVolume = new();
        buttonsWithinVolume = new();
    }

    void Update() {
        holdPosition = transform.position + transform.up * HOLD_POSITION_OFFSET_FORWARD;
        if (Input.GetMouseButtonDown(0) && buttonsWithinVolume.Count > 0) {
            int minIndex = buttonsWithinVolume.Select((item, index) => ((item.transform.position - holdPosition).sqrMagnitude, index)).Min().index;
            buttonsWithinVolume[minIndex].Push();
            return;
        }
        if (Input.GetMouseButtonDown(0) && withinVolume.Count > 0) {
            int minIndex = withinVolume.Select((item, index) => ((item.position - holdPosition).sqrMagnitude, index)).Min().index;
            grabbed = withinVolume[minIndex];
            grabbed.gameObject.tag = TAG_GRABBED;
        }
        if (!Input.GetMouseButton(0) && grabbed != null) {
            grabbed.gameObject.tag = "Untagged";
            grabbed = null;
        }
    }
    void FixedUpdate() {
        Debug.DrawLine(holdPosition, holdPosition + new Vector3(0, .1f, 0));
        if (grabbed != null) {
            Vector3 v = grabbed.velocity;
            Vector3.SmoothDamp(grabbed.position, holdPosition, ref v, .01f, 999, Time.fixedDeltaTime);
            grabbed.angularVelocity *= .9f;
            grabbed.velocity = v;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody != null) {
            withinVolume.Add(other.attachedRigidbody);
        }
        SimonButtonScript buttonScript = other.GetComponent<SimonButtonScript>();
        if (buttonScript != null) {
            buttonsWithinVolume.Add(buttonScript);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.attachedRigidbody != null) {
            withinVolume.Remove(other.attachedRigidbody);
        }
        SimonButtonScript buttonScript = other.GetComponent<SimonButtonScript>();
        if (buttonScript != null) {
            buttonsWithinVolume.Remove(buttonScript);
        }
    }
}
