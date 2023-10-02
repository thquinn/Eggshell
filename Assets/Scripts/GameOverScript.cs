using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public CanvasGroup pink, black;

    float delayTimer;
    float vPink, vBlack;
    float deadTimer;

    void Update() {
        delayTimer += Time.deltaTime;
        if (delayTimer < 4) { return; }
        pink.alpha = Mathf.SmoothDamp(pink.alpha, 1, ref vPink, 1f);
        if (pink.alpha > .9f) {
            black.alpha = Mathf.SmoothDamp(black.alpha, 1, ref vBlack, 1f);
            AudioListener.volume = 1 - black.alpha;
            if (black.alpha > .95f) {
                deadTimer += Time.deltaTime;
                if (deadTimer > 1) {
                    Application.Quit();
                }
            }
        }
    }
}
