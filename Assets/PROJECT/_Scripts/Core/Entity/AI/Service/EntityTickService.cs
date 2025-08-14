using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EntityTickService : IEntityTickService, ITickable
{
    private class TickGroup
    {
        public float Rate;
        public float Timer;
        public Queue<IEntityTickable> Tickables = new();

        public TickGroup(float rate)
        {
            Rate = rate;
        }
    }


    private readonly Dictionary<float, TickGroup> _tickGroups = new();
    private readonly Dictionary<IEntityTickable, float> _tickableToRate = new();



    public void Register(IEntityTickable tickable, float rate)
    {
        if (!_tickGroups.TryGetValue(rate, out var group))
        {
            group = new TickGroup(rate);
            _tickGroups[rate] = group;
        }

        if (!_tickableToRate.ContainsKey(tickable))
        {
            group.Tickables.Enqueue(tickable);
            _tickableToRate[tickable] = rate;
        }
    }

    public void Unregister(IEntityTickable tickable)
    {
        if (_tickableToRate.TryGetValue(tickable, out float rate) && _tickGroups.TryGetValue(rate, out var group))
        {
            _tickableToRate.Remove(tickable);

            var newQueue = new Queue<IEntityTickable>();

            while (group.Tickables.Count > 0)
            {
                var current = group.Tickables.Dequeue();
                if (current != tickable)
                    newQueue.Enqueue(current);
            }

            group.Tickables = newQueue;

            if (group.Tickables.Count == 0)
                _tickGroups.Remove(rate);
        }
    }


    public void Tick()
    {
        foreach (var group in _tickGroups.Values)
        {
            group.Timer += Time.deltaTime;

            if (group.Timer >= group.Rate && group.Tickables.Count > 0)
            {
                group.Timer = 0f;

                var entity = group.Tickables.Dequeue();
                entity.TickUpdate();
                group.Tickables.Enqueue(entity);
            }
        }
    }
}
