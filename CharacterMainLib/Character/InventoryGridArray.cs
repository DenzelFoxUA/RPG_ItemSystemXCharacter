

using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace CharacterMainLib
{
    public struct InventorySize
    { 
        public ushort Rows { get; init; }
        public ushort Columns { get; init; }

        public InventorySize(ushort rows, ushort cols)
        {
            Rows = rows;
            Columns = cols;
        }
    }

    public struct Position
    {
        public ushort Row { get; init; }
        public ushort Column { get; init; }

        public Position(ushort row, ushort col)
        {
            Row = row;
            Column = col;
        }
    }

    public abstract class InventoryGridArray<T> : IInventoryHandler<T>
        where T: class, IUsable
    {

        protected InventorySize InventoryGrid { get; init; }

        public T?[,] Container { get; }

        public InventoryGridArray(InventorySize body)
        {
            if(body.Rows > 0 && body.Columns > 0)
            {
                InventoryGrid = body;
            }
            else
            {
                throw new System.ArgumentException("Size of a body must be > 0");
            }

            Container = new T[body.Rows, body.Columns];
        }

        public abstract T? AddItem(T item, Position pos);

        public abstract T? RemoveItem(Position pos);

        public abstract void SwapItems(Position one, Position two);

        public abstract T? AddItem(T item);

        public abstract T? RemoveItemBySearch(T item);

        public abstract void SwapItems(ref T? one, ref T? two);

        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract string ShowInventory();

        public abstract T? ItemAt(Position pos);

        public abstract void SetThisAsItemOwner(ref T? item);

        public abstract void RemoveItemOwnership(ref T? item);

    }
}
