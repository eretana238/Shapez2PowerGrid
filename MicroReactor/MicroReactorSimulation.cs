using System;
using Game.Core.Simulation;
using Powergrid.Power;

namespace Powergrid.MicroReactor
{
    public class MicroReactorSimulation : Simulation<MicroReactorSimulationState>, IItemSimulation, IUpdatableSimulation
    {
        private readonly IMicroReactorConfiguration Configuration;
        private readonly PowerProducerSimulation Producer;

        public readonly BeltLane InputLane;
        public readonly DelayBeltLane ProcessingLane;
        public readonly MicroReactorFuelSinkLane FuelSinkLane;

        public int NumItemReceivers => 1;
        public int NumItemProviders => 0;

        public float CurrentPowerOutputKW => State.FuelTicksRemaining > Ticks.Zero
            ? Configuration.OutputPowerKW
            : 0.0f;

        public MicroReactorSimulation(
            MicroReactorSimulationState simulationState,
            IMicroReactorConfiguration configuration) : base(simulationState)
        {
            Configuration = configuration;
            FuelSinkLane = new MicroReactorFuelSinkLane(OnFuelConsumed);
            ProcessingLane = new DelayBeltLane(
                configuration.ProcessingDelay,
                simulationState.ProcessingLaneState,
                FuelSinkLane);
            InputLane = new BeltLane(configuration.BeltSpeed, simulationState.InputLaneState, ProcessingLane);

            Producer = new PowerProducerSimulation(
                () => CurrentPowerOutputKW,
                () => PowerNetworkManager.DefaultNetworkKey);
            PowerNetworkManager.Default.RegisterProducer(Producer);
        }

        public IItemReceiver GetItemReceiver(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }

            return InputLane;
        }

        public IItemProvider GetItemProvider(int index)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
        }

        public void TraverseLanes<TTraverser>(TTraverser traverser)
            where TTraverser : IItemLaneTraverser
        {
            traverser.Traverse(InputLane);
            traverser.Traverse(ProcessingLane);
        }

        public void ClearContent()
        {
            TraverseLanes(ClearItemsItemLaneTraverser.Default);
            State.FuelTicksRemaining = Ticks.Zero;
            State.CurrentGeneratedKW = 0.0f;
            State.IsGenerating = false;
        }

        public void Update(Ticks startTicks, Ticks deltaTicks)
        {
            if (State.FuelTicksRemaining > Ticks.Zero)
            {
                State.FuelTicksRemaining = Ticks.Max(Ticks.Zero, State.FuelTicksRemaining - deltaTicks);
            }

            ProcessingLane.Update(deltaTicks);
            InputLane.Update(deltaTicks);

            State.CurrentGeneratedKW = CurrentPowerOutputKW;
            State.IsGenerating = State.CurrentGeneratedKW > 0.0f;
        }

        private void OnFuelConsumed()
        {
            State.FuelTicksRemaining += Ticks.OneSecond;
            State.CurrentGeneratedKW = Configuration.OutputPowerKW;
            State.IsGenerating = true;
        }
    }
}
