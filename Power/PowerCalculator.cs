using System;

namespace Powergrid.Power
{
    public static class PowerCalculator
    {
        public static float CalculateKW(PowerProfile profile)
        {
            if (profile.ManualOverrideKW >= 0.0f)
            {
                return profile.ManualOverrideKW;
            }

            float opsPerSecond = profile.OperationsPerMinute / 60.0f;
            float connectorComplexity = CalculateConnectorComplexity(profile.InputCount, profile.OutputCount);

            return profile.BaseKW
                   + opsPerSecond * profile.WorkKW * connectorComplexity * profile.MachineMultiplier;
        }

        public static float CalculateConnectorComplexity(int inputCount, int outputCount)
        {
            return 1.0f
                   + 0.15f * (inputCount + outputCount - 2)
                   + 0.25f * Math.Abs(outputCount - inputCount);
        }

        public static float GetMachineMultiplier(MachineType machineType)
        {
            switch (machineType)
            {
                case MachineType.Mechanical:
                    return 1.0f;
                case MachineType.Fluid:
                    return 1.3f;
                case MachineType.Precision:
                    return 1.6f;
                case MachineType.Advanced:
                    return 2.0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(machineType), machineType, null);
            }
        }
    }
}
