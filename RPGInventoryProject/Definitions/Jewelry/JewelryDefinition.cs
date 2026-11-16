using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryCoreLib
{
    public class JewelryDefinition : EquipableItem
    {
        public JewelryDefinition(uint id,
            string name,
            string describ,
            uint price,
            double weight,
            string equipCategory,
            string icoRef,
            string miniature2DRef)
        {

            Id = id;
            Name = name;
            Descriprion = describ;
            Price = price;
            Weight = weight;

            Icon = icoRef;
            Miniature2D = miniature2DRef;

            EqupmentCategory = equipCategory;
            IsStackable = false;

        }
    }
}
