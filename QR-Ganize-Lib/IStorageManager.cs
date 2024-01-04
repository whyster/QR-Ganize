namespace QR_Ganize_Lib;

public enum CreationError
{
    AlreadyExists,
    BoxNotFound,
    LocationNotFound
}

public interface IStorageManager
{
    public Task<Result<Nothing, CreationError>> CreateItem(string itemName, IEnumerable<int> tagIds, int boxId);

    public Task<Result<Nothing, CreationError>> CreateBox(string boxName, IEnumerable<int> tagIds,
        int location);

    public Task<Result<Nothing, CreationError>> CreateLocation(string locationName);

    public Task<Result<Nothing, CreationError>> CreateTag(string tagName);

    public Task<IEnumerable<Tag>> GetTags(IEnumerable<string> tagNames);
    public Task<IEnumerable<Box>> GetBoxes();
    public Task<IEnumerable<Location>> GetLocations();
    public Task<IEnumerable<Item>> GetItems();
}