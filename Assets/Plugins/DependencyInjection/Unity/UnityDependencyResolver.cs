using System.Collections;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DependencyInjection.Unity;
using UnityEngine;

[ExecuteInEditMode]
public class UnityDependencyResolver : MonoBehaviour {

    [SerializeField] private float _updateIntervalInS = 60f;

    private DependencyContainer _dependencyContainer;
    private float _lastUpdated;

    void Awake() {
        Resolve();
    }

#if UNITY_EDITOR
    void Update() {
        if (_lastUpdated + _updateIntervalInS < Time.realtimeSinceStartup) {
            Debug.Log("dingon");
            Resolve();    
        }
    }
#endif

    public DependencyContainer DependencyContainer {
        get { return _dependencyContainer; }
    }

    
    public void Resolve() {
        _dependencyContainer = FindDependencies();
        var sceneObjects = FindObjectsOfType<GameObject>();
        foreach (var sceneObject in sceneObjects) {
            DependencyInjection.Inject(sceneObject, _dependencyContainer, overrideExisting: false, traverseHierarchy: false);    
        }
        _lastUpdated = Time.realtimeSinceStartup;
    }

    private static DependencyContainer FindDependencies() {
        var dependencies = FindObjectsOfType<IsDependency>();
        var dependencyContainer = new DependencyContainer();
        foreach (var dependency in dependencies) {
            if (dependency.Reference != null) {
                var dependencyName = dependency.Name == "" ? dependency.Reference.gameObject.name : dependency.Name;
                dependencyContainer.AddDependency(dependencyName, dependency.Reference);
            }
        }
        return dependencyContainer;
    }

}
