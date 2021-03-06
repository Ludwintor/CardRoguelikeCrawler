using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGame.Powers
{
    public class PowerGroup : IEnumerable<Power>
    {
        public event System.Action<Power> PowerApplied;
        public event System.Action<Power> PowerRemoved;
        public event System.Action PowersCleared;
        public IList<Power> Powers => _powers.AsReadOnly();

        private List<Power> _powers;

        public PowerGroup()
        {
            _powers = new List<Power>();
        }

        public void Add(PowerData data, int stack = 1)
        {
            if (stack == 0)
                return;
            Power power = Get(data);
            if (power != null)
            {
                power.Increase(stack);
                return;
            }
            power = new Power(data, stack);
            _powers.Add(power);
            PowerApplied?.Invoke(power);
        }

        public void Reduce(PowerData data, int stack)
        {
            Power power = Get(data);
            if (power == null)
                return;
            if (power.Stack <= stack)
                Remove(power);
            else
                power.Reduce(stack);
        }

        public void Remove(PowerData data)
        {
            Power power = Get(data);
            if (power == null)
                return;
            Remove(power);
        }

        public void Clear()
        {
            _powers.Clear();
            PowersCleared?.Invoke();
        }

        public Power Get(PowerData data)
        {
            foreach (Power power in _powers)
                if (power.Data == data)
                    return power;
            return null;
        }

        private void Remove(Power power)
        {
            _powers.Remove(power);
            PowerRemoved?.Invoke(power);
        }

        public IEnumerator<Power> GetEnumerator()
        {
            return _powers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
