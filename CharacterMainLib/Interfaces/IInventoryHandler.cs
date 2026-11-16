using CharacterMainLib;
using System.Collections.Generic;
/// <summary>
/// Represents a generic inventory container.
/// </summary>
/// <typeparam name="T">
/// The type of items stored in the inventory.
/// Must be a reference type.
/// </typeparam>
/// <typeparam name="P">
/// The type used to identify an item's position.
/// Can be an index, coordinates, or another position type.
/// </typeparam>
public interface IInventoryHandler<T> : IEnumerable<T>
    where T : class, IUsable
{

    public void SetThisAsItemOwner(ref T? item);
    public void RemoveItemOwnership(ref T? item);

    public T? AddItem(T item);
    public T? RemoveItemBySearch(T item);
    public void SwapItems(ref T? one, ref T? two);

    public string ShowInventory();

}