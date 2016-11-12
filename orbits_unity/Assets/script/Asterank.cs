using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Asterank : SingletonBehaviour<Asterank> {

	public const string url = "http://asterank.com/api/asterank";

	public delegate void QueryCallback(int i, Data data);

	public static void Query(string query, int limit, QueryCallback callback) {
 		instance.StartCoroutine(QueryRoutine(string.Format("{0}?query={1}&limit={2}", url, query, limit), callback));
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
				callback(i, JsonUtility.FromJson<Data>(m.ToString()));
				i ++;
			}
		}
		else {
			Debug.LogError(www.error);
		}
	}

	[System.Serializable]
	public class Data {

		public float est_diamater = 1;	// est. diameter (km ?)
		public string full_name = "";	// name
		public float i = 0;			// inclination (deg)
		public float a = 1;			// semimajor axis (au)
		public float e = 0;			// eccentricity
		public float om = 0;			// longitude of the ascending node (deg)
		public float w = 0;			// argument of periapsis (deg)

	}

}
