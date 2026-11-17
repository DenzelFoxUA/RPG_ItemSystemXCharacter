using System;

namespace InventoryCoreLib
{
    public class InventoryOperationException : InvalidOperationException
    {
        private static string MSG = "Inventory operation error.";

        public InventoryOperationException() { }

        public InventoryOperationException(string? message) 
            : base($"{ message } :: { MSG }")
        { }
    }
}
