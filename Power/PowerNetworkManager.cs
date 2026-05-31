using System;
using System.Collections.Generic;
using System.Linq;

namespace Powergrid.Power
{
    public sealed class PowerNetworkManager
    {
        public const string DefaultNetworkKey = "platform-network:auto";

        public static readonly PowerNetworkManager Default = new();

        private readonly List<WeakReference<PowerProducerSimulation>> Producers = new();
        private readonly List<WeakReference<PowerConsumerSimulation>> Consumers = new();

        public void RegisterProducer(PowerProducerSimulation producer)
        {
            Producers.Add(new WeakReference<PowerProducerSimulation>(producer));
        }

        public void RegisterConsumer(PowerConsumerSimulation consumer)
        {
            Consumers.Add(new WeakReference<PowerConsumerSimulation>(consumer));
        }

        public PowerNetworkSnapshot GetSnapshot(string networkKey = DefaultNetworkKey)
        {
            Cleanup();

            PowerProducerSimulation[] producers = Alive(Producers)
               .Where(producer => producer.NetworkKey == networkKey)
               .ToArray();
            PowerConsumerSimulation[] consumers = Alive(Consumers)
               .Where(consumer => consumer.NetworkKey == networkKey)
               .ToArray();

            float generationKW = producers.Sum(producer => producer.CurrentPowerKW);
            float demandKW = consumers.Sum(consumer => consumer.CurrentDemandKW);
            float ratio = demandKW <= 0.0f ? 1.0f : generationKW / demandKW;
            float efficiency = Math.Min(1.0f, ratio);

            return new PowerNetworkSnapshot(
                networkKey,
                generationKW,
                demandKW,
                ratio,
                efficiency,
                producers.Length,
                consumers.Length);
        }

        public float GetEfficiency(string networkKey = DefaultNetworkKey)
        {
            return GetSnapshot(networkKey).Efficiency;
        }

        public string GetHudText(string networkKey = DefaultNetworkKey)
        {
            PowerNetworkSnapshot snapshot = GetSnapshot(networkKey);
            return $"PowerGrid\nOutput: {snapshot.GenerationKW:0.#} kW\nUsage: {snapshot.DemandKW:0.#} kW\nRatio: {snapshot.Ratio:0.##}\nEfficiency: {snapshot.Efficiency:P0}";
        }

        private static IEnumerable<T> Alive<T>(IEnumerable<WeakReference<T>> references)
            where T : class
        {
            foreach (WeakReference<T> reference in references)
            {
                if (reference.TryGetTarget(out T target))
                {
                    yield return target;
                }
            }
        }

        private void Cleanup()
        {
            Producers.RemoveAll(reference => !reference.TryGetTarget(out _));
            Consumers.RemoveAll(reference => !reference.TryGetTarget(out _));
        }
    }

    public readonly struct PowerNetworkSnapshot
    {
        public readonly string NetworkKey;
        public readonly float GenerationKW;
        public readonly float DemandKW;
        public readonly float Ratio;
        public readonly float Efficiency;
        public readonly int ProducerCount;
        public readonly int ConsumerCount;

        public PowerNetworkSnapshot(
            string networkKey,
            float generationKW,
            float demandKW,
            float ratio,
            float efficiency,
            int producerCount,
            int consumerCount)
        {
            NetworkKey = networkKey;
            GenerationKW = generationKW;
            DemandKW = demandKW;
            Ratio = ratio;
            Efficiency = efficiency;
            ProducerCount = producerCount;
            ConsumerCount = consumerCount;
        }
    }
}
