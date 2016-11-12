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
		filter = gameObject.AddComponent<MeshFilter>();
		filter.mesh = mesh;
		render = gameObject.AddComponent<MeshRenderer>();
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

	public float semimajorAxis;
	public float eccentricity;
	public float inclination;
	public float periapsisArgument;
	public float nodeLongitude;
	public float trueAnomaly;

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
	}

}
