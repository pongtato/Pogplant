/******************************************************************************/
/*!
\file	QuarkTimer.cpp
\author Gabriel Wong Choon Jieh
\par	email: c.wong@digipen.edu
\details
	File containing timer class used for calculating time
* \copyright	Copyright (c) 2020 DigiPen Institute of Technology. Reproduction
				or disclosure of this file or its contents without the prior
				written consent of DigiPen Institute of Technology is prohibited.
!*/
/******************************************************************************/

/******************************************************************************/
/*!
\brief
	Default constructor
*/
/******************************************************************************/
template <typename FloatType>
PPU::ChronoTimer<FloatType>::ChronoTimer()
	:
	m_lastTime{ 0.0 }
{

}

/******************************************************************************/
/*!
\brief
	Destructor
*/
/******************************************************************************/
template <typename FloatType>
PPU::ChronoTimer<FloatType>::~ChronoTimer()
{

}

/******************************************************************************/
/*!
\brief
	Initialises the ChronoTimer
*/
/******************************************************************************/
template <typename FloatType>
void PPU::ChronoTimer<FloatType>::Init()
{
	t_startTime = std::chrono::high_resolution_clock::now();
}

/******************************************************************************/
/*!
\brief
	Starts the timer on the ChronoTimer
*/
/******************************************************************************/
template <typename FloatType>
void PPU::ChronoTimer<FloatType>::startTimer()
{
	t_startTime = std::chrono::high_resolution_clock::now();
}

/******************************************************************************/
/*!
\brief
	Logs the timer on the ChronoTimer
*/
/******************************************************************************/
template <typename FloatType>
void PPU::ChronoTimer<FloatType>::stopTimer()
{
	m_lastTime = static_cast<FloatType>(std::chrono::duration_cast<std::chrono::nanoseconds>(std::chrono::high_resolution_clock::now() - t_startTime).count() * 0.000000001);
}

/******************************************************************************/
/*!
\brief
	A more precise version of \a getElapsedTime() using \a std::chrono::nanoseconds,
	get the time since \a startTimer() is called in seconds
\returns
	the elapsed time in seconds
*/
/******************************************************************************/
template <typename FloatType>
FloatType PPU::ChronoTimer<FloatType>::getElapsedTimePrecise() const
{
	return static_cast<FloatType>(std::chrono::duration_cast<std::chrono::nanoseconds>(std::chrono::high_resolution_clock::now() - t_startTime).count() * 0.000000001);
}

/******************************************************************************/
/*!
\brief
	Gets the last time recorded from start to stop
\returns
	the elapsed time in seconds
*/
/******************************************************************************/
template <typename FloatType>
FloatType PPU::ChronoTimer<FloatType>::getLastTime() const
{
	return m_lastTime;
}