using UnityEngine;
using TMPro;

public class CompareTMPNumbers : MonoBehaviour
{
    [Header("Text Inputs")]
    public TMP_Text firstText;
    public TMP_Text secondText;

    [Header("Result GameObjects")]
    public GameObject firstGreaterObject;
    public GameObject secondGreaterObject;
    public GameObject equalObject;

    private void OnEnable()
    {
        // Safety check
        if (firstText == null || secondText == null)
            return;

        int firstValue = int.Parse(firstText.text);
        int secondValue = int.Parse(secondText.text);

        // Disable all first
        firstGreaterObject.SetActive(false);
        secondGreaterObject.SetActive(false);
        equalObject.SetActive(false);

        // Compare and activate
        if (firstValue > secondValue)
        {
            firstGreaterObject.SetActive(true);
        }
        else if (firstValue < secondValue)
        {
            secondGreaterObject.SetActive(true);
        }
        else
        {
            equalObject.SetActive(true);
        }
    }
}