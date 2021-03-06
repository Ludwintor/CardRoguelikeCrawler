using ProjectGame.Actions;
using ProjectGame.Powers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGame.Characters
{
    public abstract class Character
    {
        protected const int LOSE_ALL_BLOCK = -1;

        public event System.Action<int, int> HealthChanged;
        public event System.Action<int> BlockChanged;
        public event System.Action<Character> Dead;
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Block { get; private set; }
        public PowerGroup PowerGroup { get; }
        public bool IsAlive => Health > 0;

        protected ActionManager ActionManager => Game.GetSystem<ActionManager>();

        public Character(CharacterData data)
        {
            MaxHealth = data.MaxHealth;
            Health = MaxHealth;
            Block = data.InitialBlock;
            PowerGroup = new PowerGroup();
        }

        public virtual void TriggerStartTurn(int currentTurn) { }
        public virtual void TriggerEndTurn(int currentTurn) { }
        public virtual void TriggerCombatStart() { }
        public virtual void TriggerCombatEnd() { }

        public virtual void TakeDamage(DamageInfo info)
        {
            Debug.Assert(info.Damage >= 0, "Damage must be non-negative");
            int damage = info.ApplyPowers(this);
            int blockedDamage = Mathf.Min(Block, damage);
            LoseBlock(blockedDamage);
            Health -= damage - blockedDamage;
            Health = Mathf.Clamp(Health, 0, MaxHealth);
            HealthChanged?.Invoke(Health, MaxHealth);
            if (!IsAlive)
                Dead?.Invoke(this);
        }

        public virtual void Heal(int heal)
        {
            Debug.Assert(heal >= 0, "Heal must be non-negative");
            Health += heal;
            Health = Mathf.Clamp(Health, 0, MaxHealth);
            HealthChanged?.Invoke(Health, MaxHealth);
        }

        public virtual void GainBlock(int block)
        {
            Debug.Assert(block >= 0, "Block gain must be non-negative");
            Block += block;
            BlockChanged?.Invoke(Block);
        }

        public virtual void LoseBlock(int block = LOSE_ALL_BLOCK)
        {
            Debug.Assert(block >= -1, "Block lose must be non-negative or -1, if losing all block");
            Block -= block == -1 ? Block : block;
            Block = Mathf.Clamp(Block, 0, int.MaxValue);
            BlockChanged?.Invoke(Block);
        }
    }
}
