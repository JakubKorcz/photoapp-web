namespace PhotoApp.Common.ModelsShared
{
    public class MenuItem
    {
        public required string Description { get; set; }
        public required string IconName { get; set; }
        // Pomocne pole które pokazuje opis po najechaniu na element 
        public string? OnHoverText { get; set; }
        // Pomocne pole które przechowuje konkretny enum do identyfikacji elelmentu 
        public int Id { get; set; }

        public MenuItem()
        {
            Description = string.Empty;
            IconName = string.Empty;
            OnHoverText = string.Empty;
        }
    }
}
