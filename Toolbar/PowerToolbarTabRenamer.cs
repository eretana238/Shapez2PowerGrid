using System.Collections.Generic;
using System.Linq;
using Core.Localization;
using ShapezShifter.Flow.Toolbar;
using ShapezShifter.Hijack;
using UnityEngine;

namespace Powergrid.Toolbar
{
    internal static class PowerToolbarTabHelper
    {
        internal const int PowerTabIndex = 2;

        internal static bool TryGetPowerTab(ToolbarData toolbarData, out ParentToolbarElementData tab)
        {
            return TryGetToolbarTab(toolbarData, PowerTabIndex, out tab);
        }

        private static bool TryGetToolbarTab(
            ToolbarData toolbarData,
            int index,
            out ParentToolbarElementData tab)
        {
            tab = null;
            IEnumerable<IToolbarElementData> children = toolbarData.RootToolbarElement.Children
               .Where(child => !(child is ToolbarSlotSeparator));

            IToolbarElementData element = children.ElementAtOrDefault(index);
            tab = element as ParentToolbarElementData;
            return tab != null;
        }
    }

    internal sealed class PowerToolbarTabRenamer : IToolbarDataRewirer
    {
        private const string PowerTabTitleId = "powergrid.toolbar.power-tab.title";

        public ToolbarData ModifyToolbarData(ToolbarData toolbarData)
        {
            if (PowerToolbarTabHelper.TryGetPowerTab(toolbarData, out ParentToolbarElementData tab))
            {
                tab.Title = new LazyLocalizedText(new TranslationId(PowerTabTitleId));
            }

            return toolbarData;
        }
    }

    /// <summary>
    /// Inserts a Flow-Control-style <see cref="GroupToolbarElementData"/> ("Generators") into the Power tab.
    /// The micro reactor is the first child so it appears first in the sub-row and as the preferred preview.
    /// </summary>
    internal sealed class PowerToolbarMicroReactorInsertLocation : IToolbarEntryInsertLocation
    {
        private const string GeneratorsGroupTitleId = "powergrid.toolbar.generators.title";
        private const string GeneratorsGroupDescriptionId = "powergrid.toolbar.generators.description";

        /// <summary>0-based index in the Power tab; existing entries at this index and after shift right.</summary>
        internal const int GeneratorsGroupSlotIndex = 3;

        public void AddEntry(ToolbarData toolbarData, IToolbarElementData elementData)
        {
            if (!PowerToolbarTabHelper.TryGetPowerTab(toolbarData, out ParentToolbarElementData tab))
            {
                return;
            }

            var generatorsGroup = new GroupToolbarElementData
            {
                Title = new LazyLocalizedText(new TranslationId(GeneratorsGroupTitleId)),
                Description = new LazyLocalizedText(new TranslationId(GeneratorsGroupDescriptionId)),
                Icon = PowerToolbarLocations.GeneratorsGroupIcon,
                RememberPreferredChild = true,
                Children = new IToolbarElementData[] { elementData }
            };

            tab.InsertAtIndex(new ToolbarSlotSeparator(), GeneratorsGroupSlotIndex);
            tab.InsertAtIndex(generatorsGroup, GeneratorsGroupSlotIndex + 1);
            tab.InsertAtIndex(new ToolbarSlotSeparator(), GeneratorsGroupSlotIndex + 2);
        }
    }

    internal static class PowerToolbarLocations
    {
        internal static Sprite GeneratorsGroupIcon { get; set; }

        internal static IToolbarEntryInsertLocation MicroReactorInsert { get; } =
            new PowerToolbarMicroReactorInsertLocation();
    }
}
