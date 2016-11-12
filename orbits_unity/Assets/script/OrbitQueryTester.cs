using UnityEngine;
using System.Collections.Generic;

public class OrbitQueryTester : SingletonBehaviour<OrbitQueryTester> {

	public string query = "{}";
	public int limit = 1;

	void Awake() {
		Asterank.Query(query, limit, (data) => OrbitalPath.FromAsterankData(data));
	}

}
