using Game.Core.Coordinates;
using Powergrid.MicroReactor;

namespace Powergrid.Rendering
{
    public class MicroReactorSimulationRenderer
        : StatelessBuildingSimulationRenderer<MicroReactorSimulation, IMicroReactorDrawData>
    {
        public MicroReactorSimulationRenderer(
            IMapModel map,
            IBuildingSoundManager soundManager,
            IShapeRegistry shapeRegistry) : base(map) { }

        public override void OnDrawDynamic(in Entity entity, FrameDrawOptions options)
        {
            MicroReactorSimulation simulation = entity.Simulation;

            DrawBeltItem(
                entity.Transform,
                options,
                simulation.InputLane,
                entity.DrawData.InputLaneRenderingDefinition);

            DrawBeltItem(
                entity.Transform,
                options,
                simulation.ProcessingLane,
                entity.DrawData.ProcessingLaneRenderingDefinition);

            if (simulation.CurrentPowerOutputKW > 0.0f)
            {
                DrawShapeSupportMesh(
                    entity.Transform,
                    options,
                    pos_L: options.Renderers.BeltItems.BeltShapeHeight * LocalVector.Up,
                    alpha: 1.0f);
            }
        }
    }
}
