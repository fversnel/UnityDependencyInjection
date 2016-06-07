using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RamjetAnvil.Util;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RamjetAnvil.DependencyInjection {

    // TODO Remove overrideExisting bool, it's part of the injector
    public delegate bool DependencyInjector<in TContext>(
        object subject, InjectionPoint injectionPoint, TContext context, bool overrideExisting);

    public static class DependencyInjection {

        private static readonly Func<Type, IList<InjectionPoint>> GetInjectionPoints =
            Memoization.Memoize<Type, IList<InjectionPoint>>(GetInjectionPointsInternal);

        private static IList<InjectionPoint> GetInjectionPointsInternal(Type type) {
            var injectionPoints = new List<InjectionPoint>();
            var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.Instance | BindingFlags.SetProperty |
                                                BindingFlags.DeclaredOnly);
            for (int i = 0; i < properties.Length; i++) {
                var propertyInfo = properties[i];
                var dependencyAttributes = propertyInfo.GetCustomAttributes(typeof (Dependency), inherit: true);
                if (dependencyAttributes.Length > 0) {
                    var dependencyInfo = (Dependency) dependencyAttributes.First();
                    var propertyMember = new PropertyMember(propertyInfo);
                    if (propertyMember.HasSetter && propertyMember.HasGetter) {
                        injectionPoints.Add(new InjectionPoint(dependencyInfo, propertyMember));
                    } else {
                        throw new Exception("Dependency properties must have a getter and a setter: " + propertyMember + " on " + type);
                    }
                }
            }
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public |
                                        BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            for (int i = 0; i < fields.Length; i++) {
                var fieldInfo = fields[i];
                var dependencyAttributes = fieldInfo.GetCustomAttributes(typeof (Dependency), inherit: true);
                if (dependencyAttributes.Length > 0) {
                    var dependencyInfo = (Dependency) dependencyAttributes.First();
                    injectionPoints.Add(new InjectionPoint(dependencyInfo, new FieldMember(fieldInfo)));
                }
            }

            return injectionPoints;
        }

        private static bool InjectInternal(this InjectionPoint injectionPoint, object subject, DependencyReference dependency,
            bool overrideExisting) {

            var isNameMatch = injectionPoint.Info.Name == null ||
                              injectionPoint.Info.Name.Equals(dependency.Name);
            var isTypeMatch = injectionPoint.Injector.Type.IsInstanceOfType(dependency.Instance)
                && injectionPoint.Injector.Info.DeclaringType.IsInstanceOfType(subject);
            var currentValue = injectionPoint.Injector.GetValue(subject);
            bool isDependencyAlreadySet;
            if (currentValue is UnityEngine.Object) {
                isDependencyAlreadySet = (currentValue as UnityEngine.Object) != null;
            } else {
                isDependencyAlreadySet = currentValue != null;
            }

            if (isNameMatch && isTypeMatch && (!isDependencyAlreadySet || overrideExisting)) {
                injectionPoint.Injector.SetValue(subject, dependency.Instance);
                return true;
            }
            return false;
        }

        private static readonly List<MonoBehaviour> ComponentCache = new List<MonoBehaviour>();
        public static void Inject<TContext>(object subject, 
            DependencyInjector<TContext> inject, 
            TContext context,
            bool overrideExisting,
            bool traverseHierarchy) {

            if (subject == null) {
                throw new ArgumentNullException("Cannot inject on a null component/object");
            }

            if (subject is GameObject) {
                var gameObject = subject as GameObject;
#if UNITY_EDITOR
                if (IsPrefab(gameObject)) {
                    throw new ArgumentException("Injecting on Prefab '" + gameObject.name + "' is not allowed.");
                }
#endif

                ComponentCache.Clear();
                if (traverseHierarchy) {
                    gameObject.GetComponentsInChildren<MonoBehaviour>(includeInactive: true, results: ComponentCache);
                } else {
                    gameObject.GetComponents<MonoBehaviour>(results: ComponentCache);
                }
                //Debug.Log("Injection on " + gameObject.name + ", components " + ComponentCache.Join(","));
                for (int i = 0; i < ComponentCache.Count; i++) {
                    var component = ComponentCache[i];
                    //Debug.Log("injection into " + component);
                    Inject(component, inject, context, overrideExisting, traverseHierarchy);
                }
            } else {
                // TODO Do this for all super types
                var injectionPoints = GetInjectionPoints(subject.GetType());
                var allDependenciesResolved = true;
                var dependenciesInjected = 0;
                for (int i = 0; i < injectionPoints.Count; i++) {
                    var injectionPoint = injectionPoints[i];
                    // Search dependency for each injection point and inject it
                    var isInjectionSucceeded = inject(subject, injectionPoint, context, overrideExisting);
                    if (isInjectionSucceeded) {
                        dependenciesInjected++;
                    }
                    allDependenciesResolved = allDependenciesResolved && injectionPoint.IsDependencySet(subject);
                }
                if (allDependenciesResolved && dependenciesInjected > 0 && (subject as IOnDependenciesResolved) != null) {
                    Debug.Log("all dependencies resolved for " + subject);
                    (subject as IOnDependenciesResolved).OnDependenciesResolved();
                }
            }
        }

        public static void InjectSingle(object subject, object dependency, bool overrideExisting = false, bool traverseHierarchy = true) {
            Inject(subject, InjectSingleDependency, dependency, overrideExisting, traverseHierarchy);
        }

        public static void Inject(object subject, DependencyContainer diContainer, bool overrideExisting = false, bool traverseHierarchy = true) {
            Inject(subject, InjectFromContainer, diContainer, overrideExisting, traverseHierarchy);
        }

        public static readonly DependencyInjector<object> InjectSingleDependency =
            (subject, injectionPoint, dependency, overrideExisting) => {
                if (injectionPoint.Injector.Type.IsInstanceOfType(dependency)) {
                    return injectionPoint.InjectInternal(subject, new DependencyReference(name: null, instance: dependency), overrideExisting);
                }
                return false;
            };

        public static readonly DependencyInjector<DependencyContainer> InjectFromContainer =
            (subject, injectionPoint, diContainer, overrideExisting) => {
                bool isSomethingInjected = false;
                IList<DependencyReference> candidates;
                if (diContainer.DepsByType.TryGetValue(injectionPoint.Injector.Type, out candidates)) {
                    for (int i = 0; i < candidates.Count; i++) {
                        var dependency = candidates[i];
                        var isInjectionSucceeded = injectionPoint.InjectInternal(subject, dependency, overrideExisting);
                        if (isInjectionSucceeded) {
                            isSomethingInjected = true;
                        }
                    }
                }
                return isSomethingInjected;
            };

        public static bool IsDependencySet(this InjectionPoint injectionPoint, object subject) {
            return injectionPoint.Injector.GetValue(subject) != null;
        }

#if UNITY_EDITOR
        private static bool IsPrefab(GameObject go) {
            return PrefabUtility.GetPrefabParent(go) == null && PrefabUtility.GetPrefabObject(go) != null; // Is a prefab
        }
#endif
    }
}
