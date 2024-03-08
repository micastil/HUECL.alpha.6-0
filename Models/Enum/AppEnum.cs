namespace HUECL.alpha._6_0.Models
{

    public enum Active
    {
        Active = 1,
        NonActive = 0
    }

    public enum SaleState
    {
        NoDelivery = 1,
        PartialDelivery = 2,
        CompleteDelivery = 3
    }

    public enum DeliveryState
    {
        Empty = 1,
        WithItems = 2,
        WithInvoice = 3
    }

    public enum DeliveryVerify 
    {
        NotFound = 0,
        Found = 1
    }

    public enum PageCustomSize 
    {
        Small = 5,
        Normal = 10,
        High = 15
    }

    public enum InvoiceState 
    { 
        PaymentPending = 0,
        PaymentComplete = 1
    }

    public static class IVARate
    {
        public static readonly Double CL = 0.19;
    }
}
