using CharacterMainLib;
using InventoryCoreLib;
using System.Collections.Generic;

public class AttackComplex
{
    public uint Damage { get; init; }
    public double EffectiveRange { get; init; }

    public IEnumerable<IAffect> Effects { get; init; }

    public AttackComplex(
        uint damage,
        double range,
        IEnumerable<IAffect> effects)
    {
        Damage = damage;
        EffectiveRange = range;
        Effects = effects;
    }

    public void ApplyAttackToTarget(IAffectable? target)
    {
        if (target is not IAffectableActor actor)
            return;

        actor.DamageVital("Health", Damage);

        foreach (var effect in Effects)
        {
            switch (effect)
            {
                case IAffectTemporary<IAffectableActor> effectTempActor
                    when actor is IDemoActor collector:

                    var actorTempEffect = effectTempActor.MakeMyCopy()
                        as IAffectTemporary<IAffectableActor>;

                    if (actorTempEffect is null)
                        throw new ObjectNotInitializedException(nameof(actorTempEffect));

                    actorTempEffect.SetTarget(actor);
                    collector.AddTemporaryEffect(actorTempEffect);

                    break;

                case IAffectImmediately<IAffectableActor> effectImmediateActor
                    when actor is IDemoActor character:

                    var actorImmdEffect = effectImmediateActor.MakeMyCopy()
                        as IAffectImmediately<IAffectableActor>;

                    if (actorImmdEffect is null)
                        throw new ObjectNotInitializedException(nameof(actorImmdEffect));

                    actorImmdEffect.SetTarget(actor);
                    actorImmdEffect.Apply();

                    break;

                case IAffectTemporary<IAffectableCharacter> effectTempCharacter
                    when actor is IDemoCharacter character:

                    var charTempEffect = effectTempCharacter.MakeMyCopy()
                        as IAffectTemporary<IAffectableCharacter>;

                    if (charTempEffect is null)
                        throw new ObjectNotInitializedException(nameof(charTempEffect));

                    charTempEffect.SetTarget(character);
                    character.AddTemporaryEffect(charTempEffect);

                    break;

                case IAffectImmediately<IAffectableCharacter> effectImmediateCharacter
                    when actor is IDemoCharacter character:

                    var charImmdEffect = effectImmediateCharacter.MakeMyCopy()
                        as IAffectImmediately<IAffectableCharacter>;

                    if (charImmdEffect is null)
                        throw new ObjectNotInitializedException(nameof(charImmdEffect));

                    charImmdEffect.SetTarget(character);
                    charImmdEffect.Apply();

                    break;
            }
        }
    }
}