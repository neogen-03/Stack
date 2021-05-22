using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    [SerializeField] Palette[] palettes;

    [SerializeField] Material skyboxMaterial;
    public Material groundMaterial;

    [SerializeField] Material skyboxTop, skyboxBottom;

    public int changePaletteCount;
    int paletteChangeCount;
    int nextPaletteNumber;
    int currentPalette;

    void Awake() => instance = this;

    void Start() => StartPalette();

    void Update()
    {
        skyboxMaterial.SetColor("_Color2", skyboxTop.color);
        skyboxMaterial.SetColor("_Color3", skyboxBottom.color);

        if (changePaletteCount == paletteChangeCount)
            StartCoroutine(ChangeColor());
    }

    void StartPalette()
    {
        int randomPalette = Random.Range(0, palettes.Length);
        paletteChangeCount = Random.Range(10, 20);
        currentPalette = randomPalette;

        if (randomPalette < palettes.Length - 1)
            nextPaletteNumber = randomPalette + 1;
        else
            nextPaletteNumber = 0;

        skyboxTop.color = palettes[randomPalette].skyboxTop;
        skyboxBottom.color = palettes[randomPalette].skyboxBottom;
        groundMaterial.color = palettes[randomPalette].ground;
        UIManager.instance.fog.color = palettes[randomPalette].fog;
    }

    IEnumerator ChangeColor()
    {
        skyboxTop.DOColor(palettes[nextPaletteNumber].skyboxTop, 2f);
        skyboxBottom.DOColor(palettes[nextPaletteNumber].skyboxBottom, 2f);
        groundMaterial.DOColor(palettes[nextPaletteNumber].ground, 2f);
        UIManager.instance.fog.DOColor(palettes[nextPaletteNumber].fog, 2f);

        changePaletteCount = 0;

        yield return new WaitForSeconds(2f);

        currentPalette = nextPaletteNumber;

        if (currentPalette < palettes.Length - 1)
            nextPaletteNumber = currentPalette + 1;
        else
            nextPaletteNumber = 0;

        paletteChangeCount = Random.Range(10, 20);
    }
}

[System.Serializable]
class Palette
{
    public Color skyboxTop;
    public Color skyboxBottom;
    public Color ground;
    public Color fog;
}