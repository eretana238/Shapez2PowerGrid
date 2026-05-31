namespace Powergrid.Rendering
{
    public interface IMicroReactorDrawData : IBuildingCustomDrawData
    {
        IBeltLaneRendererDefinition InputLaneRenderingDefinition { get; }
        IBeltLaneRendererDefinition ProcessingLaneRenderingDefinition { get; }
    }
}
