using RamjetAnvil.DependencyInjection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityDependencyResolver))]
public class UnityDependencyResolverEditor : Editor {

    public override void OnInspectorGUI() {
        var instance = target as UnityDependencyResolver;
        if (instance != null && instance.DependencyContainer != null) {
            foreach (var kvPair in instance.DependencyContainer.DepsByString) {
                var dependencyName = kvPair.Key;
                var dependency = kvPair.Value.Instance;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(dependencyName);
                if (dependency is UnityEngine.Object) {
                    EditorGUILayout.ObjectField(dependency as UnityEngine.Object, dependency.GetType(), true);
                } else {
                    EditorGUILayout.LabelField(dependency.ToString());
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (GUILayout.Button("Resolve dependencies") && instance != null) {
            instance.Resolve();
        }
    }
}
