using UnityEngine;

[CreateAssetMenu(fileName = "VoiceoverScriptableObject", menuName = "Scriptable Objects/VoiceoverScriptableObject", order = 1)]
public class VOScriptableObject : ScriptableObject
{
    public AudioClip voiceClip, whisperClip;
    public string alienText, englishText;
    public VOScriptAction[] startActions, endActions;
    public float wait;
}

public enum VOScriptAction {
    Replay,
    OpenDoor,
    LookAtPlayer,
    LookAtDoor,
    Grin,
}