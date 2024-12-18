using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spoon : MonoBehaviour
{
    public GameObject FX1; // Effet à activer
    public Animator spoonAnimator; // Animator pour l'animation Cooking
    public Mixer mixer; // Référence au script Mixer

    private bool isCooking = false; // Vérifie si l'animation est en cours

    // Fonction appelée lorsqu'on utilise la cuillère
    public void UseSpoon()
    {
        if (isCooking)
        {
            Debug.Log("L'animation est déjà en cours.");
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
        FX1.SetActive(true);

        // Attend la fin de l'animation
        AnimatorStateInfo animationState = spoonAnimator.GetCurrentAnimatorStateInfo(0);
        while (animationState.IsName("Cooking") && animationState.normalizedTime < 1.0f)
        {
            animationState = spoonAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        // Désactive l'effet
        FX1.SetActive(false);

        // Récupère les objets dans le conteneur
        List<string> elements = CheckAndClearMixer();
        Debug.Log("Éléments récupérés : " + string.Join(", ", elements));

        isCooking = false;
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
