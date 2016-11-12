using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {

	private static T _instance;
	public static T instance {
		get {
			if (_instance == null) {
				_instance = Object.FindObjectOfType<T>();
				if (_instance == null) {
					_instance = new GameObject(typeof(T).Name).AddComponent<T>();
				}
			}
			return _instance;
		}
	}

}
