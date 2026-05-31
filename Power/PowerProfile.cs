namespace Powergrid.Power
{
    public class PowerProfile
    {
        public float BaseKW { get; }
        public float WorkKW { get; }
        public float MachineMultiplier { get; }
        public float ManualOverrideKW { get; }
        public float OperationsPerMinute { get; }
        public int InputCount { get; }
        public int OutputCount { get; }
        public MachineType MachineType { get; }

        public PowerProfile(
            float operationsPerMinute,
            int inputCount,
            int outputCount,
            MachineType machineType,
            float manualOverrideKW = PowerConstants.ManualOverrideDisabled,
            float baseKW = PowerConstants.BaseKW,
            float workKW = PowerConstants.WorkKW)
        {
            OperationsPerMinute = operationsPerMinute;
            InputCount = inputCount;
            OutputCount = outputCount;
            MachineType = machineType;
            MachineMultiplier = PowerCalculator.GetMachineMultiplier(machineType);
            ManualOverrideKW = manualOverrideKW;
            BaseKW = baseKW;
            WorkKW = workKW;
        }
    }
}
