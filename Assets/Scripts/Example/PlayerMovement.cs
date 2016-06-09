using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DITest;

public class PlayerMovement : MonoBehaviour {

    [Dependency("gameClock"), SerializeField] private UnityClock _clock;

    void OnEnable() {
        Debug.Log("PlayerMovement dependencies resolved");
    }

    void Update() {
        Debug.Log("update: " + _clock.DeltaTime);
    }
}
