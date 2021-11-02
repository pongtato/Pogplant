﻿/*****************************************************************************/
/*!
\file	main.cs
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
	Main class for scripting c# project

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    // My main class for scripting
    public class Scripting
    {
        // Add Script Types here
        public static PlayerScript PlayerScript()
        {
            return new PlayerScript();
        }

        public static EnemyScript EnemyScript()
        {
            return new EnemyScript();
        }

        public static FollowSpline FollowSpline()
        {
            return new FollowSpline();
        }

        public static PlayerFire PlayerFire()
        {
            return new PlayerFire();
        }
    }
}
