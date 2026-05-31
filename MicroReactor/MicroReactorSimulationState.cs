using Game.Core.Serialization;
using Game.Core.Simulation;

namespace Powergrid.MicroReactor
{
    [SyncableIdentifier("PowerGridMicroReactorState")]
    public class MicroReactorSimulationState : ISimulationState
    {
        public readonly BeltLaneState InputLaneState = new();
        public readonly BeltLaneState ProcessingLaneState = new();
        public Ticks FuelTicksRemaining = Ticks.Zero;
        public float CurrentGeneratedKW;
        public bool IsGenerating;

        public void Sync(ISerializationVisitor visitor)
        {
            InputLaneState.Sync(visitor);
            ProcessingLaneState.Sync(visitor);

            var ticksSerializer = visitor.GetSerializer<Ticks>();
            ticksSerializer.Sync(ref FuelTicksRemaining);

            visitor.SyncFloat_4(ref CurrentGeneratedKW);
            visitor.SyncBool_1(ref IsGenerating);
        }
    }
}
