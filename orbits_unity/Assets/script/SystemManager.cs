using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SystemManager : SingletonBehaviour<SystemManager> {

	public string query = "{}";
	public int limit = 1;
	public int callbacksPerFrame = 5;
	public int updatesPerStep = 50;
	public float daysPerSecond = 1;

	void Awake() {
		HashSet<Body> bodies = new HashSet<Body>();
		AsterankUtil.Query(query, limit, callbacksPerFrame,
		(data) => {		// data callback
			bodies.Add(Body.FromAsterankData(data));
		}, () => {		// finish callback
			StartCoroutine(UpdateRoutine(bodies));
		});
	}

	IEnumerator UpdateRoutine(HashSet<Body> bodies) {
		double lastTime = Time.time;
		for (;;) {
			int i = 0;
			bool hasYielded = false;
			foreach (Body body in bodies) {
				body.Step();
				if (i == updatesPerStep) {
					i = 0;
					hasYielded = true;
					yield return null;
					Orbit.epoch += (Time.time - lastTime) * daysPerSecond;
					lastTime = Time.time;
				}
				else {
					i ++;
				}
			}
			if (!hasYielded) {
				yield return null;
				Orbit.epoch += (Time.time - lastTime) * daysPerSecond;
				lastTime = Time.time;
			}
		}
	}

}
