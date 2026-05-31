using Core.Factory;

namespace Powergrid.MicroReactor
{
    public class MicroReactorSimulationFactory
        : IFactory<MicroReactorSimulationState, MicroReactorSimulation>
    {
        private readonly IMicroReactorConfiguration Configuration;

        public MicroReactorSimulationFactory(IMicroReactorConfiguration configuration)
        {
            Configuration = configuration;
        }

        public MicroReactorSimulation Produce(MicroReactorSimulationState simulationState)
        {
            return new MicroReactorSimulation(simulationState, Configuration);
        }
    }
}
