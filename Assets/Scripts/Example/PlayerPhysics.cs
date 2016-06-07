using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DITest;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour {

    [Dependency("fixedClock"), SerializeField] private UnityFixedClock _clock;

    void FixedUpdate() {
        Debug.Log("fixed update: " + _clock.DeltaTime);
    }
}
