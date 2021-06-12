using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Help : MonoBehaviour
{
    public bool Dismissed, DismissedOnce;
    public GameObject ToggleContainer;
    public GameObject NonToggleContainer;
    public Camera Camera;
    public Vector3 ButtonDist = new Vector3(1, 1, 1);

    void Update()
    {
        transform.position = Camera.ViewportToWorldPoint(ButtonDist);
        NonToggleContainer.SetActive(Dismissed);
        ToggleContainer.SetActive(!Dismissed || !DismissedOnce);
    }

    public void OnHelp(InputAction.CallbackContext context)
	{
        Dismissed = context.canceled;
        DismissedOnce |= !Dismissed;
	}
}
