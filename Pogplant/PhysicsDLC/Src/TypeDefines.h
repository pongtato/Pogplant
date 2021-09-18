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
}

#endif // !TYPEDEFINES_H_
