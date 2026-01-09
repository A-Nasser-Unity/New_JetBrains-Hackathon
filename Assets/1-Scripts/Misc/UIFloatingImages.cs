using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIFloatingImages : MonoBehaviour
{
    [Header("Images")]
    [SerializeField] private List<Image> images = new List<Image>();

    [Header("Movement")]
    [SerializeField] private float minSpeed = 20f;
    [SerializeField] private float maxSpeed = 60f;

    [Header("Transparency")]
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.8f;

    [Header("Scale")]
    [SerializeField] private float minScale = 0.6f;
    [SerializeField] private float maxScale = 1.2f;

    [Header("Overlap Control")]
    [SerializeField] private float minDistance = 80f;

    private RectTransform canvasRect;
    private Dictionary<Image, float> speeds = new Dictionary<Image, float>();

    void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        foreach (var img in images)
        {
            InitializeImage(img, true);
        }
    }

    void Update()
    {
        foreach (var img in images)
        {
            RectTransform rt = img.rectTransform;

            rt.anchoredPosition += Vector2.up * speeds[img] * Time.deltaTime;

            if (rt.anchoredPosition.y > canvasRect.rect.height / 2f + rt.rect.height)
            {
                InitializeImage(img, false);
            }
        }
    }

    void InitializeImage(Image img, bool firstTime)
    {
        RectTransform rt = img.rectTransform;

        // Random speed
        speeds[img] = Random.Range(minSpeed, maxSpeed);

        // Random scale
        float scale = Random.Range(minScale, maxScale);
        rt.localScale = Vector3.one * scale;

        // Random alpha
        Color c = img.color;
        c.a = Random.Range(minAlpha, maxAlpha);
        img.color = c;

        // Random X (no limits)
        float x = Random.Range(
            -canvasRect.rect.width / 2f,
             canvasRect.rect.width / 2f
        );

        float y = firstTime
            ? Random.Range(-canvasRect.rect.height / 2f, canvasRect.rect.height / 2f)
            : -canvasRect.rect.height / 2f - rt.rect.height;

        rt.anchoredPosition = FindNonOverlappingPosition(new Vector2(x, y), rt);
    }

    Vector2 FindNonOverlappingPosition(Vector2 startPos, RectTransform current)
    {
        Vector2 pos = startPos;
        int safety = 0;

        while (IsOverlapping(pos, current) && safety < 50)
        {
            pos.x = Random.Range(
                -canvasRect.rect.width / 2f,
                 canvasRect.rect.width / 2f
            );
            safety++;
        }

        return pos;
    }

    bool IsOverlapping(Vector2 pos, RectTransform current)
    {
        foreach (var img in images)
        {
            if (img.rectTransform == current) continue;

            if (Vector2.Distance(
                pos,
                img.rectTransform.anchoredPosition
            ) < minDistance)
                return true;
        }

        return false;
    }
}
