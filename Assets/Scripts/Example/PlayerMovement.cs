using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DITest;

public class PlayerMovement : MonoBehaviour {

    private IClock _clock;

    void Update() {
        Debug.Log("update: " + _clock.DeltaTime);
    }

    [DependencyInfo("gameClock")]
    public IClock Clock {
        set { _clock = value; }
    }
}
