namespace Domain.Options
{
    public class DatabaseOptions
    {
        public const string PropertyName = "ConnectionStrings";
        public string Default { get; set; } = string.Empty;
    }
}
