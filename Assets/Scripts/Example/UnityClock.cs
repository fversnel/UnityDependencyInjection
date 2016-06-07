using UnityEngine;

namespace RamjetAnvil.DITest {
    public class UnityClock : MonoBehaviour, IClock {

        public float DeltaTime {
            get { return Time.deltaTime; }
        }
    }
}
