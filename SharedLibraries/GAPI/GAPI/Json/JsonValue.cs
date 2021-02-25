using System;
using System.Text;

namespace Sobees.Library.BGoogleLib.Json
{
  public class JsonValue
  {
    protected static void EatSpaces(string str, ref int position)
    {
      while ((position < str.Length) &&
             ((str[position] == ' ') || (str[position] == '\n'))
        )
        position++;
    }

    protected static JsonValue ParseValue(string str, ref int position)
    {
      EatSpaces(str, ref position);

      // JsonString
      if (str[position] == '\"')
        return JsonString.Parse(str, ref position);
        // JsonObject
      else if (str[position] == '{')
        return JsonObject.Parse(str, ref position);
        // JsonNumber
      else if (JsonNumber.IsNumberPart(str[position]))
        return JsonNumber.Parse(str, ref position);
        // 'null'
      else if ((str.Length > position + 4) &&
               (str.Substring(position, 4).Equals("null", StringComparison.InvariantCultureIgnoreCase)))
      {
        position += 4;
        return null;
      }
        // 'true'
      else if ((str.Length > position + 4) &&
               (str.Substring(position, 4).Equals("true", StringComparison.InvariantCultureIgnoreCase)))
      {
        position += 4;
        return new JsonBoolean(true);
      }
        // 'false'
      else if ((str.Length > position + 5) &&
               (str.Substring(position, 5).Equals("false", StringComparison.InvariantCultureIgnoreCase)))
      {
        position += 5;
        return new JsonBoolean(false);
      }
        // JsonArray
      else if (str[position] == '[')
        return JsonArray.Parse(str, ref position);
      else
        throw new JsonParseException(str, position);
    }
  }
}