﻿using NScumm.Core.Audio.OPL;
using System.IO;
using System.Linq;

namespace WOLF3D.WOLF3DGame.OPL
{
    public class AdlibMultiplexer : IAdlibSignaller
    {
        public AdlibMultiplexer(params IAdlibSignaller[] players)
        {
            Players = players;
            TimeLeft = new int[Players.Length];
        }
        private readonly IAdlibSignaller[] Players;
        private readonly int[] TimeLeft;
        public void Init(IOpl opl)
        {
            foreach (IAdlibSignaller player in Players)
                player.Init(opl);
        }
        public void Silence(IOpl opl)
        {
            foreach (IAdlibSignaller player in Players)
                player.Silence(opl);
        }
        public uint Update(IOpl opl)
        {
            do
            {
                int soonest = Soonest;
                int subtract = TimeLeft[soonest];
                TimeLeft[soonest] = (int)Players[soonest].Update(opl);
                for (int i = 0; i < Players.Length; i++)
                    if (i != soonest)
                        TimeLeft[i] -= subtract;
            } while (TimeLeft.Where(f => f <= 0).Any());
            return (uint)TimeLeft[Soonest];
        }
        private int Soonest =>
            IndexOfSmallest(TimeLeft) is int index && index >= 0 ?
                index
                : throw new InvalidDataException("AdlibMultiplexer couldn't find next player!");
        public static int IndexOfSmallest(int[] array)
        {
            if (array == null || array.Length < 1)
                return -1;
            int min = int.MaxValue;
            int result = 0;
            for (int i = 0; i < array.Length; i++)
                if (array[i] < min)
                {
                    min = array[i];
                    result = i;
                }
            return result;
        }
    }
}
