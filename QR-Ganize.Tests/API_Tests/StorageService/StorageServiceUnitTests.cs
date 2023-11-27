using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using QR_Ganize_API;
using QR_Ganize_Lib;

namespace QR_Ganize.Tests.API_Tests.StorageService;

public class TestResult<TSuccess, TError> : Result<TSuccess, TError>;

public abstract class CreateTests
{
    private static ILogger<T> CreateConsoleLogger<T>()
    {
        var loggerFactory = LoggerFactory.Create(c => c.AddConsole());
        return loggerFactory.CreateLogger<T>();
    }

    private static ServerCallContext CreateNullCallContext()
    {
        return TestServerCallContext.Create(null,
            null,
            default,
            null,
            default,
            null,
            null,
            null,
            null,
            null,
            null);
    }

    public class CreateItem
    {
        #region CreateItem Tests

        [Fact]
        public async Task CreateItem_UniqueBoxName_ReturnsOK()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager
                .Setup(m => m.CreateItem(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(
                    new Ok<Nothing, CreationError>(Nothing.AtAll) as Result<Nothing, CreationError>));

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createItemRequest = new CreateItemRequest {Name = "Unique Box Name", BoxId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var reply = await storageService.CreateItem(createItemRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateItem(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()),
                Times.Once());
            Assert.Equal(StatusCode.OK, callContext.Status.StatusCode);
        }

        [Theory]
        [InlineData(CreationError.AlreadyExists, StatusCode.AlreadyExists,
            "Item with given name and box already exists")]
        [InlineData(CreationError.BoxNotFound, StatusCode.NotFound,
            "Given box was not found")]
        public async Task CreateItem_ExpectedErrors_ThrowsRPCException(CreationError error,
            StatusCode expectedStatusCode, string expectedDetail)
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateItem(It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(
                    Task.FromResult(
                        new Err<Nothing, CreationError>(error) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createItemRequest = new CreateItemRequest {Name = "Test Box Name", BoxId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateItem(createItemRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateItem(It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<int>()), Times.Once());
            var ex = await Assert.ThrowsAsync<RpcException>(() => task);
            Assert.Equal(expectedStatusCode, ex.StatusCode);
            Assert.Equal(expectedDetail, ex.Status.Detail);
        }

        [Theory]
        [InlineData(CreationError.LocationNotFound)]
        public async Task CreateItem_UnexpectedError_ThrowsException(CreationError error)
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateItem(It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(
                    Task.FromResult(
                        new Err<Nothing, CreationError>(error) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createItemRequest = new CreateItemRequest {Name = "Test Box Name", BoxId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateItem(createItemRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateItem(It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<int>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        [Fact]
        public async Task CreateItem_UnexpectedResultType_ThrowsException()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateItem(It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(
                    Task.FromResult(
                        new TestResult<Nothing, CreationError>() as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createItemRequest = new CreateItemRequest {Name = "Test Box Name", BoxId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateItem(createItemRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateItem(It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<int>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        #endregion
    }

    public class CreateBox
    {
        #region CreateBox Tests

        [Fact]
        public async Task CreateBox_UniqueBoxName_ReturnsOK()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager
                .Setup(m => m.CreateBox(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(
                    new Ok<Nothing, CreationError>(Nothing.AtAll) as Result<Nothing, CreationError>));

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createBoxRequest = new CreateBoxRequest {Name = "Unique Box Name", LocationId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var reply = await storageService.CreateBox(createBoxRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateBox(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()),
                Times.Once());
            Assert.Equal(StatusCode.OK, callContext.Status.StatusCode);
        }

        [Theory]
        [InlineData(CreationError.AlreadyExists, StatusCode.AlreadyExists,
            "Box with given name and location already exists")]
        [InlineData(CreationError.LocationNotFound, StatusCode.NotFound,
            "Given location was not found")]
        public async Task CreateBox_ExpectedErrors_ThrowsRPCException(CreationError error,
            StatusCode expectedStatusCode, string expectedDetail)
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateBox(It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(
                    Task.FromResult(
                        new Err<Nothing, CreationError>(error) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createBoxRequest = new CreateBoxRequest {Name = "Test Box Name", LocationId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateBox(createBoxRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateBox(It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<int>()), Times.Once());
            var ex = await Assert.ThrowsAsync<RpcException>(() => task);
            Assert.Equal(expectedStatusCode, ex.StatusCode);
            Assert.Equal(expectedDetail, ex.Status.Detail);
        }

        [Theory]
        [InlineData(CreationError.BoxNotFound)]
        public async Task CreateBox_UnexpectedError_ThrowsException(CreationError error)
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateBox(It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(
                    Task.FromResult(
                        new Err<Nothing, CreationError>(error) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createBoxRequest = new CreateBoxRequest {Name = "Test Box Name", LocationId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateBox(createBoxRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateBox(It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<int>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        [Fact]
        public async Task CreateBox_UnexpectedResultType_ThrowsException()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateBox(It.IsAny<string>(),
                    It.IsAny<IEnumerable<int>>(),
                    It.IsAny<int>()))
                .Returns(
                    Task.FromResult(
                        new TestResult<Nothing, CreationError>() as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createBoxRequest = new CreateBoxRequest {Name = "Test Box Name", LocationId = 1, TagIds = {1, 2}};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateBox(createBoxRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateBox(It.IsAny<string>(),
                It.IsAny<IEnumerable<int>>(),
                It.IsAny<int>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        #endregion
    }

    public class CreateLocation
    {
        #region CreateLocation Tests

        [Fact]
        public async Task CreateLocation_UniqueLocationName_ReturnsOK()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateLocation(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Ok<Nothing, CreationError>(Nothing.AtAll) as Result<Nothing, CreationError>));

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createLocationRequest = new CreateLocationRequest {Name = "Unique Location Name"};
            var callContext = CreateNullCallContext();

            // Act
            var reply = await storageService.CreateLocation(createLocationRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateLocation(It.IsAny<string>()), Times.Once());
            Assert.Equal(StatusCode.OK, callContext.Status.StatusCode);
        }

        [Fact]
        public async Task CreateLocation_AlreadyExists_ThrowsRPCException()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateLocation(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(
                        new Err<Nothing, CreationError>(CreationError
                            .AlreadyExists) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createLocationRequest = new CreateLocationRequest {Name = "TestLocationName"};
            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateLocation(createLocationRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateLocation(It.IsAny<string>()), Times.Once());
            var ex = await Assert.ThrowsAsync<RpcException>(() => task);
            Assert.Equal(StatusCode.AlreadyExists, ex.StatusCode);
            Assert.Equal("Location with given name already exists", ex.Status.Detail);
        }

        [Theory]
        [InlineData(CreationError.BoxNotFound)]
        [InlineData(CreationError.LocationNotFound)]
        public async Task CreateLocation_UnexpectedError_ThrowsException(CreationError error)
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateLocation(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new Err<Nothing, CreationError>(error) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createLocationRequest = new CreateLocationRequest {Name = "TestLocation"};

            var callContext = CreateNullCallContext();

            // Act
            var task = storageService.CreateLocation(createLocationRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateLocation(It.IsAny<string>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        [Fact]
        public async Task CreateLocation_UnexpectedResultType_ThrowsException()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateLocation(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new TestResult<Nothing, CreationError>() as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createLocationRequest = new CreateLocationRequest {Name = "TestTagName"};

            var callContext = CreateNullCallContext();


            // Act
            var task = storageService.CreateLocation(createLocationRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateLocation(It.IsAny<string>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        #endregion
    }

    public class CreateTag
    {
        #region CreateTag Tests

        [Fact]
        public async Task CreateTag_UniqueTagName_ReturnsOK()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateTag(It.IsAny<string>()))
                .Returns(Task.FromResult(
                    new Ok<Nothing, CreationError>(Nothing.AtAll) as Result<Nothing, CreationError>));

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createTagRequest = new CreateTagRequest {Name = "Unique Tag Name"};

            var callContext = CreateNullCallContext();


            // Act
            var reply = await storageService.CreateTag(createTagRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateTag(It.IsAny<string>()), Times.Once());
            Assert.Equal(StatusCode.OK, callContext.Status.StatusCode);
        }

        [Fact]
        public async Task CreateTag_AlreadyExists_ThrowsRPCException()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateTag(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(
                        new Err<Nothing, CreationError>(CreationError
                            .AlreadyExists) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createTagRequest = new CreateTagRequest {Name = "TestTagName"};

            var callContext = CreateNullCallContext();


            // Act
            var task = storageService.CreateTag(createTagRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateTag(It.IsAny<string>()), Times.Once());
            var ex = await Assert.ThrowsAsync<RpcException>(() => task);
            Assert.Equal(StatusCode.AlreadyExists, ex.StatusCode);
            Assert.Equal("Tag with given name already exists", ex.Status.Detail);
        }

        [Theory]
        [InlineData(CreationError.BoxNotFound)]
        [InlineData(CreationError.LocationNotFound)]
        public async Task CreateTag_UnexpectedError_ThrowsException(CreationError error)
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateTag(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new Err<Nothing, CreationError>(error) as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createTagRequest = new CreateTagRequest {Name = "TestTagName"};

            var callContext = CreateNullCallContext();


            // Act
            var task = storageService.CreateTag(createTagRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateTag(It.IsAny<string>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        [Fact]
        public async Task CreateTag_UnexpectedResultType_ThrowsException()
        {
            // Arrange
            var mockStorageManager = new Mock<IStorageManager>();
            var logger = CreateConsoleLogger<QR_Ganize_API.Services.StorageService>();

            mockStorageManager.Setup(m => m.CreateTag(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(new TestResult<Nothing, CreationError>() as Result<Nothing, CreationError>)
                );

            var storageService = new QR_Ganize_API.Services.StorageService(mockStorageManager.Object, logger);
            var createTagRequest = new CreateTagRequest {Name = "TestTagName"};

            var callContext = CreateNullCallContext();


            // Act
            var task = storageService.CreateTag(createTagRequest, callContext);

            // Assert
            mockStorageManager.Verify(manager => manager.CreateTag(It.IsAny<string>()), Times.Once());
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => task);
        }

        #endregion
    }
}