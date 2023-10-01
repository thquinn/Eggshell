using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonButtonScript : MonoBehaviour
{
    static Vector3 PUSHED_OFFSET = new Vector3(0, -.05f, 0);

    public RoomScript_09Simon roomScript;
    public int buttonIndex;

    public SpriteRenderer glowRenderer;
    public AudioSource sfx;
    Vector3 vPush;
    float vGlowAlpha;

    public void Push() {
        roomScript.HandleButton(buttonIndex);
        transform.localPosition = PUSHED_OFFSET;
        vPush = Vector3.zero;
        Color c = glowRenderer.color;
        c.a = 1;
        glowRenderer.color = c;
        vGlowAlpha = 0;
        sfx.Play();
    }

    void Update() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref vPush, .5f);
        Color c = glowRenderer.color;
        c.a = Mathf.SmoothDamp(c.a, 0, ref vGlowAlpha, .66f);
        glowRenderer.color = c;
    }
}
