using System;
using Core.Localization;
using Game.Core.Coordinates;
using Game.Core.Research;
using Powergrid;
using Powergrid.Hud;
using Powergrid.MicroReactor;
using Powergrid.Power;
using Powergrid.Rendering;
using Powergrid.Toolbar;
using Powergrid.UI;
using ShapezShifter.Flow;
using ShapezShifter.Flow.Atomic;
using ShapezShifter.Flow.Research;
using ShapezShifter.Hijack;
using ShapezShifter.Kit;
using ShapezShifter.Textures;
using UnityEngine;
using ILogger = Core.Logging.ILogger;
using Renderer = Powergrid.Rendering.MicroReactorSimulationRenderer;
using RendererData = Powergrid.Rendering.IMicroReactorDrawData;
using Simulation = Powergrid.MicroReactor.MicroReactorSimulation;

public class PowerGridMod : IMod
{
    private const string TitleId = "building-variant.powergrid-micro-reactor.title";
    private const string DescriptionId = "building-variant.powergrid-micro-reactor.description";

    private RewirerHandle _powerToolbarTabRenamerHandle;

    public PowerGridMod(ILogger logger)
    {
        logger.Info?.Log("PowerGrid initialized.");

        PowerGridUI.Install(logger, PowerNetworkManager.Default);
        _powerToolbarTabRenamerHandle = GameRewirers.AddRewirer(new PowerToolbarTabRenamer());

        ModFolderLocator resourcesLocator = ModDirectoryLocator
           .CreateLocator<PowerGridMod>()
           .SubLocator("Resources");

        PowerToolbarLocations.GeneratorsGroupIcon = FileTextureLoader.LoadTextureAsSprite(
            resourcesLocator.SubPath("PowerGrid_Icon.png"),
            out _);

        IBuildingGroupBuilder group = BuildingGroup.Create(PowerGridIds.MicroReactorGroup)
           .WithTitle(TitleId.T())
           .WithDescription(DescriptionId.T())
           .WithIcon(FileTextureLoader.LoadTextureAsSprite(
                resourcesLocator.SubPath("PowerGrid_Icon.png"),
                out _))
           .AsNonTransportableBuilding()
           .WithPreferredPlacement(DefaultPreferredPlacementMode.LinePerpendicular)
           .WithDefaultStructureOverview();
        
        IBuildingConnectorData connectorData = CreateTwoByTwoConnectorData();

        IBuildingBuilder building = Building.Create(PowerGridIds.MicroReactor)
           .WithConnectorData(connectorData)
           .DynamicallyRendering<Renderer, Simulation, RendererData>(new MicroReactorDrawData())
           .WithStaticDrawData(CreateDrawData(resourcesLocator))
           .WithoutSound()
           .WithoutSimulationConfiguration()
           .WithoutEfficiencyData();

        AtomicBuildings.Extend()
           .AllScenarios()
           .WithBuilding(building, group)
           .UnlockedAtMilestone(new ByIndexMilestoneSelector(new Index(0)))
           .WithDefaultPlacement()
           .InToolbar(PowerToolbarLocations.MicroReactorInsert)
           .WithSimulation(new MicroReactorFactoryBuilder(), logger)
           .WithCustomModules(new MicroReactorBuildingModules())
           .WithoutPrediction()
           .Build();
    }

    public void Dispose()
    {
        GameRewirers.RemoveRewirer(_powerToolbarTabRenamerHandle);
        PowerGridUI.Dispose();
    }

    private static IBuildingConnectorData CreateTwoByTwoConnectorData()
    {
        TileVector[] tiles =
        {
            new(0, 0, 0), new(1, 0, 0),
            new(0, -1, 0), new(1, -1, 0)
        };

        ShapeConnectorConfig inputConfig = ShapeConnectorConfig.DefaultInput();
        BuildingBaseIO[] connectors =
        {
            new BuildingItemInput
            {
                Position_L = new TileVector(0, 0, 0),
                Direction_L = inputConfig.Direction.Value,
                StandType = inputConfig.StandType,
                IOType = inputConfig.CapsType,
                Seperators = inputConfig.Separators
            }
        };

        LocalTileBounds tileBounds = new(
            min: new TileVector(-1, 0, 0),
            max: new TileVector(0, 1, 0));
        LocalVector center = LocalVector.Lerp(
            a: (LocalVector)tileBounds.Min,
            b: (LocalVector)tileBounds.Max,
            t: 0.5f);

        return new BuildingConnectorData(
            allInputs: connectors,
            tiles: tiles,
            tileBounds: tileBounds,
            tileBoundsCenter: center,
            tileDimensions: tileBounds.Dimensions);
    }

    private static BuildingDrawData CreateDrawData(ModFolderLocator resourcesLocator)
    {
        string meshPath = resourcesLocator.SubPath("MicroReactor.fbx");
        Mesh baseMesh = FileMeshLoader.LoadSingleMeshFromFile(meshPath);

        LOD6Mesh lod = MeshLod.Create()
           .AddLod0Mesh(baseMesh)
           .BuildLod6Mesh();

        return new BuildingDrawData(
            renderVoidBelow: false,
            new ILODMesh[] { lod, lod, lod },
            lod,
            lod,
            lod.LODClose,
            new LODEmptyMesh(),
            BoundingBoxHelper.CreateBasicCollider(baseMesh),
            new MicroReactorDrawData(),
            false,
            null,
            false);
    }
}
