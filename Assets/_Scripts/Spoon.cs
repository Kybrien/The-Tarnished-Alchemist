using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spoon : MonoBehaviour
{
    public GameObject FX1; // Effet à activer
    public GameObject FX2; // Effet à activer
    public Animator spoonAnimator; // Animator pour l'animation Cooking
    public Mixer mixer; // Référence au script Mixer

    private bool isCooking = false; // Vérifie si l'animation est en cours

    // Fonction appelée lorsqu'on utilise la cuillère
    public void UseSpoon()
    {
        if (isCooking)
        {
            return; // Empêche de relancer la fonction si l'animation est en cours
        }

        if (spoonAnimator == null || FX1 == null || mixer == null)
        {
            Debug.LogError("Assurez-vous d'avoir assigné l'Animator, FX1 et le Mixer dans l'inspecteur.");
            return;
        }
        StartCoroutine(CookingRoutine());
    }

    private IEnumerator CookingRoutine()
    {
        isCooking = true;

        // Lance l'animation
        spoonAnimator.SetTrigger("Cooking");

        // Active l'effet
        if (FX1 != null)
        {
            FX1.SetActive(false); // Désactive d'abord pour éviter tout état précédent
            FX1.SetActive(true);  // Force l'activation
        }

        yield return new WaitForSeconds(2.65f);

        // Désactive l'effet
        FX1.gameObject.SetActive(false);
        FX2.gameObject.SetActive(true);

        // Récupère les objets dans le conteneur
        List<string> elements = CheckAndClearMixer();
        Debug.Log("Éléments récupérés : " + string.Join(", ", elements));
        yield return new WaitForSeconds(0.92f);

        isCooking = false;
        FX2.gameObject.SetActive(false);
    }

    // Vérifie les éléments dans le Mixer et les détruit
    private List<string> CheckAndClearMixer()
    {
        List<string> elements = new List<string>();

        // Vérifie chaque position
        foreach (Transform pos in new Transform[] { mixer.Pos1, mixer.Pos2, mixer.Pos3 })
        {
            if (pos.childCount > 0)
            {
                // Récupère le nom de l'objet
                GameObject child = pos.GetChild(0).gameObject;
                elements.Add(child.name);

                // Détruit l'objet
                Destroy(child);
            }
        }

        return elements;
    }
}
