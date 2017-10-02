using Spinvoice.QuickBooks.Domain;

namespace Spinvoice.QuickBooks.Item
{
    public class ExternalItem : IExternalItem
    {
        public ExternalItem()
        {
            InternalItem = new Intuit.Ipp.Data.Item();
        }

        public ExternalItem(Intuit.Ipp.Data.Item item)
        {
            InternalItem = item;
        }

        public Intuit.Ipp.Data.Item InternalItem { get; set; }

        public string Id
        {
            get { return InternalItem.Id; }
            set { InternalItem.Id = value; }
        }

        public string Name
        {
            get { return InternalItem.Name; }
            set { InternalItem.Name = value; }
        }
    }
}