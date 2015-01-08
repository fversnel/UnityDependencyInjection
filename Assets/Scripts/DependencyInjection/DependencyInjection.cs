using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RamjetAnvil.DITest;
using RamjetAnvil.Util;
using UnityEngine;

namespace RamjetAnvil.DependencyInjection
{
    public static class DependencyInjection {

        public static readonly Func<Type, IEnumerable<InjectionPoint>> GetInjectionPoints =
            Memoization.Memoize<Type, IEnumerable<InjectionPoint>>(GetInjectionPointsInternal);

        private static IEnumerable<InjectionPoint> GetInjectionPointsInternal(Type type) {
            var injectionPoints = new List<InjectionPoint>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)) {
                var dependencyAttributes = propertyInfo.GetCustomAttributes(typeof (DependencyInfo), true);
                if (dependencyAttributes.Length > 0) {
                    var dependencyInfo = (DependencyInfo)dependencyAttributes.First();
                    injectionPoints.Add(new InjectionPoint(dependencyInfo, propertyInfo.GetSetMethod()));
                }
            }
            return injectionPoints;
        }

        public static void Inject(this InjectionPoint injectionPoint, object subject, Dependency dependency) {
            var isNameMatch = injectionPoint.Info.Name == null ||
                              injectionPoint.Info.Name.Equals(dependency.Name);
            var isTypeMatch = injectionPoint.InjectableType().IsInstanceOfType(dependency.Instance)
                && injectionPoint.Injector.DeclaringType.IsInstanceOfType(subject);

            if (isNameMatch && isTypeMatch) {
                injectionPoint.Injector.Invoke(subject, new[] { dependency.Instance });
            }
        }

        private static Type InjectableType(this InjectionPoint injectionPoint) {
            return injectionPoint.Injector.GetParameters()[0].ParameterType;
        }

        public static void InjectAll(object subject, DependencyContainer diContainer) {
            var injectionPoints = GetInjectionPoints(subject.GetType());
            foreach (var injectionPoint in injectionPoints) {
                IList<Dependency> candidates;
                // Search dependency for each injection point and inject it
                if (diContainer.DepsByType.TryGetValue(injectionPoint.InjectableType(), out candidates)) {
                    for (int i = 0; i < candidates.Count; i++) {
                        injectionPoint.Inject(subject, dependency: candidates[i]);
                    }
                }
            }
        }
    }
}
