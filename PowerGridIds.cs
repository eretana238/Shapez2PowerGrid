using Game.Core.Research;

namespace Powergrid
{
    internal static class PowerGridIds
    {
#pragma warning disable CS0618 // Mod-defined ids; game has no replacement API yet
        internal static readonly BuildingDefinitionGroupId MicroReactorGroup = new("PowerGridMicroReactorGroup");
        internal static readonly BuildingDefinitionId MicroReactor = new("MicroReactor");
#pragma warning restore CS0618
    }
}
