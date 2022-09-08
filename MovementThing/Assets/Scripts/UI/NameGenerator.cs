using UnityEngine;
namespace AssemblyCSharp{
	public static class NameGenerator {
		static string[] firstName = {
			"Bingo",
			"shwambit",
			"cockman",
			"shitboy"
		};
		static string[] lastName = {
			"lol",
			"gungonoid",
			"fucker",
			"dicklips"
		};
		public static string GenerateName(){
			return string.Format("{0}{1}",firstName[Random.Range(0,firstName.Length - 1)],lastName[Random.Range(0,lastName.Length - 1)]);
		}
	}
}
