using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DITest;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour {

    [SerializeField] private IClock _clock;

    void FixedUpdate() {
        Debug.Log("fixed update: " + _clock.DeltaTime);
    }

    [DependencyInfo("fixedClock")]
    public IClock Clock {
        set { _clock = value; }
    }
}
