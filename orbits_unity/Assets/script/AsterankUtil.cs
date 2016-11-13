using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class AsterankUtil : SingletonBehaviour<AsterankUtil> {

	public const string baseurl = "http://asterank.com/api/asterank";

	public delegate void DataCallback(Data data);
	public delegate void FinishCallback();

	public static void Query(string query, int limit, int callbacksPerFrame, DataCallback dataCallback, FinishCallback finishCallback) {
		string url = string.Format("{0}?query={1}&limit={2}", baseurl, WWW.EscapeURL(query), limit);
 		instance.StartCoroutine(QueryRoutine(url, callbacksPerFrame, dataCallback, finishCallback));
	}

	static IEnumerator QueryRoutine(string url, int callbacksPerFrame, DataCallback dataCallback, FinishCallback finishCallback) {
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			string json = www.text;
			int i = 0;
			foreach (Match m in Regex.Matches(json, @"(\{[^\}]*\})")) {
				Data data = null;
				try {
					data = JsonUtility.FromJson<Data>(m.ToString());
				} catch(System.ArgumentException e) {
					Debug.LogErrorFormat("Error parsing JSON data {0}: {1}", i, e);
				}
				if (data != null) {
					if (data.a <= 0) {
						Debug.LogErrorFormat("Invalid JSON data {0}: Invalid Semimajor Axis of {1} AU", i, data.a);
					}
					else if (data.e < 0 || data.e >= 1) {
						Debug.LogErrorFormat("Invalid JSON data {0}: Invalid Eccentricity of {1}", i, data.e);
					}
					else {
						data.index = i;
						dataCallback(data);
					}
				}
				if (i % callbacksPerFrame == 0) {
					yield return null;
				}
				i ++;
			}
			finishCallback();
		}
		else {
			Debug.LogError(www.error);
		}
	}

	[System.Serializable]
	public class Data {

		public int index = 0;

		public float est_diamater = 1;	// est. diameter (km ?)
		public string full_name = "";		// name
		public float i = 0;					// inclination (deg)
		public float a = 1;					// semimajor axis (au)
		public float e = 0;					// eccentricity
		public float om = 0;					// longitude of the ascending node (deg)
		public float w = 0;					// argument of periapsis (deg)
		public float n = 0;					// mean motion (deg/day)
		public double tp = 0;				// time of periapsis passage (JED)
		public float GM = 1;					// standard gravitational parameter (km^3/s^2)

	}

}
