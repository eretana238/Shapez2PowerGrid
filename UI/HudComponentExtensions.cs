using System;
using System.Linq;
using Unity.Core.View;

namespace Powergrid.UI
{
    public static class HudComponentExtensions
    {
        public static void AddChildViewInternal<TViewInterface>(
            this HUDComponent hudComponent,
            TViewInterface childView)
            where TViewInterface : UnityEngine.Object, IView
        {
            HudResources.ComponentAddChildViewInternal
                .MakeGenericMethod(typeof(TViewInterface))
                .Invoke(hudComponent, new object[] { childView });
        }

        public static void AddChildViewInternal(
            this HUDComponent hudComponent,
            Type type,
            IView childView)
        {
            HudResources.ComponentAddChildViewInternal
                .MakeGenericMethod(type)
                .Invoke(hudComponent, new object[] { childView });
        }

        public static HUDComponent[] GetChildComponentReferences(this HUDComponent hudComponent) =>
            (HUDComponent[])HudResources.ComponentChildComponentReferences.GetValue(hudComponent);

        public static void SetChildComponentReferences(
            this HUDComponent hudComponent,
            HUDComponent[] newValue)
        {
            HudResources.ComponentChildComponentReferences.SetValue(hudComponent, newValue);
        }

        public static void AddChildComponentReference(
            this HUDComponent hudComponent,
            HUDComponent toAdd)
        {
            hudComponent.SetChildComponentReferences(
                hudComponent.GetChildComponentReferences().Append(toAdd).ToArray());
        }
    }
}
