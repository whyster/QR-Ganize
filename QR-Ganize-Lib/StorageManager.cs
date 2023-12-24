using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace QR_Ganize_Lib;

public class StorageManager : IStorageManager
{
    private StorageDbContext _dbContext;
    private ILogger _logger;

    public StorageManager(StorageDbContext dbContext, ILogger<StorageManager> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    public async Task<Result<Nothing, CreationError>> CreateItem(string itemName, IEnumerable<int> tagIds,
        int boxId)
    {
        // Check if the box exists
        var existingBox = await _dbContext.Boxes
            .FirstOrDefaultAsync(box => box.BoxId == boxId);

        // Return an error if the box doesn't exist
        if (existingBox is null)
            return new Err<Nothing, CreationError>(CreationError.BoxNotFound);

        // Check if item with the same name and box already exists
        var existingItem =
            await _dbContext.Items.FirstOrDefaultAsync(item => item.Name == itemName && item.Box.BoxId == boxId);

        // Return the existing item if one was found
        if (existingItem != null)
            return new Err<Nothing, CreationError>(CreationError.AlreadyExists);

        // Create a new item
        var item = new Item {Name = itemName, Box = existingBox};

        // Fetch all tags
        var existingTags = await _dbContext.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToListAsync();
        // await DeDuplicateItemTagMaps();

        // Create ItemTagMap for each tag
        var tagMaps = existingTags.Select(tag => new ItemTagMap {Item = item, Tag = tag});

        // Set item tags
        item.Tags = tagMaps.ToList();

        // Add the item to database and save changes
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();


        _logger.LogInformation("Created Item {@Item}", new {ID = item.ItemId, Name = item.Name});
        return new Ok<Nothing, CreationError>(Nothing.AtAll);
    }

    public async Task<Result<Nothing, CreationError>> CreateBox(string boxName, IEnumerable<int> tagIds, int location)
    {
        var existingLocation = await _dbContext.Locations.FindAsync(location);
        if (existingLocation == null) return new Err<Nothing, CreationError>(CreationError.LocationNotFound);

        var existingBox = await _dbContext.Boxes
            .FirstOrDefaultAsync(box => box.Name == boxName && box.Location == existingLocation);

        if (existingBox != null) return new Err<Nothing, CreationError>(CreationError.AlreadyExists);


        var existingTags = await _dbContext.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToListAsync();

        var box = new Box {Name = boxName, Location = existingLocation};

        // TODO: make sure no duplicate boxTagMaps appear
        var tagMaps = existingTags.Select(tag => new BoxTagMap {Box = box, Tag = tag});
        // await DeDuplicateBoxTagMaps();


        box.Tags = tagMaps.ToList();

        await _dbContext.Boxes.AddAsync(box);
        await _dbContext.SaveChangesAsync();
        return new Ok<Nothing, CreationError>(Nothing.AtAll);
    }

    public async Task<Result<Nothing, CreationError>> CreateLocation(string locationName)
    {
        var existingLocation =
            await _dbContext.Locations.FirstOrDefaultAsync(location => location.Name == locationName);

        if (existingLocation != null) return new Err<Nothing, CreationError>(CreationError.AlreadyExists);

        var location = new Location {Name = locationName};
        await _dbContext.Locations.AddAsync(location);
        await _dbContext.SaveChangesAsync();

        return new Ok<Nothing, CreationError>(Nothing.AtAll);
    }

    public async Task<Result<Nothing, CreationError>> CreateTag(string tagName)
    {
        var existingTag = await _dbContext.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);

        if (existingTag != null) return new Err<Nothing, CreationError>(CreationError.AlreadyExists);

        // If tag doesn't exist, then create a new tag
        var tag = new Tag {Name = tagName};

        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        return new Ok<Nothing, CreationError>(Nothing.AtAll);
    }

    public async Task<IEnumerable<Tag>> GetTags(IEnumerable<string> tagNames)
    {
        if (!tagNames.Any()) return await _dbContext.Tags.ToListAsync();

        return await _dbContext.Tags.Where(tag => tagNames.Contains(tag.Name)).ToListAsync();
    }

    public async Task<IEnumerable<Box>> GetBoxes()
    {
        return await _dbContext.Boxes.Include(box => box.Location).Include(box => box.Tags)
            .ThenInclude(boxTagMap => boxTagMap.Tag).ToListAsync();
    }
}