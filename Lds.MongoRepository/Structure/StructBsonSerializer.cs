using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Lds.MongoRepository.Structure {
  public class StructBsonSerializer : IBsonSerializer {

    // gør sådan BsonSerializer.RegisterSerializer(typeof(MyStruct), new StructBsonSerializer(typeof(MyStruct)));

    public StructBsonSerializer(Type valueType) {
      ValueType = valueType;
    }

    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) {
      var actualType = ValueType;

      var obj = Activator.CreateInstance(actualType);

      var bsonReader = context.Reader;
      bsonReader.ReadStartDocument();

      while (bsonReader.ReadBsonType() != BsonType.EndOfDocument) {
        var name = bsonReader.ReadName();

        var field = actualType.GetField(name);
        if (field != null) {
          var value = BsonSerializer.Deserialize(bsonReader, field.FieldType);
          field.SetValue(obj, value);
        }

        var prop = actualType.GetProperty(name);
        if (prop != null) {
          var value = BsonSerializer.Deserialize(bsonReader, prop.PropertyType);
          prop.SetValue(obj, value, null);
        }
      }

      bsonReader.ReadEndDocument();

      return obj;
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) {
      var nominalType = ValueType;
      var bsonWriter = context.Writer;

      var fields = nominalType.GetFields(BindingFlags.Instance | BindingFlags.Public);
      var propsAll = nominalType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

      var props = new List<PropertyInfo>();
      foreach (var prop in propsAll) {
        if (prop.CanWrite) {
          props.Add(prop);
        }
      }

      bsonWriter.WriteStartDocument();

      foreach (var field in fields) {
        bsonWriter.WriteName(field.Name);
        BsonSerializer.Serialize(bsonWriter, field.FieldType, field.GetValue(value));
      }
      foreach (var prop in props) {
        bsonWriter.WriteName(prop.Name);
        BsonSerializer.Serialize(bsonWriter, prop.PropertyType, prop.GetValue(value, null));
      }

      bsonWriter.WriteEndDocument();
    }

    public Type ValueType { get; }
  }
}
