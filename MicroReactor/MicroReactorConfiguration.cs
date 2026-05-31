namespace Powergrid.MicroReactor
{
    public interface IMicroReactorConfiguration
    {
        BeltSpeed BeltSpeed { get; }
        BeltDelay ProcessingDelay { get; }
        float OutputPowerKW { get; }
    }

    internal class MicroReactorConfiguration : IMicroReactorConfiguration
    {
        private readonly BuffableBeltSpeed Speed;
        private readonly BuffableBeltDelay Delay;

        public MicroReactorConfiguration(
            BuffableBeltSpeed.DiscreteSpeed beltSpeed,
            BuffableBeltDelay.DiscreteDuration processingDuration,
            ResearchSpeedId researchSpeed,
            float outputPowerKW)
        {
            OutputPowerKW = outputPowerKW;
            Speed = new BuffableBeltSpeed
            {
                BaseSpeed = beltSpeed,
                ResearchId = researchSpeed
            };
            Delay = new BuffableBeltDelay
            {
                BaseDuration = processingDuration,
                Research = researchSpeed
            };

            Speed.OnAfterDeserialize();
            Delay.OnAfterDeserialize();
        }

        public BeltSpeed BeltSpeed => Speed;
        public BeltDelay ProcessingDelay => Delay;
        public float OutputPowerKW { get; }
    }
}
