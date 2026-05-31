using Core.Logging;
using HarmonyLib;
using Powergrid.Hud;
using Powergrid.Power;

namespace Powergrid.UI
{
    public static class PowerGridUI
    {
        private static Harmony harmony;

        public static void Install(ILogger logger, PowerNetworkManager networkManager)
        {
            if (harmony != null)
            {
                return;
            }

            PowerGridHudSetup.Configure(networkManager);
            harmony = new Harmony("powergrid.ui");
            harmony.PatchAll(typeof(PowerGridHudPatches));
        }

        public static void Dispose()
        {
            PowerGridHudSetup.ClearInstance();
            harmony?.UnpatchSelf();
            harmony = null;
        }
    }
}
