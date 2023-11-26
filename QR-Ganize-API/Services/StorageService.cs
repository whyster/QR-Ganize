
using Google.Protobuf.Collections;
using Grpc.Core;
using QR_Ganize_Lib;

namespace QR_Ganize_API.Services;

public class StorageService : Store.StoreBase
{
    private StorageManager _storageManager;
    private ILogger _logger;

    public StorageService(StorageManager storageManager, ILogger<StorageService> logger)
    {
        _storageManager = storageManager;
        _logger = logger;
    }



    public override async Task<GetTagReply> GetTags(GetTagRequest request, ServerCallContext context)
    {
        var tags = await _storageManager.GetTags(request.TagNames);
        return new GetTagReply {Tags = { tags.Select(tag => new Tag {Id = tag.TagId, Name = tag.Name}) }};
    }
    
    public override async Task<Tag> CreateTag(CreateTagRequest request, ServerCallContext context)
    {
        QR_Ganize_Lib.Tag tag = await _storageManager.CreateTag(request.Name);
        // await _storageManager.CreateTag(request.Name);
        return new Tag {Id = tag.TagId, Name = tag.Name};
    }

    public override async Task<Location> CreateLocation(CreateLocationRequest request, ServerCallContext context)
    {
        var location = await _storageManager.CreateLocation(request.Name);
        return new Location {Id = location.LocationId, Name = location.Name};
    }

    public override async Task<Box> CreateBox(CreateBoxRequest request, ServerCallContext context)
    {
        var box = await _storageManager.CreateBox(request.Name, request.TagIds, request.LocationId);

        return new Box
        {
            Id = box.BoxId, Location = new Location {Id = box.Location.LocationId, Name = box.Location.Name},
            Tags = {box.Tags.Select(tag => new Tag {Id = tag.Tag.TagId, Name = tag.Tag.Name})}
        };
    }

    public override async Task<Item> CreateItem(CreateItemRequest request, ServerCallContext context)
    {
        QR_Ganize_Lib.Item item = await _storageManager.CreateItem(request.Name, request.TagIds, request.BoxId);
        _logger.LogInformation("{@Item}", item);
        _logger.LogInformation("Item Tag Count: {TagCount}", item.Tags.Count);
        var itemTags = item.Tags.Select(map =>
        {
            // _logger.LogInformation("ID: {TagID},\tName: {TagName}", map.Tag.TagId, map.Tag.Name);
            return new Tag {Id = map.Tag.TagId, Name = map.Tag.Name};
        });
        _logger.LogInformation("{ItemBoxName}", item.Box.Name);
        var boxTags = item.Box.Tags.Select(map => new Tag {Id = map.Tag.TagId, Name = map.Tag.Name}).ToList();
        var location = new Location {Id = item.Box.Location.LocationId, Name = item.Box.Location.Name};
        var box = new Box {Id = item.Box.BoxId, Name = item.Box.Name, Location = location, Tags = {boxTags}};
        
        
        return new Item {Name = item.Name, Id = item.ItemId, Box = box, Tags = {itemTags}};
    }
}