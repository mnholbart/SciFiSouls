using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class m_Utility  {

	public static class ColorUtil {

		/// <summary>
		/// Parses a color value from a string
		/// </summary>
		public static Color ParseColor(string colorString) {
			string[] strings = colorString.Split(","[0]);
			Color output = new Color();;
			for (int i = 0; i < 4; i++) {
				output[i] = System.Single.Parse(strings[i]);
			}
			return output;
		}

		public static string GetParsableString(Color c) {
			return c.r + "," + c.g + "," + c.b + "," + c.a;
		}
	}
}
