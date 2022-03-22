using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
	public struct PlayerPrefs
	{
		public static T GetValue<T>(string key, T defaultValue, bool loadFromDocuments = true)
		{
            if (typeof(T) == typeof(float))
            {
                return (T)(object)GetValueFloat(key, (float)(object)defaultValue, loadFromDocuments);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)GetValueInt(key, (int)(object)defaultValue, loadFromDocuments);
            }
            else
            {
                Console.WriteLine("Get Value not supported.");
            }

            return defaultValue;
        }

        public static void SetValue<T>(string key, T value, bool saveToDocuments = true)
        {
            if (typeof(T) == typeof(float))
            {
                SetValueFloat(key, (float)(object)value, saveToDocuments);
            }
            else if (typeof(T) == typeof(int))
            {
                SetValueInt(key, (int)(object)value, saveToDocuments);
            }
            else
            {
                Console.WriteLine("Set Value not supported.");
            }
        }


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static float GetValueFloat(string key, float defaultValue, bool loadFromDocuments = true);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static int GetValueInt(string key, int defaultValue, bool loadFromDocuments = true);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static void SetValueFloat(string key, float value, bool saveToDocuments = true);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern static void SetValueInt(string key, int value, bool saveToDocuments = true);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void Save();
    }
}
