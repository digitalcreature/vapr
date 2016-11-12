using UnityEngine;
using System.Collections.Generic;

public class Asteroid : PooledBehaviour<Asteroid> {

	public OrbitalElements elements;

	public OrbitalPath path { get; private set; }

	protected override void OnAllocate() {
		path = OrbitalPath.Allocate(name);
		transform.parent = path.transform;
	}

	protected override void OnFree() {
		transform.parent = null;
		OrbitalPath.Free(path);
	}

	public static Asteroid FromAsterankData(Asterank.Data data) {
		Asteroid asteroid = Allocate("asteroid [" + data.full_name + "]");
		asteroid.elements.SetFromAsterankData(data);
		asteroid.path.name = "orbit [" + data.full_name + "]";
		asteroid.path.elements = asteroid.elements;
		asteroid.path.UpdateDisplay();
		asteroid.UpdatePosition();
		return asteroid;
	}

	void OnValidate() {
		if (path != null) {
			UpdatePosition();
			path.elements = elements;
			path.UpdateDisplay();
		}
	}

	public void UpdatePosition() {
		float a = elements.semimajorAxis;
		float e = elements.eccentricity;
		float cos = Mathf.Cos(-elements.trueAnomaly * Mathf.Deg2Rad);
		float sin = Mathf.Sin(-elements.trueAnomaly * Mathf.Deg2Rad);
		float r =
			(a * (1 - (e * e)))
			/ (1 + (e * cos));
		Vector3 pos = new Vector3(
			r * sin,
			0,
			r * cos
		);
		transform.localPosition = pos;
	}

}
