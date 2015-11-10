using UnityEngine;
using System.Collections;
using RamjetAnvil.DITest;

public class Main : MonoBehaviour {

    [SerializeField] private GameObjectFactory _goFactory;

    void Awake() {
        var gameClock = new UnityClock();
        var fixedClock = new UnityFixedClock();

        _goFactory.CreatePlayer(gameClock, fixedClock);
    }

}
