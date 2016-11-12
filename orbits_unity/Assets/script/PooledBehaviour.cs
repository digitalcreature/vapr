using UnityEngine;
using System.Collections.Generic;

public abstract class PooledBehaviour<T> : MonoBehaviour where T : PooledBehaviour<T> {

	static Stack<T> pool;
	public static T Allocate(string name = "[pooled object]") {
		if (pool == null) pool = new Stack<T>();
		T obj;
		if (pool.Count == 0) {
			obj = new GameObject(name).AddComponent<T>();
		}
		else {
			obj = pool.Pop();
			obj.name = name;
		}
		obj.gameObject.SetActive(true);
		obj.OnAllocate();
		return obj;
	}

	public static void Free(T obj) {
		if (pool == null) pool = new Stack<T>();
		if (obj != null) {
			obj.OnFree();
			obj.gameObject.SetActive(false);
			pool.Push(obj);
		}
	}

	protected virtual void OnAllocate() {}
	protected virtual void OnFree() {}

}
