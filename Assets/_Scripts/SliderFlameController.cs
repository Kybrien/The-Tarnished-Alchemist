using UnityEngine;
using UnityEngine.UI;

public class SliderBarSwitcher : MonoBehaviour
{
    public Slider slider;          // Le slider principal
    public Image flameRed;         // Barre rouge (0 - 0.33)
    public Image flamePink;        // Barre rose (0.33 - 0.66)
    public Image flameBlue;        // Barre bleue (0.66 - 1)

    void Start()
    {
        UpdateBar(); // Mise � jour initiale
    }

    public void OnSliderValueChanged()
    {
        UpdateBar();
    }

    void UpdateBar()
    {
        float sliderValue = slider.value;

        // R�initialisation des barres
        flameRed.fillAmount = 0;
        flamePink.fillAmount = 0;
        flameBlue.fillAmount = 0;

        flameRed.gameObject.SetActive(false);
        flamePink.gameObject.SetActive(false);
        flameBlue.gameObject.SetActive(false);

        // Gestion de la barre rouge (0 � 0.33)
        if (sliderValue <= 0.33f)
        {
            flameRed.gameObject.SetActive(true);
            flameRed.fillAmount = sliderValue / 0.33f; // Proportionnel � sa plage
        }
        // Gestion de la barre rose (0.33 � 0.66)
        else if (sliderValue > 0.33f && sliderValue <= 0.66f)
        {
            flamePink.gameObject.SetActive(true);
            flamePink.fillAmount = (sliderValue - 0.33f) / 0.33f; // Proportionnel � sa plage
        }
        // Gestion de la barre bleue (0.66 � 1)
        else if (sliderValue > 0.66f)
        {
            flameBlue.gameObject.SetActive(true);
            flameBlue.fillAmount = (sliderValue - 0.66f) / 0.34f; // Proportionnel � sa plage
        }
    }
}
