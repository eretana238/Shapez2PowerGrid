using Game.Core.Coordinates;

namespace Powergrid.Rendering
{
    internal class MicroReactorBeltLaneRenderingDefinition : IBeltLaneRendererDefinition
    {
        public MicroReactorBeltLaneRenderingDefinition(LocalVector itemStartPos_L, LocalVector itemEndPos_L)
        {
            ItemStartPos_L = itemStartPos_L;
            ItemEndPos_L = itemEndPos_L;
        }

        public LocalVector ItemStartPos_L { get; }
        public LocalVector ItemEndPos_L { get; }
    }
}
