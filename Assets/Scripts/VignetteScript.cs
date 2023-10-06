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
            Destroy(gameObject);
            return;
        }
    }
    public void Knock() {
        Invoke("KnockImpl", .5f);
    }
    void KnockImpl() {
        if (!dismissed) {
            yTarget += 1000 + knocks * 2500;
        }
        knocks++;
    }

    void Update() {
        float closeSpeed = 1f / Mathf.Max(1, knocks);
        if (dismissed) {
            yTarget = 2000;
        } else {
            yTarget = Mathf.SmoothDamp(yTarget, 0, ref vYTarget, closeSpeed);
        }
        eyelidHeight = Mathf.SmoothDamp(eyelidHeight, yTarget, ref vEyelid, eyelidHeight < yTarget ? closeSpeed : 1 / closeSpeed);
        foreach (RectTransform rt in eyelids) {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, eyelidHeight);
        }
        if (eyelidHeight > 1500 && knocks >= 2) {
            dismissed = true;
        }
        if (dismissed) {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, new Vector3(2, 2, 1), ref vScale, 1);
            if (transform.localScale.x > 1.9f) {
                Destroy(gameObject);
                return;
            }
            dismissTime += Time.deltaTime;
        }
    }
}
