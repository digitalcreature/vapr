using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OrbitQueryTester : SingletonBehaviour<OrbitQueryTester> {

	public string query = "{}";
	public int limit = 1;
	public int updatesPerStep = 50;
	public float daysPerSecond = 1;

	void Awake() {
		HashSet<Asteroid> asteroids = new HashSet<Asteroid>();
		Asterank.Query(query, limit, (data) => {
			asteroids.Add(Asteroid.FromAsterankData(data));
		}, () => {
			StartCoroutine(UpdateRoutine(asteroids));
		});
	}

	IEnumerator UpdateRoutine(HashSet<Asteroid> asteroids) {
		double lastTime = Time.time;
		for (;;) {
			int i = 0;
			bool hasYielded = false;
			foreach (Asteroid asteroid in asteroids) {
				asteroid.Step();
				if (i == updatesPerStep) {
					i = 0;
					hasYielded = true;
					yield return null;
					OrbitalElements.epoch += (Time.time - lastTime) * daysPerSecond;
					lastTime = Time.time;
				}
				else {
					i ++;
				}
			}
			if (!hasYielded) {
				yield return null;
				OrbitalElements.epoch += (Time.time - lastTime) * daysPerSecond;
				lastTime = Time.time;
			}
		}
	}

}
