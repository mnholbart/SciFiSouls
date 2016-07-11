using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListHelper {

	/// <summary>
	/// Pop a generic list and return the last element
	/// </summary>
	public static T Pop<T>(this List<T> param) {
		T popped = param[param.Count-1];
		param.RemoveAt(param.Count-1);

		return popped;
	}

	public static void Push<T>(this List<T> list, T param) {
		list.Add(param);
	}

	/// <summary>
	/// Finds the name of an object from a list, 
	/// warning: not really tested, works with a game object by removing unities added (System.GameObject) stuff and taking the name
	/// </summary>
	public static T FindByName<T>(this List<T> list, string name) {
		for (int i = 0; i < list.Count; i++) {
			T o = list[i];
			string[] str = o.ToString().Split(" "[0]);
			if (str[0] == name) 
				return o;
		}
		return default(T);
	}
}
