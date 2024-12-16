using UnityEngine;
using UnityEngine.UI;

public class SliderFlameController : MonoBehaviour
{
    public Slider slider; // Le slider principal
    public Image flame1;  // Image de Flame1
    public Image flame2;  // Image de Flame2
    public Image flame3;  // Image de Flame3

    void Start()
    {
        UpdateFlame(); // Met à jour la flamme initiale
    }

    public void OnSliderValueChanged()
    {
        UpdateFlame();
    }

    void UpdateFlame()
    {
        // Récupère la valeur du slider
        float sliderValue = slider.value;

        // Assure que le slider a bien une Image de FillRect
        Image fillImage = slider.fillRect.GetComponent<Image>();
        if (fillImage == null)
        {
            Debug.LogError("FillRect does not have an Image component!");
            return;
        }

        // Désactive toutes les flammes
        flame1.enabled = false;
        flame2.enabled = false;
        flame3.enabled = false;

        // Active la flamme et configure le FillRect en fonction de la valeur du slider
        if (sliderValue <= 0.33f)
        {
            flame1.enabled = true;
            fillImage.sprite = flame1.sprite; // Assignation du sprite de Flame1
        }
        else if (sliderValue > 0.33f && sliderValue <= 0.66f)
        {
            flame2.enabled = true;
            fillImage.sprite = flame2.sprite; // Assignation du sprite de Flame2
        }
        else if (sliderValue > 0.66f)
        {
            flame3.enabled = true;
            fillImage.sprite = flame3.sprite; // Assignation du sprite de Flame3
        }

        // Assure que l'image du FillRect est affichée correctement
        fillImage.enabled = true;
    }
}
