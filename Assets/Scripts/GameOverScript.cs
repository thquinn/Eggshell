using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public CanvasGroup pink, black;

    float vPink, vBlack;

    void Update() {
        pink.alpha = Mathf.SmoothDamp(pink.alpha, 1, ref vPink, 1f);
        if (pink.alpha > .9f) {
            black.alpha = Mathf.SmoothDamp(black.alpha, 1, ref vBlack, 1f);
            AudioListener.volume = 1 - black.alpha;
        }
    }
}
