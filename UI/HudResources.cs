using HarmonyLib;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Powergrid.UI
{
    internal static class HudResources
    {
        public static readonly FieldInfo ComponentChildComponentReferences =
            AccessTools.Field(typeof(HUDComponent), "ChildComponentReferences");

        public static readonly MethodInfo ComponentAddChildViewInternal =
            AccessTools.Method(typeof(HUDComponent), "AddChildViewInternal");

        public static readonly Sprite HUDButtonBase =
            FindSprite("HUDButtonBase");

        public static readonly Sprite HUDSecondaryButtonBase =
            FindSprite("HUDSecondaryButtonBase");

        public static readonly Sprite HUDButtonHover =
            FindSprite("HUDButtonHover");

        public static readonly Sprite HUDPrimaryLightPanelMask =
            FindSprite("HUDPrimaryLightPanelMask");

        public static readonly Sprite HUDPrimaryLightPanel =
            FindSprite("HUDPrimaryLightPanel");

        public static readonly Sprite HUDScrollbarPanelBg =
            FindSprite("HUDScrollbarPanelBg");

        public static readonly Sprite HUDAntiAliasedHorizontalScrollDivider =
            FindSprite("HUDAntiAliasedHorizontalScrollDivider");

        public static readonly Sprite HUDAntiAliasedHorizontalDivider =
            FindSprite("HUDAntiAliasedHorizontalDivider");

        public static readonly Material DefaultTranslucent =
            FindMaterial("Default-Translucent");

        public static readonly Material UIAnimatedPanelMenuMaterial =
            FindMaterial("UI-AnimatedPanelMenuMaterial");

        public static readonly Material UISpriteWithMipMapBiasOverride =
            FindMaterial("UI-SpriteWithMipMapBiasOverride");

        public static readonly Material UIAnimatedButtonMaterial =
            FindMaterial("UI-AnimatedButtonMaterial");

        public static readonly TMP_FontAsset FontMediumSDF =
            FindFont("Font-Medium SDF");

        public static readonly TMP_FontAsset FontLightSDF =
            FindFont("Font-Light SDF");

        public static readonly FieldInfo HUDInputFieldUIInputFieldInfo =
            AccessTools.Field(typeof(HUDInputField), "UIInputField");

        public static readonly FieldInfo HUDInputFieldUIPlaceholderTextInfo =
            AccessTools.Field(typeof(HUDInputField), "UIPlaceholderText");

        public static readonly FieldInfo HUDScrollContainerUIScrollRectInfo =
            AccessTools.Field(typeof(HUDScrollContainer), "UIScrollRect");

        public static readonly FieldInfo HUDLocalizedTextUITextInfo =
            AccessTools.Field(typeof(HUDLocalizedText), "UIText");

        public static readonly FieldInfo HUDButtonUITextInfo =
            AccessTools.Field(typeof(HUDButton), "UIText");

        public static readonly FieldInfo HUDButtonUIButtonInfo =
            AccessTools.Field(typeof(HUDButton), "UIButton");

        public static readonly FieldInfo HUDButtonUIMainGroupInfo =
            AccessTools.Field(typeof(HUDButton), "UIMainGroup");

        public static readonly FieldInfo HUDButtonUIHoverIndicatorGroupInfo =
            AccessTools.Field(typeof(HUDButton), "UIHoverIndicatorGroup");

        public static readonly FieldInfo HUDButtonUIMainTransformInfo =
            AccessTools.Field(typeof(HUDButton), "UIMainTransform");

        public static readonly FieldInfo HUDAnimatedRoundButtonUIButtonInfo =
            AccessTools.Field(typeof(HUDAnimatedRoundButton), "UIButton");

        public static readonly FieldInfo HUDAnimatedRoundButtonUIIconSpriteInfo =
            AccessTools.Field(typeof(HUDAnimatedRoundButton), "UIIconSprite");

        public static readonly FieldInfo HUDAnimatedRoundButtonUIIconTransformInfo =
            AccessTools.Field(typeof(HUDAnimatedRoundButton), "UIIconTransform");

        public static readonly FieldInfo HUDAnimatedRoundButtonUIMainIconInfo =
            AccessTools.Field(typeof(HUDAnimatedRoundButton), "UIMainIcon");

        public static readonly FieldInfo TmpInputFieldRegexValue =
            AccessTools.Field(typeof(TMP_InputField), "m_RegexValue");

        private static Sprite FindSprite(string name)
        {
            foreach (Sprite sprite in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (sprite.name == name)
                {
                    return sprite;
                }
            }

            throw new InvalidOperationException($"Sprite '{name}' was not found.");
        }

        private static Material FindMaterial(string name)
        {
            foreach (Material material in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (material.name == name)
                {
                    return material;
                }
            }

            throw new InvalidOperationException($"Material '{name}' was not found.");
        }

        private static TMP_FontAsset FindFont(string name)
        {
            foreach (TMP_FontAsset font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (font.name == name)
                {
                    return font;
                }
            }

            throw new InvalidOperationException($"Font '{name}' was not found.");
        }
    }
}
