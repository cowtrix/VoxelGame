using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsDisplay : MonoBehaviour
{
    public PlayerState State;
    public Text Text;

    // Update is called once per frame
    void Update()
    {
        Text.text = State.CurrentState.Credits.ToString();
    }
}
