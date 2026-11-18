using CharacterMainLib;
using System;
using System.Collections.Generic;


namespace InventoryCoreLib
{
    public abstract class WeaponINS<Effect,TTarget> : 
        DurableInstance<Effect, TTarget>, ICanDealDamage, IDurable
       where Effect : class, IAffect
        where TTarget : class, IAffectable
    {

        public BaseWeapon Weapon
        {
            get
            {
                if (Definition is null)
                    throw new ObjectNotInitializedException(nameof(Definition));

                return (BaseWeapon)Definition;
            }
        }

        public IAffectable? Target { get; protected set; } = null;

        public uint CurrentPrimaryDamage { get; protected set; } = 0;
        public uint CurrentSecondaryDamage { get; protected set; } = 0;

        public WeaponINS(BaseWeapon _weapon, params Effect[]?effects) :
            base(_weapon,effects)
        {
            CurrentPrimaryDamage = Weapon.PrimaryAttack.MaxDamage;

            if(Weapon.SecondaryAttack is not null)
            {
                CurrentPrimaryDamage = Weapon.SecondaryAttack.MaxDamage;
            }
        }

        //IAttack
        public virtual AttackComplex PrimaryAttack(IAffectable? target)
        {
            if (Weapon.PrimaryAttack == null)
                throw new ObjectNotInitializedException(nameof(Weapon.PrimaryAttack));

            var dist = Weapon.PrimaryAttack.AttackRange;

            TakeDamage(10);

            return new AttackComplex(CurrentPrimaryDamage, dist, Effects);
        }

        public virtual AttackComplex? SecondaryAttack(IAffectable? target)
        {

            if (Weapon.SecondaryAttack is null)
            {
                return null;
            }

            var dist = Weapon.PrimaryAttack.AttackRange;

            TakeDamage(10);

            return new AttackComplex(CurrentSecondaryDamage, dist, Effects);

        }


        public override void SetMyTarget(IAffectable? target)
        {
            if (ReferenceEquals(target, MyOwner))
                return;

            Target = target;
        }

        private uint UpdateDamage(double source)
        {
            var coef = GetDurabilityCoefficient();

            source *= coef;

            return (uint)Math.Round(source);
        }

        public override void Update()
        {
            CurrentPrimaryDamage = UpdateDamage(Weapon.PrimaryAttack.MaxDamage);

            if (Weapon.SecondaryAttack is not null)
            {
                CurrentSecondaryDamage = UpdateDamage(Weapon.SecondaryAttack.MaxDamage);
            }
        }
    }
}
