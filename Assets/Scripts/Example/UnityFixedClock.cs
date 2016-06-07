using UnityEngine;

namespace RamjetAnvil.DITest {

    public class UnityFixedClock : MonoBehaviour, IClock {

        public float DeltaTime {
            get { return Time.fixedDeltaTime; }
        }
    }
}
