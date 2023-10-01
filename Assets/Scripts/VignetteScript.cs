using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteScript : MonoBehaviour
{
    public static VignetteScript instance;

    public RectTransform[] eyelids;

    int knocks = 0;
    float eyelidHeight = 0, vEyelid;
    float yTarget = 0, vYTarget;
    public bool dismissed;
    public float dismissTime;
    Vector3 vScale;

    void Start() {
        instance = this;
        if (Application.isEditor) {
            //enabled = false; // DEBUG
            //return;
        }
    }
    public void Knock() {
        knocks++;
        if (!dismissed) {
            yTarget += knocks * 1000;
        }
    }

    void Update() {
        float closeSpeed = 1f / Mathf.Max(1, knocks);
        if (!dismissed) {
            yTarget = Mathf.SmoothDamp(yTarget, 0, ref vYTarget, closeSpeed);
        }
        eyelidHeight = Mathf.SmoothDamp(eyelidHeight, yTarget, ref vEyelid, closeSpeed);
        foreach (RectTransform rt in eyelids) {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, eyelidHeight);
        }
        if (eyelidHeight > 1000 && knocks >= 2) {
            dismissed = true;
        }
        if (dismissed) {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, new Vector3(2, 2, 1), ref vScale, 3);
            dismissTime += Time.deltaTime;
        }
    }
}
