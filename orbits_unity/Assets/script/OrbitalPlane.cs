using UnityEngine;
using System.Collections.Generic;

public class OrbitalPlane : PooledBehaviour<OrbitalPlane> {

	public Orbit orbit;

	public Color color = Color.white;
	public float alpha = 0.025f;

	static Mesh _mesh;
	public static Mesh mesh {
		get {
			if (_mesh == null) {
				Mesh mesh = new Mesh();
				mesh.name = "orbital plane mesh";
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
				_mat.SetFloat("_Alpha", 0.25f);
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

	static int _alpha = -1;
	public static int alphaPropertyID {
		get {
			if (_alpha < 0) {
				_alpha = Shader.PropertyToID("_Alpha");
			}
			return _alpha;
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

	public static OrbitalPlane FromAsterankData(AsterankUtil.Data data) {
		OrbitalPlane plane = Allocate();
		plane.SetFromAsterankData(data);
		plane.UpdateDisplay();
		return plane;
	}

	public void SetFromAsterankData(AsterankUtil.Data data) {
		name = "orbital plane [" + data.full_name + "]";
		orbit.SetFromAsterankData(data);
	}

	public void UpdateDisplay() {
		UpdateRotation();
		UpdatePropertyBlock();
	}

	void UpdateRotation() {
		transform.rotation = Quaternion.identity;
		transform.Rotate(0, -orbit.periapsisArgument, 0, Space.World);
		transform.Rotate(0, 0, -orbit.inclination, Space.World);
		transform.Rotate(0, -orbit.nodeLongitude, 0, Space.World);
	}

	void UpdatePropertyBlock() {
		if (render != null && block != null) {
			if (color != Color.white) {
				block.SetColor(colorPropertyID, color);
			}
			block.SetFloat(alphaPropertyID, alpha);
			block.SetFloat(semimajorAxisPropertyID, orbit.semimajorAxis);
			block.SetFloat(eccentricityPropertyID, orbit.eccentricity);
			render.SetPropertyBlock(block);
		}
	}

	void OnValidate() {
		UpdateDisplay();
	}

}
