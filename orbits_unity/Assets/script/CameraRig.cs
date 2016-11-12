using UnityEngine;
using System.Collections.Generic;

public class CameraRig : SingletonBehaviour<CameraRig> {


	public float sensitivity = 5;
	public float minZoom = 1;
	public float maxZoom = 15;
	public float zoomIncrement = 0.5f;
	public float zoomSmoothing = 10;

	Camera cam;

	Vector2 _look;
	public Vector2 look {
		get {
			return _look;
		}
		set {
			value.x = Mathf.Repeat(value.x, 360);
			value.y = Mathf.Clamp(value.y, -90, 90);
			transform.rotation = Quaternion.identity;
			transform.Rotate(-value.y, value.x, 0);
			_look = value;
		}
	}

	public float zoom;

	void Awake() {
		cam = GetComponentInChildren<Camera>();
		look = Vector2.zero;
		zoom = -cam.transform.localPosition.z;
	}

	void Update() {
		if (Input.GetMouseButton(0)) {
			float x = Input.GetAxis("Mouse X") * sensitivity;
			float y = Input.GetAxis("Mouse Y") * sensitivity;
			look += new Vector2(x, y);
		}
		zoom += Input.mouseScrollDelta.y * zoomIncrement;
		zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
		Vector3 camPos = cam.transform.localPosition;
		camPos.z = Mathf.Lerp(camPos.z, -zoom, Time.deltaTime * zoomSmoothing);
		cam.transform.localPosition = camPos;
	}

}
