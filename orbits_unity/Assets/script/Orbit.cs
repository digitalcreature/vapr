using UnityEngine;

[System.Serializable]
public struct Orbit {

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

	public static Orbit FromAsterankData(AsterankUtil.Data data) {
		Orbit orbit = new Orbit();
		orbit.SetFromAsterankData(data);
		return orbit;
	}

	public void SetFromAsterankData(AsterankUtil.Data data) {
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
