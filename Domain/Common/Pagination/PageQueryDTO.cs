namespace MailCrafter.Domain;
public class PageQueryDTO<T>
{
    public int Top { get; set; } = 10;
    public int Skip { get; set; }
    public string? Search { get; set; }
    public string? SearchBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.None;
    public string? SortBy { get; set; }
}
