using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SystemManager : SingletonBehaviour<SystemManager> {

	public string query = "{}";
	public int limit = 1;
	public int callbacksPerFrame = 5;
	public int updatesPerFrame = 50;
	public float daysPerSecond = 1;
	public List<Body.Info> predefinedBodies;

	void Awake() {
		DisplayManager display = DisplayManager.instance;
		HashSet<Body> bodies = new HashSet<Body>();
		foreach (Body.Info info in predefinedBodies) {
			if (info != null) {
				Body body = Body.FromBodyInfo(info);
				bodies.Add(body);
				body.plane.alpha = display.predefinedBodiesPlaneAlpha;
				body.plane.UpdateDisplay();
			}
		}
		AsterankUtil.Query(query, limit, callbacksPerFrame,
		(data) => {		// data callback
			Body body = Body.FromAsterankData(data);
			bodies.Add(body);
			body.radius = display.bodyRadius;
			body.plane.alpha = display.planeAlpha;
			body.UpdateDisplay();
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
				if (i == updatesPerFrame) {
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
