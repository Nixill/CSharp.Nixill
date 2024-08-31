using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Nixill.Utils;

namespace Nixill.Collections.Grid.CSV;

/// <summary>
/// This class contains static methods to convert between DataTables and
/// comma separated value format files and text.
/// </summary>
/// <remarks>
/// The parser and formatter follow the specifications laid out in
/// <a href="https://tools.ietf.org/html/rfc4180">RFC 4180</a>, with the
/// following exceptions:
///
/// <list type="bullet">
/// <item><description>
/// When parsing, records may be separated by CRLF, CR, or LF (but not
/// LFCR).
/// </description></item>
/// <item><description>
/// When parsing, two consecutive double quotes always produces a double
/// quote character, even when not enclosed in quotes.
/// </description></item>
/// <item><description>
/// When formatting, records are separated by LF, not CRLF.
/// </description></item>
/// </list>
/// </remarks>
public static class DataTableCSVParser
{
  static readonly Dictionary<Type, Func<string, object>> Deserializers;
  static readonly Dictionary<Type, Func<object, string>> Serializers;

  static DataTableCSVParser()
  {
    Deserializers = [];
    Serializers = [];
  }

  /// <summary>Parses a <see cref="DataTable"/> from a file.</summary>
  /// <param name="path">The path to the file in question.</param>
  /// <param name="columnDefs">
  ///   Definitions of columns in the file. Columns not specified default
  ///   to simple optional string columns. Columns specified here but not
  ///   present in the file are ignored.
  /// </param>
  /// <param name="deserializers">
  ///   Deserialization functions to use only for this method call.
  /// </param>
  /// <param name="primaryKey">Column(s) which form the primary key.</param>
  /// <returns>The parsed DataTable.</returns>
  public static DataTable FileToDataTable(string path, IEnumerable<DataColumn> columnDefs = null,
    IDictionary<Type, Func<string, object>> deserializers = null, IEnumerable<string> primaryKey = null)
    => EnumerableToDataTable(FileUtils.FileCharEnumerator(path), columnDefs, deserializers, primaryKey);

  /// <summary>
  ///   Parses a <see cref="DataTable"/> from a <see cref="Stream"/>.
  /// </summary>
  /// <param name="reader">
  ///   A StreamReader over the Stream in question.
  /// </param>
  /// <param name="columnDefs">
  ///   Definitions of columns in the stream. Columns not specified
  ///   default to simple optional string columns. Columns specified here
  ///   but not present in the file are ignored.
  /// </param>
  /// <param name="deserializers">
  ///   Deserialization functions to use only for this method call.
  /// </param>
  /// <param name="primaryKey">Column(s) which form the primary key.</param>
  /// <returns>The parsed DataTable.</returns>
  public static DataTable StreamToDataTable(StreamReader reader, IEnumerable<DataColumn> columnDefs = null,
    IDictionary<Type, Func<string, object>> deserializers = null, IEnumerable<string> primaryKey = null)
    => EnumerableToDataTable(FileUtils.StreamCharEnumerator(reader), columnDefs, deserializers, primaryKey);

  /// <summary>
  ///   Parses a <see cref="DataTable"/> from a <see cref="char"/>
  ///   enumerable.
  /// </summary>
  /// <param name="input">The input in question.</param>
  /// <param name="columnDefs">
  ///   Definitions of columns in the input. Columns not specified default
  ///   to simple optional string columns.
  /// </param>
  /// <param name="deserializers">
  ///   Deserialization functions to use only for this method call.
  /// </param>
  /// <param name="primaryKey">Column(s) which form the primary key.</param>
  /// <returns>The parsed DataTable.</returns>
  public static DataTable EnumerableToDataTable(IEnumerable<char> input, IEnumerable<DataColumn> columnDefs = null,
    IDictionary<Type, Func<string, object>> deserializers = null, IEnumerable<string> primaryKey = null)
  {
    DataTable table = new();

    var colDefsDict = (columnDefs ?? []).Select(c => (c.ColumnName, c)).ToDictionary();
    deserializers ??= new Dictionary<Type, Func<string, object>>();

    var types = new List<Type>();

    IEnumerable<IList<string>> rows = CSVParser.EnumerableToRows(input);
    bool isHeaderRow = true;

    foreach (var row in rows)
    {
      if (isHeaderRow)
      {
        foreach (string item in row)
        {
          if (colDefsDict.TryGetValue(item, out DataColumn col))
          {
            table.Columns.Add(col);
            types.Add(col.DataType);
          }
          else
          {
            table.Columns.Add(new DataColumn
            {
              ColumnName = item
            });
            types.Add(typeof(string));
          }
        }

        if (primaryKey != null)
          table.PrimaryKey = primaryKey.Select(x => table.Columns[x]).ToArray();

        isHeaderRow = false;
      }
      else
      {
        DataRow dataRow = table.NewRow();

        // Ignore both extra columns and unspecified columns
        foreach (((string value, Type type), int index) in row.Zip(types).WithIndex())
        {
          if (value != "" && value != null)
          {
            object obj = Deserialize(type, value, deserializers);
            dataRow[index] = obj;
          }
        }

        table.Rows.Add(dataRow);
      }
    }

    return table;
  }

  /// <summary>
  ///   Converts a <see cref="DataTable"> into an enumerable over the rows
  ///   of a CSV output.
  /// </summary>
  /// <param name="table">The table to serialize.</param>
  /// <param name="serializers">
  ///   Optional serialization functions to use for this call only.
  /// </param>
  /// <returns>
  ///   An enumerable over the rows of a CSV output (usually single lines,
  ///   but will contain line breaks if the serialized values in the row do).
  /// </returns>
  public static IEnumerable<string> DataTableToStringEnumerable(DataTable table,
    IDictionary<Type, Func<object, string>> serializers = null)
  {
    yield return GetColumns(table)
      .Select(c => CSVParser.CSVEscape(c.ColumnName))
      .SJoin(",");

    foreach (DataRow row in table.AsEnumerable())
    {
      List<string> values = [];

      for (int i = 0; i < table.Columns.Count; i++)
      {
        DataColumn col = table.Columns[i];
        object val = row[i];
        if (col.DefaultValue != null && val.Equals(col.DefaultValue)) values.Add("");
        else values.Add(Serialize(val, serializers));
      }

      yield return values
        .Select(v => CSVParser.CSVEscape(Serialize(v, serializers)))
        .SJoin(",");
    }
  }

  /// <summary>
  ///   Converts a <see cref="DataTable"> into a single CSV string.
  /// </summary>
  /// <param name="table">The table to serialize.</param>
  /// <param name="serializers">
  ///   Optional serialization functions to use for this call only.
  /// </param>
  /// <returns>The serialized string.</returns>
  public static string DataTableToString(DataTable table,
    IDictionary<Type, Func<object, string>> serializers = null)
  => DataTableToStringEnumerable(table, serializers).SJoin("\n");

  /// <summary>
  ///   Writes a <see cref="DataTable"> to a CSV file.
  /// </summary>
  /// <param name="table">The table to serialize.</param>
  /// <param name="file">The file to write to.</param>
  /// <param name="serializers">
  ///   Optional serialization functions to use for this call only.
  /// </param>
  public static void DataTableToFile(DataTable table, string file,
    IDictionary<Type, Func<object, string>> serializers = null)
    => File.WriteAllText(file, DataTableToString(table, serializers));

  public static void AddDeserializers(Assembly asm)
    => AddDeserializers(asm.GetTypes());

  public static void AddDeserializers(IEnumerable<Type> types)
  {
    var methods = types.SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes<DeserializerAttribute>().Any()));

    foreach (MethodInfo method in methods)
    {
      if (!method.IsStatic) continue;
      if (!method.GetParameters().Select(t => t.ParameterType).SequenceEqual([typeof(string)])) continue;
      Type returnType = method.ReturnType;
      Deserializers[returnType] = (str) => method.Invoke(null, [str]);
    }
  }

  public static void AddSerializers(Assembly asm)
    => AddSerializers(asm.GetTypes());

  public static void AddSerializers(IEnumerable<Type> types)
  {
    var methods = types.SelectMany(t => t.GetMethods().Where(m => m.GetCustomAttributes<DeserializerAttribute>().Any()));

    foreach (MethodInfo method in methods)
    {
      if (!method.IsStatic) continue;
      if (method.ReturnType != typeof(string)) continue;

      var parameterTypes = method.GetParameters().Select(t => t.ParameterType).ToArray();
      if (parameterTypes.Length != 1) continue;
      Type returnType = parameterTypes[0];
      Serializers[returnType] = (obj) => (string)method.Invoke(null, [obj]);
    }
  }

  static IEnumerable<DataColumn> GetColumns(DataTable table)
  {
    for (int i = 0; i < table.Columns.Count; i++) yield return table.Columns[i];
  }

  static object Deserialize(Type type, string input, IDictionary<Type, Func<string, object>> localDeserializers)
  {
    if (type == typeof(string)) return input;

    if (localDeserializers != null && localDeserializers.TryGetValue(type, out Func<string, object> value))
      return value(input);

    if (Deserializers.TryGetValue(type, out value)) return value(input);

    Type nullType = Nullable.GetUnderlyingType(type);
    if (nullType != null)
    {
      try
      {
        return Deserialize(nullType, input, localDeserializers);
      }
      catch (Exception)
      {
        return null;
      }
    }

    MethodInfo method = type.GetMethod("Parse", [typeof(string)]);
    if (method != null && method.IsStatic) return method.Invoke(null, [input]);

    ConstructorInfo constructor = type.GetConstructor([typeof(string)]);
    if (constructor != null) return constructor.Invoke([input]);

    throw new NoDeserializerException(type);
  }

  static string Serialize(object input, IDictionary<Type, Func<object, string>> localSerializers)
  {
    if (input is string strInput) return strInput;

    Type type = input.GetType();

    if (localSerializers != null && localSerializers.TryGetValue(type, out var value))
      return value(input);

    if (Serializers.TryGetValue(type, out value)) return value(input);

    return input?.ToString() ?? "";
  }
}

public class NoDeserializerException(Type type) : Exception
{
  public readonly Type MissingType = type;
}

[AttributeUsage(AttributeTargets.Method)]
public class DeserializerAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public class SerializerAttribute : Attribute { }