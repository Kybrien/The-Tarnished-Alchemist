using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFurnace : MonoBehaviour
{
    public Canvas CanvasFurnace;
    // Start is called before the first frame update

    private void Start()
    {
        CanvasFurnace.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        CanvasFurnace.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        CanvasFurnace.gameObject.SetActive(false);
    }
}
