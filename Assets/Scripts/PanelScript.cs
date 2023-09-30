using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    static float RAISE_TIME = .2f;
    static Vector3 RAISE_TARGET = new(0, .1f, 0);
    static float SLIDE_TIME = .5f;
    static Vector3 SLIDE_TARGET = new(0, .1f, 1.25f);
    static float SUBJECT_RAISE_TIME = .5f;

    public Transform[] panels;
    public Transform subject;

    Vector3 subjectRaisedPosition;
    bool opening;
    float t;
    Vector3 v, subjectV;

    void Start() {
        subjectRaisedPosition = subject.localPosition;
        subject.localPosition -= new Vector3(0, 4, 0);
    }
    public void Open() {
        opening = true;
    }
    
    void Update() {
        if (!opening) {
            return;
        }
        t += Time.deltaTime;
        if (t < RAISE_TIME) {
            panels[0].localPosition = Vector3.SmoothDamp(panels[0].localPosition, RAISE_TARGET, ref v, RAISE_TIME);
        } else {
            panels[0].localPosition = Vector3.SmoothDamp(panels[0].localPosition, SLIDE_TARGET, ref v, SLIDE_TIME);
        }
        panels[1].localPosition = panels[0].localPosition;
        subject.localPosition = Vector3.SmoothDamp(subject.localPosition, subjectRaisedPosition, ref subjectV, SUBJECT_RAISE_TIME);
    }
}
