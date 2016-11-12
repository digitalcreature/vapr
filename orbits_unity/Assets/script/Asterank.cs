using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Asterank : SingletonBehaviour<Asterank> {

	public const string url = "http://asterank.com/api/asterank";

	public delegate void QueryCallback(int i, Data data);

	void Awake() {
		Query("{\"e\":{\"$lt\":0.1},\"i\":{\"$lt\":4},\"a\":{\"$lt\":1.5}}", 20, (i, data) => {
			Debug.Log(string.Format("{0}: {1}", i, data.e));
		});
	}

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

		public string full_name;
		public float i = 0;
		public float a = 1;
		public float e = 0;

	}

}
