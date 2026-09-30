using System;

// A physical delivery belongs to one frozen agreement, not whichever customer is current later.
public sealed class MarketplaceFulfillmentOrder
{
    public TradeTermsSnapshot Agreement { get; }
    public SpiceType Spice { get; }
    public bool IsComplete { get; private set; }

    public MarketplaceFulfillmentOrder(TradeTermsSnapshot agreement)
    {
        if (agreement == null || agreement.Price <= 0 || agreement.QuantityGrams <= 0 ||
            OrderManager.MapSpiceName(agreement.SpiceKey) == SpiceType.None)
            throw new ArgumentException("A fulfillment order requires valid, physically supported accepted terms.", nameof(agreement));
        Agreement = agreement;
        Spice = OrderManager.MapSpiceName(agreement.SpiceKey);
    }

    public bool BelongsTo(LocalTradeState activeTrade, TradeTermsSnapshot agreement)
    {
        return ReferenceEquals(Agreement, agreement) && ReferenceEquals(Agreement.Source, activeTrade) &&
            ReferenceEquals(activeTrade?.AcceptedTerms, Agreement) && !activeTrade.SettlementClaimed;
    }

    public bool TryDeliver(LocalTradeState activeTrade, TradeTermsSnapshot agreement, SpiceType spice, bool scoopFilled)
    {
        if (!BelongsTo(activeTrade, agreement) || IsComplete || spice != Spice || !scoopFilled)
            return false;
        // One filled, matching scoop fulfills the entire frozen agreement; quantity is not physically measured.
        IsComplete = true;
        return true;
    }

    public bool CanComplete(LocalTradeState activeTrade, TradeTermsSnapshot agreement)
    {
        return IsComplete && BelongsTo(activeTrade, agreement);
    }
}
