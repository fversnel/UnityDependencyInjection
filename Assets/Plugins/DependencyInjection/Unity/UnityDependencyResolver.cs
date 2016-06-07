using System.Linq;
using RamjetAnvil.DependencyInjection;
using RamjetAnvil.DependencyInjection.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class UnityDependencyResolver : MonoBehaviour {

    private DependencyContainer _dependencyContainer;

    void Awake() {
        Resolve();
    }

    public DependencyContainer DependencyContainer {
        get { return _dependencyContainer; }
    }
    
    public void Resolve() {
        _dependencyContainer = FindDependencies();
        var rootSceneObjects = Enumerable.Range(0, SceneManager.sceneCount)
            .Select(sceneIndex => SceneManager.GetSceneAt(sceneIndex))
            .Where(scene => scene.isLoaded)
            .SelectMany(scene => scene.GetRootGameObjects());
        foreach (var sceneObject in rootSceneObjects) {
            DependencyInjection.Inject(sceneObject, _dependencyContainer, overrideExisting: false, traverseHierarchy: true);    
        }
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
