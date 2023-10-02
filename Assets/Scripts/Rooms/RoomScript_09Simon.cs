using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript_09Simon : MonoBehaviour
{
    static int TIMER_FLATNESS = 20;
    static int TIMER_LENIENCY = 180;

    public RoomScript roomScript;
    public VOScriptableObject[] words;

    List<int> sequence;
    int testIndex;
    float playTimer;
    float totalGuesses;
    int totalSuccesses, totalFails, maxLength;

    void Start() {
        sequence = new List<int>();
    }

    void Update() {
        if (!roomScript.panelCeiling.open) {
            return;
        }
        if (testIndex >= sequence.Count) {
            ExtendSequence();
        }
        playTimer -= Time.deltaTime;
        if (playTimer <= 0) {
            totalGuesses += 2;
            Fail();
        }
    }
    void ExtendSequence() {
        if (sequence.Count == 0) {
            sequence.Add(Random.Range(0, 5));
        }
        int next;
        do { next = Random.Range(0, 5); }
        while (next == sequence[sequence.Count - 1] || next == sequence[0]);
        sequence.Add(next);
        maxLength = Mathf.Max(maxLength, sequence.Count);
        testIndex = 0;
        PlayWord(sequence[0]);
        ResetTimer();
    }
    bool first = true;
    void PlayWord(int index) {
        if (first) {
            AlienScript.instance.EnqueueVO(words[index]);
            first = false;
        } else {
            AlienScript.instance.SFXSpeak(words[index]);
        }
    }
    void ResetTimer() {
        playTimer = (TIMER_FLATNESS + TIMER_LENIENCY) / (totalGuesses + TIMER_FLATNESS);
    }
    void Fail() {
        sequence.Clear();
        roomScript.sfxTestFail.Play();
        totalFails++;
        if (totalFails > 12) {
            AlienScript.instance.ClearVOQueue();
            roomScript.OpenDoor(false);
            enabled = false;
        } else {
            ResetTimer();
        }
    }

    public void HandleButton(int number) {
        if (testIndex >= sequence.Count) return;
        totalGuesses++;
        if (number == sequence[testIndex]) {
            totalSuccesses++;
            testIndex++;
            roomScript.sfxTestPass.Play();
            if (testIndex < sequence.Count) {
                PlayWord(sequence[testIndex]);
            }
            ResetTimer();
        } else {
            Fail();
        }
    }
}