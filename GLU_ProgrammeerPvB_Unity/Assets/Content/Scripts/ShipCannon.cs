using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCannon : Cannon
{
    
    private Ship _ship;

    private void Awake()
    {
        _ship = GetComponentInParent<Ship>();

        if (!_ship.CannonSettings)
        {
            Debug.LogError("Can't find cannon settings in parent ship");
            return;
        }
        _cannonSettings = _ship.CannonSettings;
    }
}
