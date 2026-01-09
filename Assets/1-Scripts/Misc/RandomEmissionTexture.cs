using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RandomEmissionTexture : MonoBehaviour
{
    [SerializeField] private Texture2D[] emissionTextures;

    private void Start()
    {
        if (emissionTextures == null || emissionTextures.Length == 0)
            return;

        Renderer rend = GetComponent<Renderer>();
        Material mat = rend.material; // instance material

        Texture2D selected = emissionTextures[Random.Range(0, emissionTextures.Length)];

        mat.SetTexture("_EmissionMap", selected);
        mat.EnableKeyword("_EMISSION");
    }
}