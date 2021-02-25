using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sobees.Library.BGoogleLib.Json
{
  public class JsonArray : JsonValue
  {
    List<JsonValue> _items;

    public JsonValue[] Items => (JsonValue[])_items.ToArray();

    public int Length => _items.Count;

    private JsonArray()
    {
      _items = new List<JsonValue>();
    }

    private void Add(JsonValue jsonValue)
    {
      _items.Add(jsonValue);
    }

    internal static JsonArray Parse(string str, ref int position)
    {
      JsonArray jsonArray = new JsonArray();

      EatSpaces(str, ref position);

      if (str[position] != '[')
        throw new JsonParseException(str, position);

      bool firstItem = true;

      while (position < str.Length)
      {
        position++;

        // Check for empty array
        if (firstItem == true)
        {
          firstItem = false;
          EatSpaces(str, ref position);

          if (str[position] == ']')
          {
            position++;
            return jsonArray;
          }
        }

        JsonValue jsonValue = ParseValue(str, ref position);
        jsonArray.Add(jsonValue);

        EatSpaces(str, ref position);

        if (str[position] == ']')
        {
          position++;
          return jsonArray;
        }

        if (str[position] != ',')
          throw new JsonParseException(str, position);
      }

      throw new JsonParseException(string.Format("Unexpected end of string during reading JSON array: '{0}'", str));
    }
  }
}