using System;
using System.Collections.Immutable;

namespace UtemorgonPOS.Core;

public interface ITransaction
{
	public Customer? Customer { get; }
	public DateTime CreationDate { get; }
	public ImmutableArray<OrderRow> AllItems { get; }
	public ImmutableArray<OrderPriceModifier> AllModifiers { get; }
	public ImmutableArray<Payment> AllPayments { get; }
}