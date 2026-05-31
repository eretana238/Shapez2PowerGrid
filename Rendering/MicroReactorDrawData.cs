using Game.Core.Coordinates;

namespace Powergrid.Rendering
{
    internal class MicroReactorDrawData : IMicroReactorDrawData
    {
        // Same pattern as DiagonalCutter: half-tile west into connector at building origin (0,0).
        // These are raw local-space coords (building origin = connector at TileVector(0,0,0)).
        // On the 2x2 footprint the building spans (0,0) to (1,1) — the input comes from the
        // west belt at -0.5x, then heads north into the reactor core.
        public IBeltLaneRendererDefinition InputLaneRenderingDefinition =>
            new MicroReactorBeltLaneRenderingDefinition(
                new LocalVector(-0.5f, 0.0f, 0.0f),
                new LocalVector(0.0f, 0.0f, 0.0f));

        public IBeltLaneRendererDefinition ProcessingLaneRenderingDefinition =>
            new MicroReactorBeltLaneRenderingDefinition(
                new LocalVector(0.0f, 0.0f, 0.0f),
                new LocalVector(0.5f, 0.0f, 0.0f));
    }
}
