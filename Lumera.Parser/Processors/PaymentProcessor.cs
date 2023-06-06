using Lumera.Parser.Dto;
using Lumera.Parser.Extensions;

namespace Lumera.Parser.Processors
{
    public class PaymentProcessor : ProcessorBase
    {
        enum ParserState
        {
            Idle,
            Payment,
        }
        class State
        {
            public ParserState Parser { get; set; } = ParserState.Idle;
            public PaymentStart? PaymentStart { get; set; }
            public List<Payment> Payments { get; set; } = new();
        }
        State _state = new();

        private readonly IPaymentReceiver _apiEndpoint;

        public PaymentProcessor(IPaymentReceiver apiEndpoint)
        {
            _apiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));
        }
        protected override void OnProcessLine(string line)
        {
            switch (_state.Parser)
            {
                case ParserState.Idle:
                    _state.PaymentStart = line.ToPaymentStart();
                    _state.Parser = ParserState.Payment;
                    break;
                case ParserState.Payment:
                    _state.Payments.Add(line.ToPayment());
                    break;
                default:
                    throw new InvalidDataException($"Invalid state {_state.Parser}");
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
            _apiEndpoint.StartPaymentBundle(_state.PaymentStart!.AccountNumber.ToCanonicalString(), _state.PaymentStart!.PaymentDate.ToDateTime(TimeOnly.MinValue), _state.PaymentStart!.Currency);
            foreach (var payment in _state.Payments)
            {
                _apiEndpoint.Payment(payment.Amount, payment.Reference);
            }
            _apiEndpoint.EndPaymentBundle();
        }
        void EnsureValidState()
        {
            EnsureNotNull(_state.PaymentStart, "No Start record read");
            if (_state.PaymentStart.RecordCount != _state.Payments.Count)
            {
                throw new InvalidDataException($"Number of payments read {_state.Payments.Count} does not match start record count {_state.PaymentStart.RecordCount}");
            }
            var totalAmount = _state.Payments.Sum(x => x.Amount);
            if (_state.PaymentStart.TotalAmount != totalAmount)
            {
                throw new InvalidDataException($"Sum of payments read {totalAmount} does not match start record amount {_state.PaymentStart!.TotalAmount}");
            }
        }
    }
}
