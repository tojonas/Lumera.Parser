using Lumera.Parser.Dto;
using Lumera.Parser.Extensions;

namespace Lumera.Parser.Processors
{
    public class DepositProcessor : ProcessorBase
    {
        class State
        {
            public DepositStart? DepositStart { get; set; }
            public DepositEnd? DepositEnd { get; set; }
            public List<Deposit> Deposits { get; set; } = new();
        }
        State _state = new();
        const string Currency = "SEK";
        private readonly IPaymentReceiver _apiEndpoint;

        public DepositProcessor(IPaymentReceiver apiEndpoint)
        {
            _apiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));
        }
        protected override void OnProcessLine(string line)
        {
            var token = line[0..2];
            switch (token)
            {
                case "00":
                    EnsureNull(_state.DepositStart, "Multiple Start records detected");
                    _state.DepositStart = line.ToDepositStart();
                    break;
                case "30":
                    EnsureNotNull(_state.DepositStart, "No Start record read");
                    EnsureNull(_state.DepositEnd, "End record read no more records allowed");
                    _state.Deposits.Add(line.ToDeposit());
                    break;
                case "99":
                    EnsureNotNull(_state.DepositStart, "No Start record read");
                    EnsureNull(_state.DepositEnd, "Multiple End records detected");
                    _state.DepositEnd = line.ToDepositEnd();
                    break;
                default:
                    throw new InvalidDataException($"Unknown token [{token}]");
            }
        }

        protected override void OnDisposed()
        {
            try
            {
                EnsureValidState();
                CallExternalApi();
            }
            finally 
            {
                _state = new();
            }
        }

        void CallExternalApi()
        {
            //StartPaymentBundle(string accountNumber, DateOnly paymentDate, string currency);
            // I really don't know what the payment date is supposed to be, so I'm just going to use today's date
            // The file doesn\t contain any date information and I'm just assuming that I can use the same Api with different backends for Deposits AND Payments
            _apiEndpoint.StartPaymentBundle(_state.DepositStart!.AccountNumber.ToCanonicalString(), DateTime.UtcNow, Currency);
            foreach (var deposit in _state.Deposits)
            {
                _apiEndpoint.Payment(deposit.Amount, deposit.Reference);
            }
            _apiEndpoint.EndPaymentBundle();
        }
        void EnsureValidState()
        {
            EnsureNotNull(_state.DepositStart, "No Start record read");
            EnsureNotNull(_state.DepositEnd, "No End record read");

            var totalAmount = _state.Deposits.Sum(x => x.Amount);
            if (_state.DepositEnd.TotalAmount != totalAmount)
            {
                throw new InvalidDataException($"Sum of deposits read {totalAmount} does not match end record amount {_state.DepositEnd.TotalAmount}");
            }
            if (_state.DepositEnd.TotalCount != _state.Deposits.Count)
            {
                throw new InvalidDataException($"Number of deposits read {_state.Deposits.Count} does not match end record number of records {_state.DepositEnd.TotalCount}");
            }
        }
    }
}
