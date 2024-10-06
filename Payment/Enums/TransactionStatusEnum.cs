using System.ComponentModel;

namespace Payment.Enums
{
    public enum TransactionStatusEnum
    {
        Cancelled,
        Success,
        [Description("Waiting Payment")]
        WaitingPayment,
    }
}
