# DiscriminatorJsonConverter
JsonConverter for System.Text.Json for polymorphic deserialization based on discriminator

## Example

```csharp
    public class PaymentDtoConverter : DiscriminatorJsonConverter<PaymentDto, PaymentType>
    {
        public PaymentDtoConverter() : base(nameof(PaymentType))
        {
            HasValue<CashPaymentDto>(PaymentType.Cash);
            HasValue<ChequePaymentDto>(PaymentType.Cheque);
            HasValue<TransferPaymentDto>(PaymentType.Transfer);
        }
    }
```
