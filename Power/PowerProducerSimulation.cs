using System;

namespace Powergrid.Power
{
    public sealed class PowerProducerSimulation
    {
        private readonly Func<float> GetCurrentPowerKW;
        private readonly Func<string> GetNetworkKey;

        public PowerProducerSimulation(Func<float> getCurrentPowerKW, Func<string> getNetworkKey)
        {
            GetCurrentPowerKW = getCurrentPowerKW;
            GetNetworkKey = getNetworkKey;
        }

        public float CurrentPowerKW => Math.Max(0.0f, GetCurrentPowerKW());

        public string NetworkKey => GetNetworkKey() ?? PowerNetworkManager.DefaultNetworkKey;
    }
}
