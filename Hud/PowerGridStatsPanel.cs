using Powergrid.Power;
using System.Collections;
using UnityEngine;

namespace Powergrid.Hud
{
    public class PowerGridStatsPanel : MonoBehaviour
    {
        private PowerNetworkManager NetworkManager;
        private Coroutine RefreshRoutine;

        internal PowerGridStatChip OutputChip;
        internal PowerGridStatChip UsageChip;
        internal PowerGridStatChip RatioChip;
        internal PowerGridStatChip EfficiencyChip;

        public void SetNetworkManager(PowerNetworkManager networkManager)
        {
            NetworkManager = networkManager;
        }

        public void StartRefreshing()
        {
            Refresh();

            if (RefreshRoutine == null)
            {
                RefreshRoutine = StartCoroutine(RefreshLoop());
            }
        }

        private IEnumerator RefreshLoop()
        {
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            while (true)
            {
                Refresh();
                yield return wait;
            }
        }

        private void Refresh()
        {
            if (NetworkManager == null || OutputChip == null || UsageChip == null || RatioChip == null || EfficiencyChip == null)
            {
                return;
            }

            PowerNetworkSnapshot snapshot = NetworkManager.GetSnapshot();
            OutputChip.ValueText.text = FormatPower(snapshot.GenerationKW);
            UsageChip.ValueText.text = FormatPower(snapshot.DemandKW);
            RatioChip.ValueText.text = snapshot.Ratio.ToString("0.##");
            EfficiencyChip.ValueText.text = $"{Mathf.RoundToInt(snapshot.Efficiency * 100f)}%";
        }

        private static string FormatPower(float kilowatts)
        {
            if (kilowatts >= 1000.0f)
            {
                return $"{kilowatts / 1000.0f:0.#}k";
            }

            if (kilowatts >= 100.0f)
            {
                return $"{kilowatts:0}k";
            }

            return $"{kilowatts:0.#}k";
        }

        private void OnDestroy()
        {
            if (RefreshRoutine != null)
            {
                StopCoroutine(RefreshRoutine);
                RefreshRoutine = null;
            }

            if (PowerGridHudSetup.Instance == this)
            {
                PowerGridHudSetup.ClearInstance();
            }
        }
    }
}
