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
                Console.WriteLine("Value not supported.");
            }

            return defaultValue;
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static float GetValueFloat(string key, float defaultValue, bool loadFromDocuments = true);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static int GetValueInt(string key, int defaultValue, bool loadFromDocuments = true);
    }
}
