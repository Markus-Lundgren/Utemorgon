using System.Collections.Immutable;

namespace UtemorgonPOS.Core;

public interface ITransaction
{
	public Customer? Customer { get; }
	public ImmutableArray<OrderRow> AllItems { get; }
	public ImmutableArray<OrderPriceModifier> AllModifiers { get; }
}