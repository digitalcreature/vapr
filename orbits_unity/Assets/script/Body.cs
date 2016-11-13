using UnityEngine;
using System.Collections.Generic;

public class Body : PooledBehaviour<Body> {

	public Orbit orbit;

	public OrbitalPlane path { get; private set; }

	static Mesh _mesh;
	public static Mesh mesh {
		get {
			if (_mesh == null) {
				GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				_mesh = prim.GetComponent<MeshFilter>().sharedMesh;
				Destroy(prim);
			}
			return _mesh;
		}
	}

	static Material _mat;
	public static Material mat {
		get {
			if (_mat == null) {
				_mat = new Material(Shader.Find("Sprites/Default"));
			}
			return _mat;
		}
	}

	public float smoothedTrueAnomaly { get; private set; }

	MeshFilter filter;
	MeshRenderer render;


	protected override void OnAllocate() {
		filter = GetComponent<MeshFilter>();
		if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
		render = GetComponent<MeshRenderer>();
		if (render == null) render = gameObject.AddComponent<MeshRenderer>();
		filter.mesh = mesh;
		render.material = mat;
		path = OrbitalPlane.Allocate(name);
		transform.parent = path.transform;
		transform.localScale = Vector3.one * 0.025f;
	}

	protected override void OnFree() {
		transform.parent = null;
		OrbitalPlane.Free(path);
	}

	public static Body FromAsterankData(AsterankUtil.Data data) {
		Body body = Allocate("body [" + data.full_name + "]");
		body.orbit.SetFromAsterankData(data);
		body.orbit.CalculateAnomalies();
		body.path.name = "orbit [" + data.full_name + "]";
		body.path.orbit = body.orbit;
		body.path.UpdateDisplay();
		body.UpdatePosition();
		return body;
	}

	void OnValidate() {
		if (path != null) {
			UpdatePosition();
			path.orbit = orbit;
			path.UpdateDisplay();
		}
	}

	void Update() {
		smoothedTrueAnomaly = Mathf.LerpAngle(smoothedTrueAnomaly, orbit.trueAnomaly, Time.deltaTime * 5);
		UpdatePosition(smoothedTrueAnomaly);
	}

	public void Step() {
		orbit.CalculateAnomalies();
	}

	public void UpdatePosition(float trueAnomaly) {
		float a = orbit.semimajorAxis;
		float e = orbit.eccentricity;
		float cos = Mathf.Cos(-trueAnomaly * Mathf.Deg2Rad);
		float sin = Mathf.Sin(-trueAnomaly * Mathf.Deg2Rad);
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
	public void UpdatePosition() { UpdatePosition(orbit.trueAnomaly); }

}
