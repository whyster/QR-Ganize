using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace QR_Ganize_Lib;

public class StorageManager
{
    private StorageDbContext _dbContext;
    private ILogger _logger;

    public StorageManager(StorageDbContext dbContext, ILogger<StorageManager> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }


    /*private async Task DeDuplicateItemTagMaps()
    {
        var duplicateItemTagMaps = _dbContext.ItemTagMap
            .GroupBy(itm => new { itm.ItemId, itm.TagId })
            .Where(g => g.Count() > 1);
        
        foreach (var group in duplicateItemTagMaps)
        {
            // Skip the first record and remove the rest of duplicates in each group
            foreach(var duplicateRecord in group.Skip(1))
            {
                _dbContext.ItemTagMap.Remove(duplicateRecord);
            }
        }
        await _dbContext.SaveChangesAsync();
    }*/

    /*private async Task DeDuplicateBoxTagMaps()
    {
        var duplicateBoxTagMaps = _dbContext.BoxTagMap
            .GroupBy(btm => new { btm.BoxId, btm.TagId })
            .Where(g => g.Count() > 1);

        // Traverse each group of duplicate ItemTagMaps


        // Similar process for duplicate BoxTagMaps
        foreach (var group in duplicateBoxTagMaps)
        {
            foreach(var duplicateRecord in group.Skip(1))
            {
                _dbContext.BoxTagMap.Remove(duplicateRecord);
            }
        }

        await _dbContext.SaveChangesAsync();
    }*/

    public async Task<Item> CreateItem(string itemName, IEnumerable<Int32> tagIds, Int32 boxId)
    {
        // Check if the box exists
        var existingBox = await _dbContext.Boxes.Include(box => box.Tags).ThenInclude(map => map.Tag).Include(box => box.Location).FirstAsync(box => box.BoxId == boxId );

        // Throw an error if the box doesn't exist
        if (existingBox == null)
        {
            throw new KeyNotFoundException($"Box with ID {boxId} does not exist");
        }

        // Check if item with the same name and box already exists
        var existingItem = await _dbContext.Items.Include(item => item.Tags).ThenInclude(item_map => item_map.Tag).Include(item => item.Box).ThenInclude(box => box.Tags).FirstOrDefaultAsync(item => item.Name == itemName && item.Box.BoxId == boxId);

        // Return the existing item if one was found
        if (existingItem != null)
        {
            _logger.LogInformation("Returning existing item");
            return existingItem;
        }
            
        // Create a new item
        var item = new Item { Name = itemName, Box = existingBox };

        // Fetch all tags
        var existingTags = await _dbContext.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToListAsync();
        // await DeDuplicateItemTagMaps();

        // Create ItemTagMap for each tag
        var tagMaps = existingTags.Select(tag => new ItemTagMap { Item = item, Tag = tag });

        // Set item tags
        item.Tags = tagMaps.ToList();
        
        // Add the item to database and save changes
        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        
        _logger.LogInformation("Created Item {Item}", item.Name);
        return item;
    }
    public async Task<Box> CreateBox(string boxName, IEnumerable<Int32> tagIds, Int32 location)
    {

        var existingLocation = await _dbContext.Locations.FindAsync(location);
        if (existingLocation == null)
        {
            throw new KeyNotFoundException($"Location with ID {location} does not exist");
        }
        var existingBox = await _dbContext.Boxes.Include(box => box.Tags).FirstOrDefaultAsync(box => box.Name == boxName && box.Location==existingLocation);

        if (existingBox != null)
        {
            // If item exists, return that item instead of creating a new one
            return existingBox;
        }
        

        // var existingLocation = await _dbContext.Locations.FindAsync(location.LocationId) ?? location;

        var existingTags =  await _dbContext.Tags.Where(tag => tagIds.Contains(tag.TagId)).ToListAsync();

        var box = new Box {Name = boxName, Location = existingLocation};
        
        // TODO: make sure no duplicate boxTagMaps appear
        var tagMaps = existingTags.Select(tag => new BoxTagMap {Box = box, Tag = tag});
        // await DeDuplicateBoxTagMaps();
        
        
        
        box.Tags = tagMaps.ToList();
        
        await _dbContext.Boxes.AddAsync(box);
        await _dbContext.SaveChangesAsync();
        return box;
    }

    public async Task<Location> CreateLocation(string locationName)
    {
        // Check if tag already exists in the database
        var existingTag = await _dbContext.Locations.FirstOrDefaultAsync(location => location.Name == locationName);

        if (existingTag != null)
        {
            // If tag exists, return that tag instead of creating a new one
            return existingTag;
        }

        // If tag doesn't exist, then create a new tag

        Location location = new Location {Name = locationName};
        await _dbContext.Locations.AddAsync(location);
        await _dbContext.SaveChangesAsync();

        return location; // Return the newly created tag
    }
    public async Task<IEnumerable<Tag>> GetTags(IEnumerable<string> tagNames)
    {
        if(!tagNames.Any())
        {
            return await _dbContext.Tags.ToListAsync();
        }

        return await _dbContext.Tags.Where(tag => tagNames.Contains(tag.Name)).ToListAsync();
    }
    public async Task<Tag> CreateTag(string tagName)
    {
        // Check if tag already exists in the database
        var existingTag = await _dbContext.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);

        if (existingTag != null)
        {
            // If tag exists, return that tag instead of creating a new one
            return existingTag;
        }

        // If tag doesn't exist, then create a new tag
        Tag tag = new Tag {Name = tagName};

        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        return tag; // Return the newly created tag    }
    }
}