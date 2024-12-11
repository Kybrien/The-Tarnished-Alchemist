using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOutlineScript : MonoBehaviour
{
    public Camera playerCamera;
    public float lookRange = 5f; // Distance maximale pour détecter les objets
    public LayerMask interactableLayer; // Layer pour les objets interactables

    private GameObject currentTarget; // Objet actuellement visé
    private Material originalMaterial; // Matériau d'origine
    public Material outlineMaterial; // Matériau pour l'effet Outline

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        // Raycast à partir de la caméra du joueur
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, lookRange, interactableLayer))
        {
            GameObject target = hit.collider.gameObject;

            // Si on change de cible
            if (target != currentTarget)
            {
                ResetOutline(); // Réinitialiser l'outline de l'ancien objet
                ApplyOutline(target); // Appliquer l'outline sur le nouvel objet
                currentTarget = target;
            }
        }
        else
        {
            ResetOutline(); // Réinitialiser l'outline si aucun objet interactable n'est détecté
            currentTarget = null;
        }
    }

    void ApplyOutline(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material; // Sauvegarder le matériau actuel
            renderer.material = outlineMaterial; // Appliquer le matériau Outline
        }
    }

    void ResetOutline()
    {
        if (currentTarget != null)
        {
            Renderer renderer = currentTarget.GetComponent<Renderer>();
            if (renderer != null && originalMaterial != null)
            {
                renderer.material = originalMaterial; // Restaurer le matériau d'origine
            }
        }
    }
}

