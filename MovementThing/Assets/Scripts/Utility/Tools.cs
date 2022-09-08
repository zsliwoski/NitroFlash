using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp{
	public static class Tools{
		public static class JSON{
			public static string SerializeNameDict(Dictionary<uint,string> dict){
				string str = "{";
				foreach (KeyValuePair<uint,string> kp in dict) {
					str += string.Format ("{0}:\"{1}\",", kp.Key, kp.Value);
				}
				str += "}";
				return str;
			}
			public static Dictionary<uint,string> DeserializeNameDict(string input){
				Dictionary<uint,string> output = new Dictionary<uint,string> ();
				string inputStr = input;

				//TODO: find the formal way to implement this (or import a library)
				inputStr = inputStr.Replace("{","");
				inputStr = inputStr.Replace ("}", "");

				if (inputStr == "") {
					return output;
				}

				string[] keyValuePairs = inputStr.Split (',');

				if (keyValuePairs.Length > 0) {
					foreach (string keyValueStr in keyValuePairs) {
						if (keyValueStr != "") {
							string[] keyValue = keyValueStr.Split (':');
							uint key = uint.Parse (keyValue [0]);
							string value = keyValue [1].Replace ("\"", "");
							output.Add (key, value);
						}
					}
				}
				return output;
			}
		}
	}
}