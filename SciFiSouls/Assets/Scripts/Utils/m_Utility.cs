using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public static class m_Utility {

    public static class ColorUtil {

        /// <summary>
        /// Parses a color value from a string
        /// </summary>
        public static Color ParseColor(string colorString) {
            string[] strings = colorString.Split(","[0]);
            Color output = new Color(); ;
            for (int i = 0; i < 4; i++) {
                output[i] = System.Single.Parse(strings[i]);
            }
            return output;
        }

        public static string GetParsableString(Color c) {
            return c.r + "," + c.g + "," + c.b + "," + c.a;
        }
    }

    /// <summary>
    /// Create a copy of a component from a target object 
    /// source/author: http://answers.unity3d.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
    /// </summary>
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component {
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos) {
            if (pinfo.CanWrite) {
                try {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                } catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos) {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }


}
