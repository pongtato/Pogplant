/******************************************************************************/
/*!
\file	ChronoTimer.h
\author Gabriel Wong Choon Jieh
\par	email: c.wong@digipen.edu
\details
	File containing timer class used for calculating time
* \copyright	Copyright (c) 2020 DigiPen Institute of Technology. Reproduction
				or disclosure of this file or its contents without the prior
				written consent of DigiPen Institute of Technology is prohibited.
!*/
/******************************************************************************/
#ifndef CHRONOTIMER_H
#define CHRONOTIMER_H

#include <chrono>

namespace PPU
{
	/**************************************************************************/
	/*!
	\brief
		a class that handles time calculation
	*/
	/**************************************************************************/
	template <typename FloatType>
	class ChronoTimer
	{
	private:
		/**> the start time of the clock*/
		std::chrono::time_point<std::chrono::steady_clock> t_startTime;

		FloatType m_lastTime;

	public:
		ChronoTimer();
		~ChronoTimer();
		ChronoTimer(const ChronoTimer&) = delete;

		void Init();
		void startTimer();
		void stopTimer();
		FloatType getElapsedTimePrecise() const;

		FloatType getLastTime() const;
	};
}

#include "ChronoTimer.hpp"

#endif // _TIMER_H