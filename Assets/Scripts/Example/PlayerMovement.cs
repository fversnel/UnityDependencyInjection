using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DITest;

public class PlayerMovement : MonoBehaviour, IOnDependenciesResolved {

    [Dependency("gameClock"), SerializeField] private UnityClock _clock;

    void Update() {
        Debug.Log("update: " + _clock.DeltaTime);
    }

    public void OnDependenciesResolved() {
        Debug.Log("PlayerMovement dependencies resolved");
    }
}
