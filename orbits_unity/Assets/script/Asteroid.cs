using UnityEngine;
using System.Collections.Generic;

public class Asteroid : PooledBehaviour<Asteroid> {

	public OrbitalElements elements;

	public OrbitalPath path { get; private set; }

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
		path = OrbitalPath.Allocate(name);
		transform.parent = path.transform;
		transform.localScale = Vector3.one * 0.05f;
	}

	protected override void OnFree() {
		transform.parent = null;
		OrbitalPath.Free(path);
	}

	public static Asteroid FromAsterankData(Asterank.Data data) {
		Asteroid asteroid = Allocate("asteroid [" + data.full_name + "]");
		asteroid.elements.SetFromAsterankData(data);
		asteroid.elements.CalculateAnomalies();
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

	void Update() {
		smoothedTrueAnomaly = Mathf.LerpAngle(smoothedTrueAnomaly, elements.trueAnomaly, Time.deltaTime * 5);
		UpdatePosition(smoothedTrueAnomaly);
	}

	public void Step() {
		elements.CalculateAnomalies();
	}

	public void UpdatePosition(float trueAnomaly) {
		float a = elements.semimajorAxis;
		float e = elements.eccentricity;
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
	public void UpdatePosition() { UpdatePosition(elements.trueAnomaly); }

}
