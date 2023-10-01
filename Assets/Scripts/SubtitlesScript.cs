using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SubtitlesScript : MonoBehaviour
{
    public TextMeshProUGUI tmpAlien;

    VOScriptableObject voActive;
    float vAlpha;

    void Start() {
        Color c = tmpAlien.color;
        c.a = 0;
        tmpAlien.color = c;
    }

    void Update() {
        Color c = tmpAlien.color;
        c.a = Mathf.SmoothDamp(c.a, voActive == null ? 0 : 1, ref vAlpha, vAlpha > 0 ? 1f : .5f);
        tmpAlien.color = c;

        if (voActive != AlienScript.instance.voActive) {
            voActive = AlienScript.instance.voActive;
            if (voActive != null) {
                tmpAlien.text = AlienTextToTennent(voActive.alienText);
            }
        }
    }

    StringBuilder sb = new StringBuilder();
    string AlienTextToTennent(string alienText) {
        alienText = alienText.Trim();
        if (alienText.EndsWith('.')) {
            alienText = alienText.Substring(0, alienText.Length - 1);
        }
        alienText = alienText.Replace(",", "").Replace("'", "").Replace(".", "");
        alienText = Regex.Replace(alienText, @"\s+", " ");
        string[] tokens = alienText.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        sb.Clear();
        Random.State randomState = Random.state;
        foreach (string t in tokens) {
            TranslateAndPrependToken(sb, t);
        }
        Random.state = randomState;
        string tennent = sb.ToString();
        tennent = Regex.Replace(tennent, @"(\w)\1+", "$1\\.");
        return new string(tennent.Reverse().ToArray());
    }
    void TranslateAndPrependToken(StringBuilder sb, string alienToken) {
        Random.InitState(alienToken.GetHashCode());
        int length = Mathf.Max(1, Mathf.RoundToInt(alienToken.Length * Random.Range(.25f, .5f)));
        for (int i = 0; i < length; i++) {
            sb.Append((char)('a' + Random.Range(0, 26)));
        }
    }
}
