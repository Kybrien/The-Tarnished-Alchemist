using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SynchronizedSlider : MonoBehaviour
{
    public Slider slider;          // Le slider principal
    public Image flameRed;         // Barre rouge (0 - 0.33)
    public Image flamePink;        // Barre rose (0.33 - 0.66)
    public Image flameBlue;        // Barre bleue (0.66 - 1)

    public GameObject fireRed;
    public GameObject firePink;
    public GameObject fireBlue;

    public TextMeshProUGUI temperatureText; // Texte pour afficher la température

    private float segment1End = 0.33f; // Fin du segment rouge
    private float segment2End = 0.66f; // Fin du segment rose

    public Transform sliderHandle;     // Transform du Handle pour détection
    public float rotationSensitivity = 0.1f; // Sensibilité pour ajuster le slider

    private bool isInteracting = false; // Indique si le joueur manipule le slider
    private Transform playerCamera;     // La caméra du joueur pour capter les mouvements
    private float initialYaw;           // Rotation initiale de la caméra (Yaw)

    void Start()
    {
        UpdateBar(); // Mise à jour initiale
        playerCamera = Camera.main.transform; // Récupère la caméra principale
    }

    public void OnSliderValueChanged()
    {
        UpdateBar();
    }

    void UpdateBar()
    {
        float sliderValue = slider.value;
        ResetBars();

        // Température calculée
        int temperature = Mathf.RoundToInt(sliderValue * 300); // Exemple : 0 → 300°C
        temperatureText.text = $"{temperature}°C";

        // Gestion de la barre rouge (0 à 0.33)
        if (sliderValue <= segment1End && sliderValue != 0)
        {
            flameRed.gameObject.SetActive(true);
            fireRed.gameObject.SetActive(true);
            fireBlue.gameObject.SetActive(false);
            firePink.gameObject.SetActive(false);
            flameRed.fillAmount = sliderValue; // Synchrone avec la valeur globale

            // Couleur du texte en rouge
            temperatureText.color = Color.red;
        }
        // Gestion de la barre rose (0.33 à 0.66)
        else if (sliderValue > segment1End && sliderValue <= segment2End)
        {
            flamePink.gameObject.SetActive(true);
            fireBlue.gameObject.SetActive(false);
            fireRed.gameObject.SetActive(false);
            firePink.gameObject.SetActive(true);
            flamePink.fillAmount = sliderValue; // Synchrone avec la valeur globale

            // Couleur du texte en rose
            temperatureText.color = new Color(1f, 0.41f, 0.71f); // RGB pour le rose
        }
        // Gestion de la barre bleue (0.66 à 1)
        else if (sliderValue > segment2End)
        {
            flameBlue.gameObject.SetActive(true);
            fireRed.gameObject.SetActive(false);
            firePink.gameObject.SetActive(false);
            fireBlue.gameObject.SetActive(true);
            flameBlue.fillAmount = sliderValue; // Synchrone avec la valeur globale

            // Couleur du texte en bleu
            temperatureText.color = new Color(0f, 0.59f, 1f);
        }
    }

    void Update()
    {
        HandleSliderInteraction(); // Ajout pour gérer l'interaction
    }

    void ResetBars()
    {
        flameRed.fillAmount = 0;
        flamePink.fillAmount = 0;
        flameBlue.fillAmount = 0;

        flameRed.gameObject.SetActive(false);
        flamePink.gameObject.SetActive(false);
        flameBlue.gameObject.SetActive(false);
    }

    void HandleSliderInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.transform == sliderHandle)
                {
                    isInteracting = true;               // Active l'interaction
                    initialYaw = playerCamera.eulerAngles.y; // Sauvegarde l'angle initial
                }
            }
        }

        if (isInteracting)
        {
            float currentYaw = playerCamera.eulerAngles.y;       // Récupère la rotation actuelle
            float deltaYaw = Mathf.DeltaAngle(initialYaw, currentYaw); // Différence d'angle
            slider.value += deltaYaw * rotationSensitivity * Time.deltaTime; // Ajuste la valeur du slider
            slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue); // Limite la valeur
            initialYaw = currentYaw; // Met à jour la position de référence
        }

        if (Input.GetMouseButtonUp(0))
        {
            isInteracting = false;
        }
    }
}
