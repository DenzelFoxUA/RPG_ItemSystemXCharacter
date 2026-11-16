using InventoryCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DemoGameLib
{
    #region [ EFFECTS TDO ]

    internal abstract class EffectData
    {
        public string Type { get; init; } = "";

        public string Name { get; init; } = "";
    }

    internal sealed class TempDrainHealthEffectData : EffectData
    {
        public uint Power { get; init; }

        public float Duration { get; init; }

    }

    internal sealed class DamageVitalEffectData : EffectData
    {
        public string VitalName { get; init; } = "";

        public uint Power { get; init; }

    }

    internal sealed class RecoveryVitalEffectData : EffectData
    {
        public string VitalName { get; init; } = "";

        public uint Power { get; init; }
    }

    internal sealed class IncreaseAttributeEffectData : EffectData
    {
        public string AttributeName { get; init; } = "";
        public uint Power { get; init; }
    }

    #endregion
}
