using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {
	public static T Instance { get; private set; }
	
	protected void SingletonSetup() {
		if (Instance != null) {
            Debug.LogWarning("Attempted to instantiate second instance of Singleton " + typeof(T).ToString());
			Destroy(gameObject);
        	return;
    	}
		
		//DontDestroyOnLoad(gameObject);
        Instance = GetComponent<T>();
	}
	
	protected virtual void Awake() {
		SingletonSetup();
	}
}
