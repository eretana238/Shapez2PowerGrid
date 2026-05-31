using System.Collections.Generic;

namespace Powergrid.Hud
{
    public class MicroReactorBuildingModules : IBuildingModules
    {
        public IEnumerable<IHUDSidePanelModuleData> GetInfoModules(IBuildingDefinition definition)
        {
            yield break;
        }

        public IEnumerable<IHUDSidePanelModuleData> GetInfoModules(IMapModel map, BuildingModel building)
        {
            yield break;
        }
    }
}
