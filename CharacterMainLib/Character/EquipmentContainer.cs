
using Microsoft.VisualBasic;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace CharacterMainLib
{

    public abstract class EquipmentContainer<T> : IEquipmentHandler<T>
        where T : class, IEquipable //: IEquipmentContainer
    {
        protected class EquipmentBlock : IManageEquipmentBlock<T>
        {

            public T? ItemInstance { get; protected set; } = null;

            public bool IsAvailable { get; protected set; } = true;

            public EquipmentBlock()
            {
                ItemInstance = null;
            }

            /// <summary>
            /// You can equip only if slot is empty
            /// </summary>
            public bool TryEquip(ref T? equipableItem)
            {
                if (equipableItem is null) return false;

                if(ItemInstance is null && IsAvailable)
                {
                    ItemInstance = equipableItem;
                    IsAvailable = false;
                    equipableItem = null;
                    return true;
                }

                return false;
            }

            public T? Unequip()
            {
                T? temp = ItemInstance;
                ItemInstance = null;
                IsAvailable = true;
                return temp;
            }

            public void SetAvailability(bool setting)
            {
                if(ItemInstance is not null)
                    throw new InvalidOperationException("You cannot setup occupied block");

                IsAvailable = setting;
            }

            public override string ToString()
            {
                if (ItemInstance is not null)
                    return ItemInstance.ToString();
                else
                    return "[EMPTY]";
            }
        }

        protected FrozenDictionary<string, EquipmentBlock>? EquipmentSlots { get; }

        public EquipmentContainer(IEnumerable<string> equipBlockNames)
        {
            Dictionary<string, EquipmentBlock> temp = new();

            foreach (var block in equipBlockNames)
            {
                temp.Add(block, new EquipmentBlock());
            }

            EquipmentSlots = temp.ToFrozenDictionary();
        }

        public virtual bool TryEquipIntoSlot(ref T? item)
        {
            if (EquipmentSlots is null)
                return false;

            string slotCategory = item is not null ? item.GetEquipSlotCategory() 
                :string.Empty;

            if (!EquipmentSlots.ContainsKey(slotCategory))
                return false;

            bool isEquipped = EquipmentSlots[slotCategory].TryEquip(ref item);

            return isEquipped;
        }

        public virtual T? UnequipFromSlot(string slotName)
        {
            if (slotName == string.Empty ||
                EquipmentSlots is null ||
                !EquipmentSlots.ContainsKey(slotName))
                return null;

            return EquipmentSlots[slotName].Unequip();
        }

        public abstract void Update(float deltaTime);
        public abstract string ItemInfo(string slotName);

        public abstract string ShowEquipment();

        public void SetSlotAvailability(string slotName, bool setting)
        {
            if (EquipmentSlots is null || !EquipmentSlots.ContainsKey(slotName))
                return;

            try
            {
                EquipmentSlots[slotName].SetAvailability(setting);
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("SOURCE::" + ex.Source);
            }
               
        }

        public bool IsSlotAvailable(string slotName)
        {
            if (EquipmentSlots is null)
                throw new NullReferenceException("Equipment slots are null");

            if (!EquipmentSlots.ContainsKey(slotName))
                throw new InvalidOperationException("Wrong slot parameter");

            return EquipmentSlots[slotName].IsAvailable;
        }

        public abstract IEquipable? GetInstanse(string blockName);
    }
}
