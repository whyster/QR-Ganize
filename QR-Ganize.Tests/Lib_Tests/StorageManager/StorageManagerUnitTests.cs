using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QR_Ganize_Lib;

namespace QR_Ganize.Tests.Lib_Tests.StorageManager;

public abstract class CreateTests
{
    private static ILogger<T> CreateConsoleLogger<T>()
    {
        var loggerFactory = LoggerFactory.Create(c => c.AddConsole());
        return loggerFactory.CreateLogger<T>();
    }

    private static StorageDbContext CreateInMemoryDbContext(SqliteConnection connection)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StorageDbContext>();
        optionsBuilder.UseSqlite(connection);
        return new StorageDbContext(optionsBuilder.Options);
    }

    private static int InsertTag(SqliteConnection connection, int tagId, string tagName)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Tags (TagId, Name) VALUES ($tagId, $tagName)";
        command.Parameters.AddWithValue("$tagId", tagId);
        command.Parameters.AddWithValue("$tagName", tagName);
        return command.ExecuteNonQuery();
    }

    private static int InsertLocation(SqliteConnection connection, int locationId, string locationName)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Locations (LocationId, Name) VALUES ($locationId, $locationName)";
        command.Parameters.AddWithValue("$locationId", locationId);
        command.Parameters.AddWithValue("$locationName", locationName);
        return command.ExecuteNonQuery();
    }

    private static int InsertBoxNoTags(SqliteConnection connection, int boxId, string boxName, int locationId)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Boxes (BoxId, Name, LocationId) VALUES ($boxId, $boxName, $locationId)";
        command.Parameters.AddWithValue("$boxId", boxId);
        command.Parameters.AddWithValue("$boxName", boxName);
        command.Parameters.AddWithValue("$locationId", locationId);
        return command.ExecuteNonQuery();
    }

    private static int InsertItemNoTags(SqliteConnection connection, int itemId, string itemName, int boxId)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Items (ItemId, Name, BoxId) VALUES ($itemId, $itemName, $boxId)";
        command.Parameters.AddWithValue("$itemId", itemId);
        command.Parameters.AddWithValue("$itemName", itemName);
        command.Parameters.AddWithValue("$boxId", boxId);
        return command.ExecuteNonQuery();
    }

    public class CreateTag
    {
        #region CreateTag Tests

        [Fact]
        public async Task CreateTag_UniqueTag_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var result = await storageManager.CreateTag("TagName");
            var count = await dbContext.Tags.Where(tag => tag.Name == "TagName").CountAsync();

            // Assert
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CreateTag_ExistingTag_ReturnsAlreadyExistsError()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var tagsInitialized = InsertTag(sqliteConnection, 1, "TagName");
            var result = await storageManager.CreateTag("TagName");
            var count = await dbContext.Tags.Where(tag => tag.Name == "TagName").CountAsync();

            // Assert
            Assert.Equal(1, tagsInitialized);
            Assert.IsType<Err<Nothing, CreationError>>(result);
            Assert.Equal(CreationError.AlreadyExists, (result as Err<Nothing, CreationError>)!.Error);
            Assert.Equal(1, count);
        }

        #endregion
    }

    public class CreateLocation
    {
        #region CreateLocation Tests

        [Fact]
        public async Task CreateLocation_UniqueLocation_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var result = await storageManager.CreateLocation("LocationName");
            var count = await dbContext.Locations.Where(location => location.Name == "LocationName").CountAsync();

            // Assert
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CreateTag_ExistingTag_ReturnsAlreadyExistsError()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var tagsInitialized = InsertLocation(sqliteConnection, 1, "LocationName");
            var result = await storageManager.CreateLocation("LocationName");
            var count = await dbContext.Locations.Where(location => location.Name == "LocationName").CountAsync();

            // Assert
            Assert.Equal(1, tagsInitialized);
            Assert.IsType<Err<Nothing, CreationError>>(result);
            Assert.Equal(CreationError.AlreadyExists, (result as Err<Nothing, CreationError>)!.Error);
            Assert.Equal(1, count);
        }

        #endregion
    }

    public class CreateBox
    {
        #region CreateBox Tests

        [Fact]
        public async Task CreateBox_UniqueBoxNoTags_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var newLocations = InsertLocation(sqliteConnection, 1, "LocationName");
            var result = await storageManager.CreateBox("BoxName", new List<int>(), 1);
            var boxCount = await dbContext.Boxes.Where(box => box.Name == "BoxName" && box.Location.LocationId == 1)
                .CountAsync();
            var boxTagMapCount = await dbContext.BoxTagMap.CountAsync();

            // Assert
            Assert.Equal(1, newLocations);
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, boxCount);
            Assert.Equal(0, boxTagMapCount);
        }

        [Fact]
        public async Task CreateBox_UniqueBoxWithTags_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var newLocations = InsertLocation(sqliteConnection, 1, "LocationName");
            var newTags = InsertTag(sqliteConnection, 1, "TagName");
            var result = await storageManager.CreateBox("BoxName", new[] {1}, 1);
            var boxCount = await dbContext.Boxes.Where(box => box.Name == "BoxName" && box.Location.LocationId == 1)
                .CountAsync();
            var boxTagMapCount = await dbContext.BoxTagMap.Where(map => map.Box.Name == "BoxName" && map.TagId == 1)
                .CountAsync();

            // Assert
            Assert.Equal(1, newLocations);
            Assert.Equal(1, newTags);
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, boxCount);
            Assert.Equal(1, boxTagMapCount);
        }

        [Fact]
        public async Task CreateBox_WithMultipleUniqueBoxes_ShouldReturnOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var newLocations = InsertLocation(sqliteConnection, 1, "LocationName");
            var newBox = InsertBoxNoTags(sqliteConnection, 1, "UniqueBox", 1);

            var result = await storageManager.CreateBox("BoxName", new List<int>(), 1);
            var boxCount = await dbContext.Boxes.Where(box => box.Name == "BoxName" && box.Location.LocationId == 1)
                .CountAsync();
            var boxTagMapCount = await dbContext.BoxTagMap.CountAsync();

            // Assert
            Assert.Equal(1, newLocations);
            Assert.Equal(1, newBox);
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, boxCount);
            Assert.Equal(0, boxTagMapCount);
        }

        [Fact]
        public async Task CreateBox_AlreadyExistsNoTags_ReturnsAlreadyExistsError()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act
            var newLocations = InsertLocation(sqliteConnection, 1, "LocationName");
            var newBoxes = InsertBoxNoTags(sqliteConnection, 1, "BoxName", 1);

            var result = await storageManager.CreateBox("BoxName", new List<int>(), 1);
            var boxCount = await dbContext.Boxes.Where(box => box.Name == "BoxName" && box.Location.LocationId == 1)
                .CountAsync();

            // Assert
            Assert.Equal(1, newLocations);
            Assert.Equal(1, newBoxes);
            Assert.IsType<Err<Nothing, CreationError>>(result);
            Assert.Equal(CreationError.AlreadyExists, (result as Err<Nothing, CreationError>)!.Error);
            Assert.Equal(1, boxCount);
        }


        [Fact]
        public async Task CreateBox_LocationDoesNotExist_ReturnsLocationNotFoundError()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();

            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            // Act

            var result = await storageManager.CreateBox("BoxName", new List<int>(), 1);
            var boxCount = await dbContext.Boxes.Where(box => box.Name == "BoxName" && box.Location.LocationId == 1)
                .CountAsync();

            // Assert
            Assert.IsType<Err<Nothing, CreationError>>(result);
            Assert.Equal(CreationError.LocationNotFound, (result as Err<Nothing, CreationError>)!.Error);
            Assert.Equal(0, boxCount);
        }

        #endregion
    }

    public class CreateItem
    {
        #region CreateItem Tests

        // Test normal use case with valid inputs

        [Fact]
        public async Task CreateItem_UniqueItemNoTags_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();
            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            var itemName = "ItemName";

            // Act
            var locationInitialized = InsertLocation(sqliteConnection, 1, "LocationName");
            var boxInitialized = InsertBoxNoTags(sqliteConnection, 1, "BoxName", 1);

            var result = await storageManager.CreateItem(itemName, new List<int>(), 1);
            var count = await dbContext.Items.Where(item => item.Name == itemName && item.Box.BoxId == 1).CountAsync();

            // Assert
            Assert.Equal(1, locationInitialized);
            Assert.Equal(1, boxInitialized);
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CreateItem_UniqueItemWithTags_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();
            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            var itemName = "ItemName";

            // Act
            var locationInitialized = InsertLocation(sqliteConnection, 1, "LocationName");
            var boxInitialized = InsertBoxNoTags(sqliteConnection, 1, "BoxName", 1);
            var tagsInitialized = InsertTag(sqliteConnection, 1, "TagName");

            var result = await storageManager.CreateItem(itemName, new[] {1}, 1);
            var itemCount = await dbContext.Items.Where(item => item.Name == itemName && item.Box.BoxId == 1)
                .CountAsync();
            var tagMapCount = await dbContext.ItemTagMap
                .Where(map => map.Item.Name == itemName && map.Item.Box.BoxId == 1).CountAsync();


            // Assert
            Assert.Equal(1, locationInitialized);
            Assert.Equal(1, tagsInitialized);
            Assert.Equal(1, boxInitialized);
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, itemCount);
            Assert.Equal(1, tagMapCount);
        }

        [Fact]
        public async Task CreateItem_WithMultipleUniqueItems_ReturnsOK()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();
            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            var itemName = "ItemName";

            // Act
            var locationInitialized = InsertLocation(sqliteConnection, 1, "LocationName");
            var boxInitialized = InsertBoxNoTags(sqliteConnection, 1, "BoxName", 1);
            var itemInitialized = InsertItemNoTags(sqliteConnection, 1, "UniqueItem", 1);

            var result = await storageManager.CreateItem(itemName, new List<int>(), 1);
            var count = await dbContext.Items.Where(item => item.Name == itemName && item.Box.BoxId == 1).CountAsync();

            // Assert
            Assert.Equal(1, locationInitialized);
            Assert.Equal(1, boxInitialized);
            Assert.Equal(1, itemInitialized);
            Assert.IsType<Ok<Nothing, CreationError>>(result);
            Assert.Equal(1, count);
        }


        // Test missing box
        [Fact]
        public async Task CreateItem_MissingBox_ReturnsBoxNotFoundError()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();
            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            var itemName = "ItemName";
            var invalidBoxId = 1;

            // Act
            var result = await storageManager.CreateItem(itemName, new List<int>(), invalidBoxId);

            // Assert  
            Assert.IsType<Err<Nothing, CreationError>>(result);
            Assert.Equal(CreationError.BoxNotFound, (result as Err<Nothing, CreationError>)!.Error);
        }

        // Test already exists

        [Fact]
        public async Task CreateItem_AlreadyExists_ReturnsAlreadyExistsError()
        {
            // Arrange
            var logger = CreateConsoleLogger<QR_Ganize_Lib.StorageManager>();

            await using var sqliteConnection = new SqliteConnection("Data Source=:memory:");
            sqliteConnection.Open();
            await using var dbContext = CreateInMemoryDbContext(sqliteConnection);
            await dbContext.Database.EnsureCreatedAsync();
            var storageManager = new QR_Ganize_Lib.StorageManager(dbContext, logger);

            var itemName = "ItemName";

            // Act
            var locationInitialized = InsertLocation(sqliteConnection, 1, "LocationName");
            var boxInitialized = InsertBoxNoTags(sqliteConnection, 1, "BoxName", 1);
            var itemInitialized = InsertItemNoTags(sqliteConnection, 1, itemName, 1);

            var result = await storageManager.CreateItem(itemName, new List<int>(), 1);
            var itemCount = await dbContext.Items.Where(item => item.Name == itemName && item.Box.BoxId == 1)
                .CountAsync();
            var tagMapCount = await dbContext.ItemTagMap
                .Where(map => map.Item.Name == itemName && map.Item.Box.BoxId == 1).CountAsync();


            // Assert
            Assert.Equal(1, locationInitialized);
            Assert.Equal(1, boxInitialized);
            Assert.Equal(1, itemInitialized);
            Assert.IsType<Err<Nothing, CreationError>>(result);
            Assert.Equal(CreationError.AlreadyExists, (result as Err<Nothing, CreationError>)!.Error);
            Assert.Equal(1, itemCount);
            Assert.Equal(0, tagMapCount);
        }

        #endregion
    }
}