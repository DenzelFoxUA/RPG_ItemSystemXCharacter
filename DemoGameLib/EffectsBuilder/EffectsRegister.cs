
using InventoryCoreLib;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace DemoGameLib
{
    internal sealed record EffectRegistration(
        Type DataType,
        Func<EffectData, IAffect> Factory);


    internal static class EffectsRegister
    {
        internal static readonly
            FrozenDictionary<string, EffectRegistration> EFFECT_TYPES =
            new Dictionary<string, EffectRegistration>
            {
                ["TempDrainHealth"] = 
                new EffectRegistration(typeof(TempDrainHealthEffectData),
                data => {
                    var effect = (TempDrainHealthEffectData)data;

                    return new TempDrainHealthEffect(
                        effect.Power,
                        effect.Duration,
                        effect.Name);
                }),

                ["DamageVital"] =
                new EffectRegistration(typeof(DamageVitalEffectData),
                data => {
                    var effect = (DamageVitalEffectData)data;

                    return new ImmdDamageVitalEffect(
                        effect.Name,
                        effect.VitalName,
                        effect.Power);
                }),

                ["RecoveryVital"] =
                new EffectRegistration(typeof(RecoveryVitalEffectData),
                data => {
                    RecoveryVitalEffectData dto = (RecoveryVitalEffectData)data;

                    return new ImmdRecoveryVitalEffect(
                        dto.Name,
                        dto.VitalName,
                        dto.Power);
                }),

                ["IncreaseAttribute"] =
                new EffectRegistration(
                    typeof(IncreaseAttributeEffectData),
                    data => {
                        var effect = (IncreaseAttributeEffectData)data;

                        return new BuffAttributeEffect(
                            effect.AttributeName,
                            effect.Power,
                            effect.Name);
        }),

            }
            .ToFrozenDictionary();
    }
}