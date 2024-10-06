namespace Event.Models.Result
{
    public class GetEventListModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Ticket { get; set; }
    }
}
