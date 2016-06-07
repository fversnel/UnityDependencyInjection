using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DITest;

public class GameObjectFactory : MonoBehaviour {

    [SerializeField] private GameObject _playerPrefab;

    public GameObject CreatePlayer(UnityClock gameClock, UnityFixedClock fixedClock) {
        var go = (GameObject)Instantiate(_playerPrefab);

        var diContainer = new DependencyContainer(new Dictionary<string, object> {
            {"gameClock", gameClock},
            {"fixedClock", fixedClock}
        });

        DependencyInjection.Inject(go, diContainer);

        return go;
    }

}
