namespace QR_Ganize_Server.Data;

public record Tag(string Name);

public record Box(string Name, string Location, HashSet<Tag>? Tags);

public record Item(string Name, Box Box, HashSet<Tag>? Tags);




public class ItemService
{

    public HashSet<Tag> Tags
    {
        get => _tags;
    }
    private HashSet<Tag> _tags = new HashSet<Tag>();

    public void AddTag(string tagName)
    {
        _tags.Add(new Tag(tagName));
    }
}