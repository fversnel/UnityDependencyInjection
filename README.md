# Unity dependency injection

Basic dependency injection with Unity support.

## Features

 - Dependency injection for (but not limited to) Unity components and game objects.

## Usage example

Let's say we have an abstract definition of what a game clock should look like:

``` c#
public interface IGameClock {
    float DeltaTime { get; }
    float TimeSinceStartup { get; }
    int FrameCount { get; }
}
```

We have one concrete instance of that definition:

``` c#
public class UnityClock {
    public float DeltaTime {
        get { return Time.deltaTime; }
    }

    public float TimeSinceStartup {
        get { return Time.time; }
    }

    public int FrameCount {
        get { return Time.frameCount; }
    }
}
```

And a Unity script with a dependency (notice the DependencyInfo attribute on the setter):

``` c#
public class Something : MonoBehaviour {
    [Dependency("gameClock"), SerializeField] private IGameClock _clock;
    
    void Update() {
        Debug.Log("delta time: " + _clock.deltaTime);
    } 

    // Or you can inject through a setter
    [Dependency("gameClock")]
    public IGameClock Clock {
        set { _clock = value; }
    }
}
```

We can then initialize a GameObject with its dependencies like this:

``` c#
var go = new GameObject();
go.AddComponent<Something>();
var diContainer = new DependencyContainer(new Dictionary<string, object> {
    {"gameClock", new UnityClock() }    
});
DependencyInjection.Inject(go, diContainer);
```

And that's all there is to it.

