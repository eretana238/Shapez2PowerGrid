using UnityEngine;

namespace Powergrid.UI
{
    internal static class GameObjectExtensions
    {
        public static void SetActiveSelfExt(this GameObject gameObject, bool active) =>
            gameObject.SetActive(active);
    }
}
