using UnityEngine;

public class HoverCanvas : MonoBehaviour
{
    public Canvas canvas; // Le Canvas en World Space
    public Transform player; // Référence au joueur

    private bool isHovered = false;

    void Start()
    {
        if (canvas != null)
        {
            canvas.enabled = false; // Désactive le Canvas au début
        }

        if (player == null)
        {
            // Trouve le joueur automatiquement si non assigné
            player = Camera.main.transform;
        }
    }

    void Update()
    {
        if (isHovered && canvas != null)
        {
            // Oriente le Canvas vers le joueur
            Vector3 direction = (player.position - canvas.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(-direction, Vector3.up); // Inverse pour correspondre au World Space
            canvas.transform.rotation = lookRotation * Quaternion.Euler(180f, 0f, 0f); ;
        }
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        if (canvas != null)
        {
            canvas.enabled = true;
        }
    }

    public void OnHoverExit()
    {
        isHovered = false;
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}
