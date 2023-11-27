namespace QR_Ganize_Lib;

public enum CreationError
{
    AlreadyExists,
    BoxNotFound,
    LocationNotFound,
}

public interface IStorageManager
{
    public Task<Result<Nothing, CreationError>> CreateItem(string itemName, IEnumerable<Int32> tagIds, Int32 boxId);

    public Task<Result<Nothing, CreationError>> CreateBox(string boxName, IEnumerable<Int32> tagIds,
        Int32 location);

    public Task<Result<Nothing, CreationError>> CreateLocation(string locationName);

    public Task<Result<Nothing, CreationError>> CreateTag(string tagName);

    public Task<IEnumerable<Tag>> GetTags(IEnumerable<string> tagNames);
}