using System;

namespace Scripts.UI.Spinner
{
    public interface ICurrencyProvider
    {
        int GemCount { get; }
        bool CanAfford(int amount);
        bool TrySpend(int amount);

        event Action<int> OnGemsChanged;
    }
}
