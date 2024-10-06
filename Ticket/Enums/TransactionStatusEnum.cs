using System.ComponentModel;

namespace Ticket.Enums
{
    public enum TransactionStatusEnum
    {
        Cancelled,
        Success,
        [Description("Waiting Payment")]
        WaitingPayment,
    }
}
