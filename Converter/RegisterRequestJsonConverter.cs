// RegisterRequestJsonConverter.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using Booking.Enums;
using Booking.DTO;

namespace Booking.Converters
{
    public class RegisterRequestJsonConverter : JsonConverter<RegisterRequest>
    {
        public override RegisterRequest? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("accountType", out var accountTypeProp))
                throw new JsonException("Missing 'accountType' field.");

            if (!Enum.TryParse<AccountType>(accountTypeProp.GetString(), ignoreCase: true, out var accountType))
                throw new JsonException($"Unknown accountType: '{accountTypeProp.GetString()}'.");

            var json = root.GetRawText();

            return accountType switch
            {
                AccountType.Customer => JsonSerializer.Deserialize<CustomerRegisterRequest>(json, options),
                AccountType.AgencyOwner => JsonSerializer.Deserialize<AgencyOwnerRegisterRequest>(json, options),
                _ => throw new JsonException($"Unsupported accountType: {accountType}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            RegisterRequest value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}
