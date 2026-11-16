using CharacterMainLib;
using InventoryCoreLib;
using System;
using System.Numerics;
using System.Threading;

namespace TestEnvironment
{
    internal class Program
    {
        static void RunFightScene(DemoCharacter one, DemoCharacter other)
        {
            AttackDefinition prim = new AttackDefinition
            {
                AttackRange = WeaponDistanceConstants.DAGGER_PHYSICAL_ATTACK_DIST,
                MaxDamage = 15
            };
            DaggerDefinition daggerDef = new DaggerDefinition(144,
                    "Poisoned Dagger",
                    "Made of Steel",
                    0.5,
                    100,
                    1000,
                    1,
                    prim,
                    null,
                    false, "Ico.jpg", "mini2D.png");

            //one picks-up and equip weapons
            {
                DaggerInstance poisonDagger =
                    new DaggerInstance(daggerDef, new TempDrainHealthEffect(5, 5, "Poison"));

                one.PickUpItem(poisonDagger);
            }

            //two picks-up and equip weapons

            {
                DaggerInstance poisonDagger = new DaggerInstance(
                    daggerDef,
                    new TempDrainHealthEffect(5, 5, "Poison"),
                    new ImmdDamageVitalEffect("Poison", "Health", 30));

                other.PickUpItem(poisonDagger);
            }

            Console.WriteLine("Open inventory and check if there weapons picked:");
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine(one);
            one.OpenInventory();

            Console.WriteLine(other);
            other.OpenInventory();
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("\n\nNEXT STEP->");

            one.UseItem(new Position(0, 0), other);

            other.UseItem(new Position(0, 0), one);

            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("\n\nNEXT STEP->");

            Console.WriteLine("Open inventory and check if weapons are equipped:");
            Console.WriteLine("--------------------------------------------------");

            Console.WriteLine(one);
            one.OpenInventory();

            Console.WriteLine(other);
            other.OpenInventory();
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("\n\nNEXT STEP->");

            double currentDistance = 5;
            double daggerDist = WeaponDistanceConstants.DAGGER_PHYSICAL_ATTACK_DIST;

            while (currentDistance > daggerDist)
            {
                Vector3 vec = new Vector3();

                Console.WriteLine($"Distance: {currentDistance} m");
                one.Move<Vector3>(vec);
                other.Move<Vector3>(vec);

                currentDistance--;
                Thread.Sleep(1000);
                Console.Clear();
            }

            Console.WriteLine("\n\nNEXT STEP->");

            Console.WriteLine(" ------- Test stab initiate -------- ");
            Console.ReadLine();

            var attackRes = one.PerformPrimaryAttack(other);
            attackRes!.ApplyAttackToTarget(other);

            var attackRes2 = other.PerformPrimaryAttack(one);
            attackRes2!.ApplyAttackToTarget(one);

            float deltaTimeSec = 1;
            float roundTime = 10;

            while (roundTime > 0)
            {
                one.Update(deltaTimeSec);
                other.Update(deltaTimeSec);

                Console.WriteLine(one);
                Console.WriteLine(other);

                Console.WriteLine($" ---- ROUND TIME: {roundTime} sec");

                Thread.Sleep((int)deltaTimeSec * 1000);
                roundTime -= deltaTimeSec;
                Console.Clear();
            }

        }
        // ---------------------------------------------------------------

        static void Main(string[] args)
        {
            //CharacterBase playerBase = new()
            //{
            //    MaxLevel = 99,
            //    MinAttributeValue = 5,
            //    MaxAttributeValue = 95,
            //    MaxCarryWeight = 400,
            //    MinCarryWeight = 5,
            //    MaxSpeed = 900.0f,
            //    BaseCarryWeight = 90,
            //    BaseSpeed = 300.0f
            //};

            //BodyArmorInstance leatherArmorDefinition = new BodyArmorInstance(
            //    10,
            //    "Leather Armor",
            //    "Very light armor.",
            //    100,
            //    50,
            //    9,
            //    1000,
            //    "ico.ref",
            //    "miniature2d.ref");

            //DemoCharacter character = new DemoCharacter(
            //        "PLAYER_TEST",
            //        playerBase,
            //        new InventorySize(6, 8),
            //        CharDefaultModel.Vitals,
            //        CharDefaultModel.Attributes,
            //        CharDefaultModel.EquipmentSection);

            //DemoCharacter enemy = new DemoCharacter(
            //    "ENEMY_TEST",
            //    playerBase,
            //    new InventorySize(6, 8),
            //    CharDefaultModel.Vitals,
            //    CharDefaultModel.Attributes,
            //    CharDefaultModel.EquipmentSection);


            //RunFightScene(character, enemy);

            //PotionDefinition potionDef1
            //    = new PotionDefinition(1, "Mana Potion", "Restores mana", 50, 0.2, 15, 5, "Ico.ref", "mini2D.ref");

            //PotionDefinition hpPotionDef
            //    = new PotionDefinition(2, "HP Potion", "Restores HP", 50, 0.2, 15, 5, "Ico.ref", "mini2D.ref");
            //{
            //    PotionInstance manaPotion =
            //        new PotionInstance(potionDef1, 2, new ImmdRecoveryVitalEffect("Restore mana", "Mana", 50));

            //    PotionInstance healthPotion =
            //        new PotionInstance(hpPotionDef, 2, new ImmdRecoveryVitalEffect("Restore HP", "Health", 50));

            //    PotionInstance manaPotion2 =
            //        new PotionInstance(potionDef1, 1, new ImmdRecoveryVitalEffect("Restore mana", "Mana", 50));

            //    BodyArmorInstance leatherArmor1 = new BodyArmorInstance(leatherArmorDefinition, new StrengthBuffEffect(10));


            //    character.PickUpItem(manaPotion);
            //    character.PickUpItem(healthPotion);
            //    character.PickUpItem(manaPotion2);
            //    character.PickUpItem(leatherArmor1);
            //}


            //Console.WriteLine(character);
            //character.OpenInventory();
            //Console.ReadLine();

            //character.VitalStats["Mana"].Decrease(80);

            //Console.WriteLine(character);
            //character.OpenInventory();
            //Console.ReadLine();

            //character.UseItem(new Position(0, 1), character);

            //Console.WriteLine(character);
            //character.OpenInventory();
            //Console.ReadLine();


            //character.UseItem(new Position(0, 2), character);

            //character.UseItem(new Position(0, 1), character);
            //character.Update(1);
            //Console.WriteLine(character);
            //character.OpenInventory();
            //Console.ReadLine();

            //character.Unequip("Body");
            //character.Update(1);
            //Console.WriteLine(character);
            //character.OpenInventory();
            //Console.ReadLine();

        }
    }
}
