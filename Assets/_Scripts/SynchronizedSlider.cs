using UnityEngine;
using UnityEngine.UI;

public class SynchronizedSlider : MonoBehaviour
{
    public Slider slider;          // Le slider principal
    public Image flameRed;         // Barre rouge (0 - 0.33)
    public Image flamePink;        // Barre rose (0.33 - 0.66)
    public Image flameBlue;        // Barre bleue (0.66 - 1)

    private float segment1End = 0.33f; // Fin du segment rouge
    private float segment2End = 0.66f; // Fin du segment rose

    void Start()
    {
        UpdateBar(); // Mise à jour initiale
    }

    public void OnSliderValueChanged()
    {
        UpdateBar();
    }

    void UpdateBar()
    {
        float sliderValue = slider.value;

        // Réinitialisation des barres
        ResetBars();

        // Gestion de la barre rouge (0 à 0.33)
        if (sliderValue <= segment1End)
        {
            flameRed.gameObject.SetActive(true);
            flameRed.fillAmount = sliderValue; // Synchrone avec la valeur globale
        }

        // Gestion de la barre rose (0.33 à 0.66)
        else if (sliderValue > segment1End && sliderValue <= segment2End)
        {
            flamePink.gameObject.SetActive(true);
            flamePink.fillAmount = sliderValue; // Synchrone avec la valeur globale
        }

        // Gestion de la barre bleue (0.66 à 1)
        else if (sliderValue > segment2End)
        {
            flameBlue.gameObject.SetActive(true);
            flameBlue.fillAmount = sliderValue; // Synchrone avec la valeur globale
        }
    }

    void ResetBars()
    {
        // Remet les barres à 0 mais garde leur synchronisation
        flameRed.fillAmount = 0;
        flamePink.fillAmount = 0;
        flameBlue.fillAmount = 0;

        flameRed.gameObject.SetActive(false);
        flamePink.gameObject.SetActive(false);
        flameBlue.gameObject.SetActive(false);
    }
}
