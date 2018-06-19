using System;
using System.Collections.Generic;
using System.Linq;
using Spinvoice.Domain.Accounting;
using Spinvoice.QuickBooks.Domain;
using Spinvoice.Utils;

namespace Spinvoice.Domain.Invoices
{
    public sealed class PositionAutoFill : IPositionAutoFill
    {
        public Position[] FillPositions(IEnumerable<IInventoryValuationItem> items,
            decimal totalAmount,
            decimal exchangeRate,
            decimal markup)
        {
            var amount = 0m;
            var positions = new List<Position>();
            foreach (var item in items.Where(item => item.Quantity > 0 && item.Amount > 0))
            {
                var positionAmount = Math.Round(item.Amount / exchangeRate * (1 + markup), 2);
                var position = new Position(item.Name, (int)item.Quantity, positionAmount);
                positions.Add(position);

                amount += positionAmount;
                if (amount >= totalAmount)
                {
                    MakeExact(positions, totalAmount);
                    break;
                }
            }

            return positions.ToArray();
        }

        private static void MakeExact(
            List<Position> positions,
            decimal totalAmount)
        {
            var amount = positions.Sum(x => x.Amount);
            var deltaAmount = amount - totalAmount;
            var adjustable = positions.Select(x =>
                {
                    var itemPrice = x.Amount / x.Quantity;
                    var deltaQuantity = (int)Math.Ceiling(deltaAmount / itemPrice);
                    var newQuantity = x.Quantity - deltaQuantity;
                    return new
                    {
                        ItemPrice = itemPrice,
                        Position = x,
                        NewQuatity = newQuantity,
                        DeltaAdjustment = deltaAmount - newQuantity * itemPrice
                    };
                }).Where(x => x.NewQuatity > 0)
                .MinByOrDefault(x => x.DeltaAdjustment);

            if (adjustable == null) return;
            if (totalAmount - positions.Sum(
                    x => x != adjustable.Position 
                        ? x.Amount 
                        : adjustable.ItemPrice * adjustable.NewQuatity) 
                > totalAmount * 0.02m) return;

            adjustable.Position.Quantity = adjustable.NewQuatity;
            adjustable.Position.Amount -= deltaAmount;
        }
    }   
}