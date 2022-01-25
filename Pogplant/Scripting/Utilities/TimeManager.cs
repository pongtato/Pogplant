using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
	public struct TimeManager
	{
		/*********************************************************************/
		/*!
		\brief
			Triggers a time alteration event
		\param timeMultiplier
			The timeScale to set to, 0.5 for 0.5x slow motion
		\param durationInRealTime
			How long the slow motion should last, in real time seconds

		\param audioAffectStat
			The setting to affect audio effects

			if == 0, will not affect the audio
			if == 1, will affect all except BGM
			if == 2, will affect all

		\param overrideAll
			Whether it should clear all the other slow motion effects that
			are still active
		*/
		/*********************************************************************/
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void TriggerTimeAlter(float timeMultiplier, float durationInRealTime, int audioAffectStat = 2, bool overrideAll = false);


		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void ForceNormalTime();
	}
}
