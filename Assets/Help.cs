using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Help : Singleton<Help>
{
    public bool Dismissed, DismissedOnce;
    public GameObject ToggleContainer;
    public GameObject NonToggleContainer;
    public GameObject Close;
    public Camera Camera;
    public Vector3 ButtonDist = new Vector3(1, 1, 1);

    void Update()
    {
        transform.position = Camera.ViewportToWorldPoint(ButtonDist);
        NonToggleContainer.SetActive(Dismissed);
        ToggleContainer.SetActive(!Dismissed || !DismissedOnce);
        Close.SetActive(!NonToggleContainer.activeInHierarchy);

        if(Time.time > 10)
		{
            DismissedOnce = true;
        }
    }

    public void OnHelp(InputAction.CallbackContext context)
	{
        Dismissed = context.canceled;
        DismissedOnce |= !Dismissed;
	}
}
