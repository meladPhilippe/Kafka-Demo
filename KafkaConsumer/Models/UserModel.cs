using Avro.Specific;
using Avro;

namespace KafkaConsumer.Models;

public class UserModel : ISpecificRecord
{
    public static Schema _SCHEMA = Schema.Parse("{\"type\":\"record\",\"name\":\"UserModel\",\"namespace\":\"com.example\",\"fields\":[{\"name\":\"id\",\"type\":\"int\"},{\"name\":\"name\",\"type\":\"string\"}]}");

    public int Id { get; set; }
    public string? Name { get; set; }

    public Schema Schema => _SCHEMA;

    public object? Get(int fieldpos)
    {
        switch (fieldpos)
        {
            case 0: return Id;
            case 1: return Name;
            default: throw new AvroRuntimeException("unknown field position " + fieldpos);
        }
    }

    public void Put(int fieldpos, object value)
    {
        switch (fieldpos)
        {
            case 0: Id = (int)value; break;
            case 1: Name = (string)value; break;
            default: throw new AvroRuntimeException("unknown field position " + fieldpos);
        }
    }
}
