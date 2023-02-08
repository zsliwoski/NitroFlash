using UnityEngine;
namespace AssemblyCSharp{

	/// <summary>
	/// Class for generating basic placeholder names
	/// </summary>
	public static class NameGenerator {
		static string[] firstName = {
			"Bingo",
			"George",
			"Hank",
			"Zim"
		};
		static string[] lastName = {
			"Bongo",
			"Smith",
			"Bowl",
			"Peterson"
		};

		/// <summary>
		/// Generates a random name
		/// </summary>
		/// <returns>String with a random name</returns>
		public static string GenerateName(){
			return string.Format("{0}{1}",firstName[Random.Range(0,firstName.Length - 1)],lastName[Random.Range(0,lastName.Length - 1)]);
		}
	}
}
