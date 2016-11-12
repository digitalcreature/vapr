using UnityEngine;
using System.Collections.Generic;

public class OrbitQueryTester : SingletonBehaviour<OrbitQueryTester> {

	public string query = "{}";
	public int limit = 1;

	void Awake() {
		Asterank.Query(query, limit, (i, data) => {
			OrbitalPath op = new GameObject("orbital path (" + data.full_name + ")").AddComponent<OrbitalPath>();
			op.semimajorAxis = data.a;
			op.eccentricity = data.e;
			op.inclination = data.i;
			op.periapsisArgument = data.w;
			op.nodeLongitude = data.om;
			op.UpdateDisplay();
		});
	}

}
