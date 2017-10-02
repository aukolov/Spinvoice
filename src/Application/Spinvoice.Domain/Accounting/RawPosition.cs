namespace Spinvoice.Domain.Accounting
{
    public class RawPosition
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }

        public bool IsFullyInitialized => !string.IsNullOrEmpty(Name)
                                          && !string.IsNullOrEmpty(Quantity)
                                          && !string.IsNullOrEmpty(Amount);

        public override string ToString()
        {
            return string.Format($"Name: {Name}, Quantity: {Quantity}, Amount: {Amount}");
        }
    }
}