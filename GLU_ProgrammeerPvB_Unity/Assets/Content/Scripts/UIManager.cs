using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Action OnStartButtonPressed;
    public Action OnRetryGameButtonPressed;
    public Action OnExitToMainMenuButtonPressed;


    public void InvokeOnStartButtonPressed()
    {
        OnStartButtonPressed?.Invoke();

    }

    public void InvokeOnRetryGameButtonPressed()
    {
        OnRetryGameButtonPressed?.Invoke();
    }
    
    public void InvokeOnExitToMainMenuButtonPressed()
    {
        OnExitToMainMenuButtonPressed?.Invoke();
    }



    public void OnShowControlsScreenButtonPressed()
    {

    }

    public void OnHideControlsScreenButtonPressed()
    {

    }

}