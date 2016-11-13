using UnityEngine;
using System.Collections.Generic;

public class DisplayManager : SingletonBehaviour<DisplayManager> {

	public float bodyRadius = 0.1f;
	public float planeAlpha = 0.025f;
	public float predefinedBodiesPlaneAlpha = 0.5f;


	void Update() {
		SystemManager sys = SystemManager.instance;
		if (Input.GetKeyDown("p")) {
			OrbitalPlane.visiblePaths = !OrbitalPlane.visiblePaths;
		}
		if (Input.GetKeyDown("space")) {
			sys.timeScale = sys.timeScale == 0 ? 1 : 0;
		}
	}

}
