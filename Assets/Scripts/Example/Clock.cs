using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RamjetAnvil.DITest
{
    public interface IClock {
        float DeltaTime { get; }
    }

    [Serializable]
    public class UnityClock : IClock {

        public float DeltaTime {
            get { return Time.deltaTime; }
        }
    }

    [Serializable]
    public class UnityFixedClock : IClock {

        public float DeltaTime {
            get { return Time.fixedDeltaTime; }
        }
    }
}
