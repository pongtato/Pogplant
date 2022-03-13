using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Scripting
{
    //Using for data transfer mainly atm
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public Single X;
        public Single Y;
        public Single Z;
        public Single W;

        public Vector4(Single x = 0.0f, Single y = 0.0f, Single z = 0.0f, Single w = 0.0f)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4(Single v)
        {
            X = v;
            Y = v;
            Z = v;
            W = v;
        }
    }
}
