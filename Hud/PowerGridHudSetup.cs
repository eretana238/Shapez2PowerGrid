using Core.Localization;
using Powergrid.Power;
using Powergrid.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Powergrid.Hud
{
    public static class PowerGridHudSetup
    {
        internal const string PanelName = "PowerGrid Stats";

        private static PowerNetworkManager networkManager;

        internal static PowerGridStatsPanel Instance { get; private set; }

        internal static void Configure(PowerNetworkManager manager)
        {
            networkManager = manager;
        }

        internal static void ClearInstance()
        {
            Instance = null;
            networkManager = null;
        }

        internal static void TryAttach(Transform insertAfter)
        {
            if (networkManager == null)
            {
                return;
            }

            TryAttach(insertAfter, networkManager);
        }

        internal static void TryAttachFromScene()
        {
            if (Instance != null || networkManager == null)
            {
                return;
            }

            HUDChunkLimitInfo chunkLimit = Object.FindObjectOfType<HUDChunkLimitInfo>();
            if (chunkLimit != null)
            {
                TryAttach(chunkLimit.transform, networkManager);
            }
        }

        private static void TryAttach(Transform insertAfter, PowerNetworkManager manager)
        {
            if (Instance != null || insertAfter == null)
            {
                return;
            }

            Transform container = insertAfter.parent;
            if (container == null || container.Find(PanelName) != null)
            {
                return;
            }

            GameObject panelObject = new GameObject(PanelName, typeof(RectTransform));
            panelObject.transform.SetParent(container, false);
            panelObject.transform.SetSiblingIndex(insertAfter.GetSiblingIndex() + 1);
            panelObject.layer = LayerMask.NameToLayer("UI");

            RectTransform rectTransform = panelObject.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;

            LayoutElement layoutElement = panelObject.AddComponent<LayoutElement>();
            RectTransform reference = insertAfter as RectTransform;
            if (reference != null && reference.rect.height > 0.0f)
            {
                layoutElement.preferredHeight = reference.rect.height;
            }
            else
            {
                layoutElement.preferredHeight = 32.0f;
            }

            layoutElement.preferredWidth = 240.0f;
            layoutElement.flexibleWidth = 0.0f;

            PowerGridStatsPanel panel = panelObject.AddComponent<PowerGridStatsPanel>();
            BuildPanel(panel, manager);
            panel.StartRefreshing();
            Instance = panel;
        }

        private static void BuildPanel(PowerGridStatsPanel panel, PowerNetworkManager manager)
        {
            panel.SetNetworkManager(manager);

            HorizontalLayoutGroup layout = panel.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 12.0f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            panel.OutputChip = CreateStat(panel.transform, "powergrid.hud.output");
            panel.UsageChip = CreateStat(panel.transform, "powergrid.hud.usage");
            panel.RatioChip = CreateStat(panel.transform, "powergrid.hud.ratio");
            panel.EfficiencyChip = CreateStat(panel.transform, "powergrid.hud.efficiency");
        }

        private static PowerGridStatChip CreateStat(Transform parent, string titleKey)
        {
            GameObject chip = new GameObject("Stat", typeof(RectTransform));
            chip.transform.SetParent(parent, false);
            chip.layer = LayerMask.NameToLayer("UI");

            LayoutElement layoutElement = chip.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = 52.0f;
            layoutElement.minHeight = 24.0f;

            Image background = chip.AddComponent<Image>();
            background.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            background.raycastTarget = true;

            TextMeshProUGUI valueText = UIFactory.AddTextSecondary(chip.transform);
            UIFactory.SetupSecondaryText(valueText);
            valueText.color = new Color(1.0f, 1.0f, 1.0f, 0.92f);
            valueText.alignment = TextAlignmentOptions.Center;
            valueText.enableAutoSizing = false;
            valueText.fontSize = 18.0f;
            valueText.text = "0";
            valueText.raycastTarget = false;

            RectTransform textRect = valueText.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            HUDTooltipTarget tooltip = chip.AddComponent<HUDTooltipTarget>();
            tooltip.Title = titleKey.T();
            tooltip.Description = titleKey.T();

            return new PowerGridStatChip
            {
                ValueText = valueText,
                Tooltip = tooltip
            };
        }
    }
}
