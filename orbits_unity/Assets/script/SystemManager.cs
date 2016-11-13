using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class SystemManager : SingletonBehaviour<SystemManager> {

	public QueryInfo[] queries;
	public Text queryNameText;
	public int callbacksPerFrame = 5;
	public int updatesPerFrame = 50;
	public float daysPerSecond = 1;
	public float timeScale = 1;
	public List<Body.Info> predefinedBodies;

	public HashSet<Body> bodies { get; private set; }

	int query = 0;

	void Awake() {
		DisplayManager display = DisplayManager.instance;
		bodies = new HashSet<Body>();
		foreach (Body.Info info in predefinedBodies) {
			if (info != null) {
				Body body = Body.FromBodyInfo(info);
				body.plane.alpha = display.predefinedBodiesPlaneAlpha;
				body.plane.UpdateDisplay();
			}
		}
		Query(queries[query]);
	}

	public void ClearBodies() {
		foreach (Body body in bodies) {
			Body.Free(body);
		}
		bodies.Clear();
	}

	public void Query(string query, int limit) {
		if (!AsterankUtil.busy) {
			StopCoroutine("UpdateRoutine");
			DisplayManager display = DisplayManager.instance;
			AsterankUtil.Query(query, limit, callbacksPerFrame,
			(data) => {		// data callback
				Body body = Body.FromAsterankData(data);
				bodies.Add(body);
				body.radius = display.bodyRadius;
				body.plane.alpha = display.planeAlpha;
				body.UpdateDisplay();
			}, () => {		// finish callback
				StartCoroutine("UpdateRoutine");
				});
		}
	}
	public void Query(QueryInfo info) {
		Query(info.query, info.limit);
		queryNameText.text = info.name;
	}

	IEnumerator UpdateRoutine() {
		for (;;) {
			int i = 0;
			bool hasYielded = false;
			foreach (Body body in Body.active) {
				body.Step();
				if (i == updatesPerFrame) {
					i = 0;
					hasYielded = true;
					yield return null;
				}
				else {
					i ++;
				}
			}
			if (!hasYielded) {
				yield return null;
			}
		}
	}

	void Update() {
		Orbit.epoch += (Time.deltaTime) * daysPerSecond * timeScale;
		bool right = Input.GetKeyDown("right");
		bool left = Input.GetKeyDown("left");
		if (left || right) {
			if (!AsterankUtil.busy) {
				if (left) query --;
				if (right) query ++;
				if (query < 0) query = queries.Length - 1;
				if (query >= queries.Length) query %= queries.Length;
				ClearBodies();
				Query(queries[query]);
			}
		}
	}

	[System.Serializable]
	public class QueryInfo {
		public string name;
		public string query;
		public int limit;
	}

}
