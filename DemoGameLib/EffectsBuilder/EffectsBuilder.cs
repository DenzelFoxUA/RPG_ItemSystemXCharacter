using InventoryCoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DemoGameLib
{
    internal sealed class EffectsBuilder
    {
        public IAffect Build(EffectData data)
        {
            if (!EffectsRegister.EFFECT_TYPES.TryGetValue
                (data.Type, out EffectRegistration? registration))
            {
                throw new InvalidDataException($"Unsupported effect type: '{data.Type}'.");
            }

            return registration.Factory(data);
        }


        public IAffect[] Build(IEnumerable<EffectData> effects)
        {
            return effects.Select(Build).ToArray();
        }
    }
}
