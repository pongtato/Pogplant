/******************************************************************************/
/*!
\file	DebugDraw.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
    This script externs some debug drawing function.

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System.Runtime.CompilerServices;

namespace Scripting
{
    class DebugDraw
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void DebugSphere(Vector3 _Position, Vector3 _CamDirection, float _Radius, uint _Segments);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void DebugLine(Vector3 _Position1, Vector3 _Position2);
    }
}
