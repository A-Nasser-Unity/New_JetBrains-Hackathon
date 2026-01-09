using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFadeIn : MonoBehaviour
{
    public Image targetImage;

    [Range(0, 255)]
    public float targetTransparency = 155f;

    public float fadeTime = 2f;

    void OnEnable()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        Color c = targetImage.color;
        c.a = 0f;
        targetImage.color = c;

        float targetAlpha = targetTransparency / 255f;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, targetAlpha, t / fadeTime);
            targetImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        targetImage.color = c;
    }
}