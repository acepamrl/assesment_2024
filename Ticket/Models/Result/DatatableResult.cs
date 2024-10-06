namespace Ticket.Models.Result
{
    public class DatatableResult
    {
        public DatatableResult()
        {
            RecordsTotal = 0;
            RecordsFiltered = 0;
            Data = new List<object>();
        }
        public long RecordsTotal { get; set; }
        public long RecordsFiltered { get; set; }
        public object? Data { get; set; }
    }
}
