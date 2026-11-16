
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CharacterMainLib
{

    public struct VitalsBaseValues
    {
        public uint StartMax { get; init; }
        public uint Peak { get; init; }

        public VitalsBaseValues(uint startMax, uint peak)
        {
            StartMax = startMax;
            Peak = peak;
        }

    }

    #region [ CHARACTER BASE ]

    /// <summary>
    /// Base character features:
    /// MaxLevel, MaxSpeed, BaseSpeed, Base/Min/MaxCarryWeight, Min/MaxAttributeVal
    /// </summary>
    [Serializable]
    public class CharacterBase
    {
        public uint MaxLevel { get; init; } = 99;
        public float MaxSpeed { get; init; } = 0.0f;

        public float BaseSpeed { get; init; }
        public double BaseCarryWeight { get; init; } 

        public double MinCarryWeight { get; init; } = 0.0d;
        public double MaxCarryWeight { get; init; } = 0.0d;

        public uint MinAttributeValue { get; init; }
        public uint MaxAttributeValue { get; init; }
    }

    #endregion

    /// <typeparam name="TypeItem">
    /// The type of items stored in the inventory.
    /// Must be a reference type.
    /// </typeparam>
    /// 
    [Serializable]
    public abstract class Character<IType, EType> : IAffectableCharacter
        where IType : class, IUsable
        where EType : class, IEquipable
    {
        public string Name { get; protected init; } = "";

        CharacterBase BaseFeatures { get; }

        public FrozenDictionary<string, VitalStat> VitalStats { get; protected set; }
        public FrozenDictionary<string, AttributeStat> AttributeStats { get; protected set; }

        #region [ CURRENT ]

        public uint CurrentLevel { get; protected set; }
        public float CurrMoveSpeed { get; protected set; } = 0.0f;
        public double CurrCarryWeight { get; protected set; } = .0d;

        #endregion

        #region [ EQUIPMENT ]

        protected virtual IEquipmentHandler<EType>? EquipmentSection { get => null; }

        #endregion 

        #region [ INVENTORY ]

        protected virtual IInventoryHandler<IType>? InventorySection { get => null; }

        #endregion

        #region [ UPDATE ]
        //підганяє Vitals під стати
        protected abstract void UpdateVitalsByStats();

        //підганяє швидкість під стати
        protected abstract void UpdateMovespeedByStats();
        #endregion

        #region [ MAIN CTOR ]

        public Character(string name,
            in CharacterBase baseFeatures,
            in IReadOnlyDictionary<string, VitalsBaseValues> vitals,
            in IEnumerable<string> attributes,
            in IEnumerable<string> equipBlockNames)
        {
            Name = name;
            BaseFeatures = baseFeatures;
            CurrentLevel = 1;

            //physics
            CurrCarryWeight = baseFeatures.BaseCarryWeight;
            CurrMoveSpeed = baseFeatures.BaseSpeed;

            //vitals,attributes,equip
            Dictionary<string, VitalStat> vitalStats = new();
            Dictionary<string, AttributeStat> attributeStats = new();

            foreach (var vital in vitals)
            {
                vitalStats.Add(vital.Key, new VitalStat(vital.Value.StartMax, vital.Value.Peak));
            }

            foreach (var attribute in attributes)
            {
                attributeStats.Add(attribute, 
                    new AttributeStat(baseFeatures.MinAttributeValue, baseFeatures.MaxAttributeValue));
            }

            VitalStats = vitalStats.ToFrozenDictionary();
            AttributeStats = attributeStats.ToFrozenDictionary();
        }

        #endregion

        public override string ToString()
        {
            string attributeStats = "";
            string vitals = "";

            foreach (var stat in VitalStats)
            {
                vitals += stat.Key;
                vitals += ": ";
                vitals += stat.Value.ToString();
                vitals += "\n";
            }

            foreach (var stat in AttributeStats)
            {
                attributeStats += $"{stat.Key,-20}{stat.Value.CurrentValue}\n";
            }

            return $"Name: {Name}\n" +
                $"Level: {CurrentLevel}\n" +
                $"Attributes: \n" +
                $"\n" + attributeStats +
                $"Vital Stats: \n" +
                $"{vitals}\n";
        }

        #region [ IAffectibleChar ]

        public virtual void RestoreVital(string name, uint amount)
        {
            if (!VitalStats.ContainsKey(name))
                throw new ArgumentException($"{name} - There is no such vital parameter!");

            VitalStats[name].Increase(amount);
        }

        public virtual void IncreaseAttribute(string name, uint amount)
        {
            if (!AttributeStats.ContainsKey(name))
                throw new ArgumentException($"{name} - There is no such attribute parameter!");

            AttributeStats[name].Increase(amount);
        }

        public void DamageVital(string name, uint amount)
        {
            if (!VitalStats.ContainsKey(name))
                throw new ArgumentException($"{name} - There is no such vital parameter!");

            VitalStats[name].Decrease(amount);
        }

        public void DecreaseAttribute(string name, uint amount)
        {
            if (!AttributeStats.ContainsKey(name))
                throw new ArgumentException($"{name} - There is no such attribute parameter!");

            AttributeStats[name].Decrease(amount);
        }

        public void CarryWeightUp(double value)
        {
            if (!double.IsFinite(value) || value < 0.0f)
                return;

            double res = CurrCarryWeight;

            bool isOk = CommonFunctions.Increase(ref res, value, BaseFeatures.MaxCarryWeight);

            if(isOk)
            {
                CurrCarryWeight = res;
            }    
        }

        public void CarryWeightDown(double value)
        {
            if (!double.IsFinite(value) || value < 0.0f)
                return;

            double res = CurrCarryWeight;

            bool isOk = CommonFunctions.Decrease(ref res, value, BaseFeatures.MinCarryWeight);

            if (isOk)
            {
                CurrCarryWeight = res;
            }
        }

        #endregion

        #region [ ICanMove ]

        public abstract void Move<DirVector>(DirVector? directionVector);

        public void MovementSpeedUp(float amount)
        {
            if (!float.IsFinite(amount) || amount < 0.0f)
                return;

            float res = CurrMoveSpeed;

            bool isOk = CommonFunctions.Increase(ref res, amount, BaseFeatures.MaxSpeed);

            if(isOk)
            {
                CurrMoveSpeed = res;
            }
        }

        public void MovementSlowDown(float amount)
        {
            if (!float.IsFinite(amount) || amount < 0.0f)
                return;

            float res = CurrMoveSpeed;

            bool isOk = CommonFunctions.Decrease(ref res, amount, 0);

            if (isOk)
            {
                CurrMoveSpeed = res;
            }
        }

        public virtual void Accelerate()
        {
        }

        #endregion
    }
}
