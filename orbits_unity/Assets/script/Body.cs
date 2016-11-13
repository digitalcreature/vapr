using UnityEngine;
using System.Collections.Generic;

public class Body : PooledBehaviour<Body> {

	public Orbit orbit;
	public float radius = 0.1f;
	public Color color = Color.white;

	public OrbitalPlane plane { get; private set; }

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
		plane = OrbitalPlane.Allocate(name);
		transform.parent = plane.transform;
		UpdateDisplay();
	}

	public void UpdateDisplay() {
		transform.localScale = Vector3.one * radius;
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		if (color != Color.white) {
			block.SetColor(OrbitalPlane.colorPropertyID, color);
		}
		render.SetPropertyBlock(block);
		plane.color = color;
		plane.UpdateDisplay();
	}

	protected override void OnFree() {
		transform.parent = null;
		OrbitalPlane.Free(plane);
	}

	public static Body FromAsterankData(AsterankUtil.Data data) {
		Body body = Allocate("body [" + data.full_name + "]");
		body.orbit.SetFromAsterankData(data);
		body.orbit.CalculateAnomalies();
		body.plane.name = "orbit [" + data.full_name + "]";
		body.plane.orbit = body.orbit;
		body.plane.UpdateDisplay();
		body.UpdatePosition();
		return body;
	}

	public static Body FromBodyInfo(Body.Info info) {
		Body body = Allocate("body [" + info.name + "]");
		body.orbit = info.orbit;
		body.orbit.CalculateAnomalies();
		body.color = info.color;
		body.radius = info.radius;
		body.plane.name = "orbit [" + info.name + "]";
		body.plane.orbit = body.orbit;
		body.plane.UpdateDisplay();
		body.UpdatePosition();
		body.UpdateDisplay();
		return body;
	}

	void OnValidate() {
		if (plane != null) {
			UpdatePosition();
			plane.orbit = orbit;
			plane.UpdateDisplay();
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

	[System.Serializable]
	public class Info {
		public string name;
		public Color color;
		public float radius;
		public Orbit orbit;
	}

}
