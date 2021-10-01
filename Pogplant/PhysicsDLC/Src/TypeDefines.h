/******************************************************************************/
/*!
\file	TypeDefines.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong\@digipen.edu
\details
	Header containing default types used for the library

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
#ifndef TYPEDEFINES_H_
#define TYPEDEFINES_H_

namespace PhysicsDLC
{
	/**> Type used for vector 3*/
	using vec3 = glm::vec3;
	
	/**> Type used for floating point values*/
	using floatType = float;

	/**> Type used for delta time calculations*/
	using dtType = double;

	namespace Vector
	{
		static const glm::vec3 X = glm::vec3(1.f, 0.f, 0.f);
		static const glm::vec3 Y = glm::vec3(0.f, 1.f, 0.f);
		static const glm::vec3 Z = glm::vec3(0.f, 0.f, 1.f);
		static const glm::vec3 Zero = glm::vec3(0.f, 0.f, 0.f);
	};
}

#endif // !TYPEDEFINES_H_
