using System;
using System.Linq;

namespace Powergrid.Power
{
    public sealed class BuildingPowerProfileResolver
    {
        public bool TryResolve(IBuildingDefinition definition, out PowerProfile profile)
        {
            profile = null;

            string id = definition.Id.ToString();
            if (IsTransportOnly(id))
            {
                return false;
            }

            int inputCount = 1;
            int outputCount = 1;
            float operationsPerMinute = 60.0f;
          
            TryReadConnectorCounts(definition, ref inputCount, ref outputCount);
            TryReadOperationsPerMinute(definition, ref operationsPerMinute);

            profile = new PowerProfile(
                operationsPerMinute,
                inputCount,
                outputCount,
                InferMachineType(id));
            return true;
        }

        private static bool IsTransportOnly(string definitionId)
        {
            string id = definitionId.ToLowerInvariant();
            return id.Contains("belt")
                   || id.Contains("pipe")
                   || id.Contains("rail")
                   || id.Contains("platform")
                   || id.Contains("foundation");
        }

        private static MachineType InferMachineType(string definitionId)
        {
            string id = definitionId.ToLowerInvariant();
            if (id.Contains("fluid") || id.Contains("pipe") || id.Contains("pump"))
            {
                return MachineType.Fluid;
            }

            if (id.Contains("stack") || id.Contains("crystal") || id.Contains("pin"))
            {
                return MachineType.Precision;
            }

            if (id.Contains("advanced") || id.Contains("train") || id.Contains("space"))
            {
                return MachineType.Advanced;
            }

            return MachineType.Mechanical;
        }

        private static void TryReadConnectorCounts(IBuildingDefinition definition, ref int inputCount, ref int outputCount)
        {
            try
            {
                IBuildingConnectorData connectorData = definition.CustomData.Get<IBuildingConnectorData>();
                inputCount = connectorData.AllBuildingConnectors.Count(connector => connector is BuildingItemInput
                                                                                    || connector is BuildingFluidInput
                                                                                    || connector is BuildingSignalInput);
                outputCount = connectorData.AllBuildingConnectors.Count(connector => connector is BuildingItemOutput
                                                                                     || connector is BuildingFluidOutput
                                                                                     || connector is BuildingSignalOutput);
            }
            catch
            {
                inputCount = Math.Max(1, inputCount);
                outputCount = Math.Max(0, outputCount);
            }
        }

        private static void TryReadOperationsPerMinute(IBuildingDefinition definition, ref float operationsPerMinute)
        {
            try
            {
                BuildingEfficiencyData efficiencyData = definition.CustomData.Get<BuildingEfficiencyData>();
                if (efficiencyData.OriginalProcessingDuration > 0.0f)
                {
                    operationsPerMinute = 60.0f * efficiencyData.ProcessingLaneCount
                                          / efficiencyData.OriginalProcessingDuration;
                }
            }
            catch
            {
                operationsPerMinute = Math.Max(1.0f, operationsPerMinute);
            }
        }
    }
}
