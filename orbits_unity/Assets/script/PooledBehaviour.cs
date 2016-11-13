using UnityEngine;
using System.Collections.Generic;

public abstract class PooledBehaviour<T> : MonoBehaviour where T : PooledBehaviour<T> {

	static Stack<T> inactive;

	static void CheckPools() {
		if (inactive == null) inactive = new Stack<T>();
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
		return obj;
	}

	public static void Free(T obj) {
		CheckPools();
		if (obj != null) {
			obj.OnFree();
			obj.gameObject.SetActive(false);
			inactive.Push(obj);
		}
	}

	protected virtual void OnAllocate() {}
	protected virtual void OnFree() {}

}
