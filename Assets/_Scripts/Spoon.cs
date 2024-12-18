using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spoon : MonoBehaviour
{
    public GameObject FX1; // Effet � activer
    public Animator spoonAnimator; // Animator pour l'animation Cooking
    public Mixer mixer; // R�f�rence au script Mixer

    private bool isCooking = false; // V�rifie si l'animation est en cours

    // Fonction appel�e lorsqu'on utilise la cuill�re
    public void UseSpoon()
    {
        if (isCooking)
        {
            Debug.Log("L'animation est d�j� en cours.");
            return; // Emp�che de relancer la fonction si l'animation est en cours
        }

        if (spoonAnimator == null || FX1 == null || mixer == null)
        {
            Debug.LogError("Assurez-vous d'avoir assign� l'Animator, FX1 et le Mixer dans l'inspecteur.");
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

        // D�sactive l'effet
        FX1.SetActive(false);

        // R�cup�re les objets dans le conteneur
        List<string> elements = CheckAndClearMixer();
        Debug.Log("�l�ments r�cup�r�s : " + string.Join(", ", elements));

        isCooking = false;
    }

    // V�rifie les �l�ments dans le Mixer et les d�truit
    private List<string> CheckAndClearMixer()
    {
        List<string> elements = new List<string>();

        // V�rifie chaque position
        foreach (Transform pos in new Transform[] { mixer.Pos1, mixer.Pos2, mixer.Pos3 })
        {
            if (pos.childCount > 0)
            {
                // R�cup�re le nom de l'objet
                GameObject child = pos.GetChild(0).gameObject;
                elements.Add(child.name);

                // D�truit l'objet
                Destroy(child);
            }
        }

        return elements;
    }
}
