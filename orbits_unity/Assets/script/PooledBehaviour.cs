using UnityEngine;
using System.Collections.Generic;

public abstract class PooledBehaviour<T> : MonoBehaviour where T : PooledBehaviour<T> {

	static Stack<T> inactive;
	public static HashSet<T> active { get; private set; }

	static void CheckPools() {
		if (inactive == null) inactive = new Stack<T>();
		if (active == null) active = new HashSet<T>();
	}

	public static T Allocate(string name = "[pooled object]") {
		CheckPools();
		T obj;
		if (inactive.Count == 0) {
			obj = new GameObject(name).AddComponent<T>();
		}
		else {
			obj = inactive.Pop();
			obj.name = name;
		}
		obj.gameObject.SetActive(true);
		obj.OnAllocate();
		active.Add(obj);
		return obj;
	}

	public static void Free(T obj) {
		CheckPools();
		if (obj != null) {
			obj.OnFree();
			obj.gameObject.SetActive(false);
			active.Remove(obj);
			inactive.Push(obj);
		}
	}

	protected virtual void OnAllocate() {}
	protected virtual void OnFree() {}

}
