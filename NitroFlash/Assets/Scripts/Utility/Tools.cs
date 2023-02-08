using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp{

	/// <summary>
	/// This class adds additional utilities not found in the default unity libraries
	/// </summary>
	public static class Tools{
		/// <summary>
		/// This class defines a rudimentary JSON implementation for parsing and dumping javascript object
		/// </summary>
		public static class JSON{

			/// <summary>
			/// Converts a dictionary of names into a portable JSON object
			/// </summary>
			/// <param name="dict">Dictionary of names to serialize</param>
			/// <returns>Portable JSON string</returns>
			public static string SerializeNameDict(Dictionary<uint,string> dict){
				string str = "{";
				foreach (KeyValuePair<uint,string> kp in dict) {
					str += string.Format ("{0}:\"{1}\",", kp.Key, kp.Value);
				}
				str += "}";
				return str;
			}

			/// <summary>
			/// Parses a JSON object of player names into a native Dictionary type
			/// </summary>
			/// <param name="input">String representation of JSON name dictionary</param>
			/// <returns>Native Dictionary of IDs and Names</returns>
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