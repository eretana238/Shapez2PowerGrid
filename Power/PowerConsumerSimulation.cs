using System;

namespace Powergrid.Power
{
    public sealed class PowerConsumerSimulation
    {
        private readonly Func<string> GetNetworkKey;

        public PowerConsumerSimulation(PowerProfile profile, Func<string> getNetworkKey)
        {
            Profile = profile;
            GetNetworkKey = getNetworkKey;
        }

        public PowerProfile Profile { get; }

        public float CurrentDemandKW => Math.Max(0.0f, PowerCalculator.CalculateKW(Profile));

        public string NetworkKey => GetNetworkKey() ?? PowerNetworkManager.DefaultNetworkKey;
    }
}
