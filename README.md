#Unity dependency injection

Basic dependency injection with Unity support.

## Prerequisites

 - System.Threading DLL

## Usage example

Let's say we have an abstract definition of what a game clock should look like:

    public interface IGameClock {
        float DeltaTime { get; }
        float TimeSinceStartup { get; }
        int FrameCount { get; }
    }

We have one concrete instance of that definition:

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

And a Unity script with a dependency (notice the DependencyInfo attribute on the setter):

    public class Something : MonoBehaviour {
        [SerializeField] private IGameClock _clock;
        
        void Update() {
            Debug.Log("delta time: " + _clock.deltaTime);
        } 

        // The name is optional, if no name is specified any object
        // that conforms to the IGameClock interface can be injected
        [DependencyInfo(name: "gameClock")]
        public IGameClock Clock {
            set { _clock = value; }
        }
    }

We can then initialize a GameObject with its dependencies like this:

    var go = new GameObject();
    go.AddComponent<Something>();
    var diContainer = new DependencyContainer(new Dictionary<string, object> {
        {"gameClock", new UnityClock() }    
    });
    UnityDependencyInjection.InjectAll(go, diContainer);

And that's all there is to it.

