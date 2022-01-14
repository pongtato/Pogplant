using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
	public struct AudioEngine
	{
		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static uint PlayAudio(uint entityID, uint index, string channelGroupName = "NULL");

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static float GetChannelGroupVolume(string channelGroupName);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static float SetChannelGroupVolume(string channelGroupName, float volume);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void CreateChannelGroup(string channelGroupName);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void PauseChannelGroup(string channelGroupName);

		[MethodImplAttribute(MethodImplOptions.InternalCall)]
		public extern static void ResumeChannelGroup(string channelGroupName);
	}
}
