
using Google.Protobuf.Collections;
using Grpc.Core;
using QR_Ganize_Lib;

namespace QR_Ganize_API.Services;

public class StorageService : Store.StoreBase
{
    private readonly IStorageManager _storageManager;
    private ILogger _logger;

    public StorageService(IStorageManager storageManager, ILogger<StorageService> logger)
    {
        _storageManager = storageManager;
        _logger = logger;
    }

    public override async Task<GetTagReply> GetTags(GetTagRequest request, ServerCallContext context)
    {
        var tags = await _storageManager.GetTags(request.TagNames);
        return new GetTagReply {Tags = { tags.Select(tag => new Tag {Id = tag.TagId, Name = tag.Name}) }};
    }
    
    public override async Task<CreateTagReply> CreateTag(CreateTagRequest request, ServerCallContext context)
    {
        var result = await _storageManager.CreateTag(request.Name);
        switch (result)
        {
            case Err<Nothing, CreationError> err:
                throw err.Error switch
                {
                    CreationError.AlreadyExists => new RpcException(new Status(StatusCode.AlreadyExists,
                        "Tag with given name already exists")),
                    _ => throw new ArgumentOutOfRangeException()
                };
            case Ok<Nothing, CreationError> ok:
                return new CreateTagReply();
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }

    public override async Task<CreateLocationReply> CreateLocation(CreateLocationRequest request, ServerCallContext context)
    {
        var result = await _storageManager.CreateLocation(request.Name);
        switch (result)
        {
            case Err<Nothing, CreationError> err:
                throw err.Error switch
                {
                    CreationError.AlreadyExists => new RpcException(new Status(StatusCode.AlreadyExists,
                        "Location with given name already exists")),
                    _ => throw new ArgumentOutOfRangeException()
                };
            case Ok<Nothing, CreationError> ok:
                return new CreateLocationReply();
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }

    public override async Task<CreateBoxReply> CreateBox(CreateBoxRequest request, ServerCallContext context) 
    {
        var result = await _storageManager.CreateBox(request.Name, request.TagIds, request.LocationId);
        switch (result)
        {
            case Err<Nothing, CreationError> err:
                throw err.Error switch
                {
                    CreationError.AlreadyExists => new RpcException(new Status(StatusCode.AlreadyExists,
                        "Box with given name and location already exists")),
                    CreationError.LocationNotFound => new RpcException(new Status(StatusCode.NotFound,
                        "Given location was not found")),
                    _ => throw new ArgumentOutOfRangeException()
                };
            case Ok<Nothing, CreationError> ok:
                return new CreateBoxReply();
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }

    public override async Task<CreateItemReply> CreateItem(CreateItemRequest request, ServerCallContext context)
    {
        var result = await _storageManager.CreateItem(request.Name, request.TagIds, request.BoxId);
        switch (result)
        {
            case Err<Nothing, CreationError> err:
                throw err.Error switch
                {
                    CreationError.AlreadyExists => new RpcException(new Status(StatusCode.AlreadyExists,
                        "Item with given name and box already exists")),
                    CreationError.BoxNotFound => new RpcException(new Status(StatusCode.NotFound,
                        "Given box was not found")),
                    _ => new ArgumentOutOfRangeException()
                };
            case Ok<Nothing, CreationError> ok:
                return new CreateItemReply();
            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}