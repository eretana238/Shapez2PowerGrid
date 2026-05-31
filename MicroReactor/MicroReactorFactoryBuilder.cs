using Core.Factory;
using Powergrid.Power;
using ShapezShifter.Flow.Atomic;
using ShapezShifter.Hijack;

namespace Powergrid.MicroReactor
{
    internal class MicroReactorFactoryBuilder
        : IBuildingSimulationFactoryBuilder<MicroReactorSimulation, MicroReactorSimulationState,
            MicroReactorConfiguration>
    {
        public IFactory<MicroReactorSimulationState, MicroReactorSimulation> BuildFactory(
            SimulationSystemsDependencies dependencies,
            out MicroReactorConfiguration config)
        {
            config = new MicroReactorConfiguration(
                BuffableBeltSpeed.DiscreteSpeed.OneSecondPerTile,
                BuffableBeltDelay.DiscreteDuration.OneSecond,
                new ResearchSpeedId("PowerGridMicroReactorSpeed"),
                PowerConstants.MicroReactorOutputKW);

            return new MicroReactorSimulationFactory(config);
        }
    }
}
