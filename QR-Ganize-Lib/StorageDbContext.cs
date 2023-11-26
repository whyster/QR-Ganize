using Microsoft.EntityFrameworkCore;

namespace QR_Ganize_Lib;

public class Tag
{
    public int TagId { get; set; }
    public string Name { get; set; }
}

public class Box
{
    public int BoxId { get; set; }
    public string Name { get; set; }
    public Location Location { get; set; }
    public List<BoxTagMap> Tags { get; set; }
    public List<Item> Items { get; set; }
}

public class BoxTagMap
{
    public int BoxTagMapId { get; set; }
    public int BoxId { get; set; }
    public Box Box { get; set; }
    
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}

public class ItemTagMap
{
   public int ItemTagMapId { get; set; }
   public int ItemId { get; set; }
   public Item Item { get; set; }
   
   public int TagId { get; set; }
   public Tag Tag { get; set; }
}
   

public class Location
{
    public int LocationId { get; set; }
    public string Name { get; set; }
}

public class Item
{
    public int ItemId { get; set; }
    public string Name { get; set; }
    public List<ItemTagMap> Tags { get; set; }
    
    public int BoxId { get; set; }
    public Box Box { get; set; }
}

public class StorageDbContext(DbContextOptions<StorageDbContext> options) : DbContext(options)
{
    public DbSet<Box> Boxes { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<BoxTagMap> BoxTagMap { get; set; }
    public DbSet<ItemTagMap> ItemTagMap { get; set; }
    
}