using UnityEngine;
using System.Collections.Generic;

public class OrbitalPath : PooledBehaviour<OrbitalPath> {

	public OrbitalElements elements;

	public Color color = new Color(1, 1, 1, 0.15f);

	static Mesh _mesh;
	public static Mesh mesh {
		get {
			if (_mesh == null) {
				Mesh mesh = new Mesh();
				mesh.name = "orbital path mesh";
				int vcount = 64;
				Vector3[] verts = new Vector3[vcount];
				int[] indices = new int[vcount + 1];
				for (int v = 0; v < vcount; v ++) {
					verts[v] = Vector3.right * v / vcount;
					indices[v] = v;
				}
				indices[vcount] = 0;
				mesh.vertices = verts;
				mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
				_mesh = mesh;
			}
			return _mesh;
		}
	}

	static Material _mat;
	public static Material mat {
		get {
			if (_mat == null) {
				_mat = new Material(Shader.Find("Orbital Path"));
			}
			return _mat;
		}
	}

	static int _color = -1;
	public static int colorPropertyID {
		get {
			if (_color < 0) {
				_color = Shader.PropertyToID("_Color");
			}
			return _color;
		}
	}

	static int _a = -1;
	public static int semimajorAxisPropertyID {
		get {
			if (_a < 0) {
				_a = Shader.PropertyToID("_A");
			}
			return _a;
		}
	}

	static int _e = -1;
	public static int eccentricityPropertyID {
		get {
			if (_e < 0) {
				_e = Shader.PropertyToID("_E");
			}
			return _e;
		}
	}

	MeshFilter filter;
	MeshRenderer render;
	MaterialPropertyBlock block;

	protected override void OnAllocate() {
		filter = GetComponent<MeshFilter>();
		if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
		render = GetComponent<MeshRenderer>();
		if (render == null) render = gameObject.AddComponent<MeshRenderer>();
		filter.mesh = mesh;
		render.sharedMaterial = mat;
		block = new MaterialPropertyBlock();
		UpdateDisplay();
	}

	public static OrbitalPath FromAsterankData(Asterank.Data data) {
		OrbitalPath path = Allocate();
		path.SetFromAsterankData(data);
		path.UpdateDisplay();
		return path;
	}

	public void SetFromAsterankData(Asterank.Data data) {
		name = "orbital path [" + data.full_name + "]";
		elements.SetFromAsterankData(data);
	}

	public void UpdateDisplay() {
		UpdateRotation();
		UpdatePropertyBlock();
	}

	void UpdateRotation() {
		transform.rotation = Quaternion.identity;
		transform.Rotate(0, -elements.periapsisArgument, 0, Space.World);
		transform.Rotate(0, 0, -elements.inclination, Space.World);
		transform.Rotate(0, -elements.nodeLongitude, 0, Space.World);
	}

	void UpdatePropertyBlock() {
		if (render != null && block != null) {
			// block.SetColor(colorPropertyID, color);
			block.SetFloat(semimajorAxisPropertyID, elements.semimajorAxis);
			block.SetFloat(eccentricityPropertyID, elements.eccentricity);
			render.SetPropertyBlock(block);
		}
	}

	void OnValidate() {
		UpdateDisplay();
	}

}

[System.Serializable]
public struct OrbitalElements {

	public const double AU = 149597870.7;		// length of astronomical unit in km
	public const double AU3 = AU * AU * AU;

	static double _epoch = 0;
	public static double epoch {
		get {
			if (_epoch == 0) {
				UpdateEpoch();
			}
			return _epoch;
		}
		set {
			_epoch = value;
		}
	}

	public static double UpdateEpoch() {
		return _epoch = System.DateTime.Now.ToOADate() + 2415018.5;
	}

	public float semimajorAxis;
	public float eccentricity;
	public float inclination;
	public float periapsisArgument;
	public float nodeLongitude;
	public float meanAnomaly;
	public float trueAnomaly;
	public float eccentricAnomaly;
	public float meanMotion;
	public double periapsisPassage;
	public double gravitationalParameter;

	// public float speed {
	// 	get {
	// 		return Mathf.Sqrt();
	// 	}
	// }

	public static OrbitalElements FromAsterankData(Asterank.Data data) {
		OrbitalElements elements = new OrbitalElements();
		elements.SetFromAsterankData(data);
		return elements;
	}

	public void SetFromAsterankData(Asterank.Data data) {
		semimajorAxis = data.a;
		eccentricity = data.e;
		inclination = data.i;
		periapsisArgument = data.w;
		nodeLongitude = data.om;
		meanMotion = data.n;
		periapsisPassage = data.tp;
		gravitationalParameter = ((double) data.GM) / (AU3); // units are important
	}

	public static float CalculateEccentricAnomaly(float M, float e, int maxiterations = 25, float minprecision = 0.00001f) {
		float E = M;
		for (int i = 0; i < maxiterations; i ++) {
			float m = E - e * Mathf.Sin(E);
			float delta = Mathf.Abs(m - M);
			if (delta <= minprecision) {
				break;
			}
			E = M + e * Mathf.Sin(E);

		}
		return E;
	}

	public void CalculateAnomalies(double epoch) {
		float dt = (float) (epoch - periapsisPassage);
		meanAnomaly = dt * meanMotion;
		eccentricAnomaly = CalculateEccentricAnomaly(
			meanAnomaly * Mathf.Deg2Rad, eccentricity
		);
		trueAnomaly = 2 * Mathf.Rad2Deg * Mathf.Atan2(
			Mathf.Sqrt(1 + eccentricity) * Mathf.Sin(eccentricAnomaly / 2),
			Mathf.Sqrt(1 - eccentricity) * Mathf.Cos(eccentricAnomaly / 2)
		);
		eccentricAnomaly *= Mathf.Rad2Deg;
	}
	public void CalculateAnomalies() { CalculateAnomalies(epoch); }

}
