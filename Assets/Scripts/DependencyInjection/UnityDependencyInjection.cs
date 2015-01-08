using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RamjetAnvil.DITest;
using UnityEngine;

namespace RamjetAnvil.DependencyInjection
{
    public static class UnityDependencyInjection
    {
        public static void InjectAll(GameObject gameObject, DependencyContainer diContainer) {
            var components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++) {
                var component = components[i];
                Debug.Log(component);
                DependencyInjection.InjectAll(component, diContainer);    
            }
        }
    }
}
