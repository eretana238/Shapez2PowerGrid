using System;
using Game.Core.Simulation;

namespace Powergrid.MicroReactor
{
    public class MicroReactorFuelSinkLane : IItemReceiver
    {
        private readonly Action OnFuelConsumed;

        public MicroReactorFuelSinkLane(Action onFuelConsumed)
        {
            OnFuelConsumed = onFuelConsumed;
        }

        public Steps MaxStep_S => LaneConstants.ItemSpacing;

        public bool CanAcceptItem(IBeltItem itemToDiscard)
        {
            return true;
        }

        public void HandOverItem(IBeltItem itemToDiscard, Ticks remainingTicks)
        {
            OnFuelConsumed();
        }
    }
}
