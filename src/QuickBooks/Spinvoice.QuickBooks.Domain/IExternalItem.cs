﻿namespace Spinvoice.QuickBooks.Domain
{
    public interface IExternalItem
    {
        string Id { get; set; }
        string Name { get; set; }
    }
}