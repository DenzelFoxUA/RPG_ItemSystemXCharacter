using System;

namespace InventoryCoreLib
{
    public class CannotUseItemException : InvalidOperationException
    {
        private static string MSG = "Cannot use this item!";
        
        protected string NameOfItem { get; set; }

        public override string Message => base.Message + $"Item: {NameOfItem}";

        public CannotUseItemException(string nameOfItem) : base(MSG)
        {
            NameOfItem = nameOfItem;
        }
    }
}
