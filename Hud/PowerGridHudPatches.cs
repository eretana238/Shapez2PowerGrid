using HarmonyLib;

namespace Powergrid.Hud
{
    internal static class PowerGridHudPatches
    {
        [HarmonyPatch(typeof(HUDChunkLimitInfo), "Construct")]
        [HarmonyPostfix]
        private static void AfterChunkLimit(HUDChunkLimitInfo __instance) =>
            PowerGridHudSetup.TryAttach(__instance.transform);

        [HarmonyPatch(typeof(HUDQuestArea), "Construct")]
        [HarmonyPostfix]
        private static void AfterQuestArea() =>
            PowerGridHudSetup.TryAttachFromScene();
    }
}
