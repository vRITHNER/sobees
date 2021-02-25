using System;
using System.Text;

namespace Sobees.Library.BGoogleLib.Json
{
  public class JsonString : JsonValue
  {
    string _value;

    public string Value => _value;

    public JsonString(string value)
    {
      _value = value;
    }

    static string JsonDecode(string str, ref int position)
    {
      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      EatSpaces(str, ref position);

      if (str[position] == '\"')
        position++;
      else
        throw new JsonParseException(str, position);

      while (position < str.Length)
      {
        switch (str[position])
        {
          case '\\':
            position++;
            switch (str[position])
            {
              case 'b':
                sb.Append('\b');
                break;
              case 't':
                sb.Append('\t');
                break;
              case 'n':
                sb.Append('\n');
                break;
              case 'f':
                sb.Append('\f');
                break;
              case 'r':
                sb.Append('\r');
                break;
              case 'u':
                string number = str.Substring(position + 1, 4);
                position += 4;
                int code = int.Parse(number, System.Globalization.NumberStyles.HexNumber);
                char ch = (char)code;
                sb.Append(ch);
                break;
              default:
                sb.Append(str[position]);
                break;
            }
            break;
          case '\"':
            position++;
            return sb.ToString();
          default:
            sb.Append(str[position]);
            break;
        }

        position++;
      }

      return sb.ToString();
    }

    internal static JsonString Parse(string str, ref int position)
    {
      EatSpaces(str, ref position);

      if (str[position] != '\"')
        throw new JsonParseException(str, position);

      string jsonString = JsonDecode(str, ref position);

      return new JsonString(jsonString);
    }

    public override string ToString()
    {
      return Value;
    }
  }
}