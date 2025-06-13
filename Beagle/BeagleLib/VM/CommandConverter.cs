using Newtonsoft.Json;

namespace BeagleLib.VM;

public class CommandConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Command) || objectType == typeof(Command?);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null) throw new ArgumentException(nameof(value));
        var cmd = (Command)value;

        writer.WriteStartObject();
        writer.WritePropertyName("O");
        serializer.Serialize(writer, cmd.Operation);

        var operationProperties = cmd.Operation.GetOperationProperties();
        switch (operationProperties.CommandType)
        {
            case CommandTypeEnum.CommandOnly: break;
            case CommandTypeEnum.CommandPlusFloat: writer.WritePropertyName("V"); serializer.Serialize(writer, cmd.ConstValue); break;
            case CommandTypeEnum.CommandPlusIndex: writer.WritePropertyName("V"); serializer.Serialize(writer, cmd.Idx); break;
            default: throw new Exception($"Invalid command type {operationProperties.CommandType}");
        }
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        OpEnum? operation = null;
        float? value = null;

        while (reader.Read())
        {
            if (reader.TokenType != JsonToken.PropertyName) break;

            var propertyName = (string?)reader.Value;
            if (!reader.Read()) continue;

            if (propertyName == "O") operation = serializer.Deserialize<OpEnum>(reader);
            if (propertyName == "V") value = serializer.Deserialize<float>(reader);
        }

        if (operation == null) throw new InvalidDataException("A Command must contain Operation (O)");

        var operationProperties = operation.Value.GetOperationProperties();
        switch (operationProperties.CommandType)
        {
            case CommandTypeEnum.CommandOnly:
            {
                return new Command(operation.Value);
            }
            case CommandTypeEnum.CommandPlusFloat:
            {
                if (value == null) throw new InvalidDataException($"A {operation} Command must contain a float value (V)");
                return new Command(operation.Value, value.Value);
            }
            case CommandTypeEnum.CommandPlusIndex:
            {
                if (value == null) throw new InvalidDataException($"A {operation} Command must contain an int value (V)");
                return new Command(operation.Value, (int)value.Value);
            }
            default:
            {
                throw new Exception($"Invalid command type {operationProperties.CommandType}");
            }
        }
    }
}