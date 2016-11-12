using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Asterank : SingletonBehaviour<Asterank> {

	public const string baseurl = "http://asterank.com/api/asterank";

	public delegate void QueryCallback(Data data);

	public static void Query(string query, int limit, QueryCallback callback) {
		string url = string.Format("{0}?query={1}&limit={2}", baseurl, WWW.EscapeURL(query), limit);
 		instance.StartCoroutine(QueryRoutine(url, callback));
	}
	public static void Query(string query, QueryCallback callback) {
		Query(query, 1, callback);
	}

	static IEnumerator QueryRoutine(string url, QueryCallback callback) {
		WWW www = new WWW(url);
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			string json = www.text;
			int i = 0;
			foreach (Match m in Regex.Matches(json, @"(\{[^\}]*\})")) {
				Data data = JsonUtility.FromJson<Data>(m.ToString());
				data.index = i;
				callback(data);
				yield return null;
				i ++;
			}
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
