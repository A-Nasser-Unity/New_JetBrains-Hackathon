using UnityEngine;
using TMPro;

public class TMPTextChangeActivator : MonoBehaviour
{
    public TextMeshProUGUI targetText;
    public GameObject objectToActivate;

    private string lastText = "";

    void Update()
    {
        if (targetText.text != lastText)
        {
            lastText = targetText.text;

            if (!string.IsNullOrEmpty(lastText))
            {
                objectToActivate.SetActive(true);
            }
        }
    }
}