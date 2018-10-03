using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Keith.Towers
{
    [Serializable]
    public class TowerStats
    {
        public float BaseValue;
      

        public virtual float Value
        {
            get
            {
                if (isDirty || BaseValue != lastBaseValue)
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }
        }

        protected bool isDirty = true;
        protected float _value;
        protected float lastBaseValue = float.MinValue;
        protected readonly List<TowerModifier> towerModifiers;
        public ReadOnlyCollection<TowerModifier> TowerModifiers;

        public TowerStats() //constructs
        {
            towerModifiers = new List<TowerModifier>();
            TowerModifiers = towerModifiers.AsReadOnly();
        }

        public TowerStats(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public virtual void AddModifier(TowerModifier mod)
        {
            isDirty = true;
            towerModifiers.Add(mod);
            towerModifiers.Sort(CompareModifierOrder);
        }

        protected virtual int CompareModifierOrder(TowerModifier a, TowerModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // if (a.Order == b.Order)
        }
        public virtual bool RemoveModifier(TowerModifier mod)
        {
            if (towerModifiers.Remove(mod))
            {
                isDirty = true;
                return isDirty;

            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;
            for (int i = towerModifiers.Count - 1; i >= 0; i--)
            {
                if (towerModifiers[i].Source == source)
                {
                    isDirty = true;
                    didRemove = true;
                    towerModifiers.RemoveAt(i);
                }
            }
            return didRemove;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < towerModifiers.Count; i++)
            {
                TowerModifier mod = towerModifiers[i];

                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;
                    if (i + 1 >= towerModifiers.Count || towerModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
            }
            return (float)Math.Round(finalValue, 4); //4 significant digits
        }
       


    }
}

