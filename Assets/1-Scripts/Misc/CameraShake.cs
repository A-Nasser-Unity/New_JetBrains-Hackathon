using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public Camera targetCamera;
    public float shakeStrength = 0.2f;
    public float shakeDuration = 0.3f;

    Vector3 originalPosition;

    void OnEnable()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        originalPosition = targetCamera.transform.position;
        StartCoroutine(Shake());
        StartCoroutine(DeactivateAfterDelay());
    }

    IEnumerator Shake()
    {
        float time = 0f;

        while (time < shakeDuration)
        {
            time += Time.deltaTime;

            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;
            targetCamera.transform.position = originalPosition + randomOffset;

            yield return null;
        }

        targetCamera.transform.position = originalPosition;
    }

    IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}