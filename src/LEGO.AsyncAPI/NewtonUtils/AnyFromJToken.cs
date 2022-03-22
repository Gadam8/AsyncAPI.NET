// Copyright (c) The LEGO Group. All rights reserved.

namespace LEGO.AsyncAPI.NewtonUtils
{
    using System.Linq;
    using LEGO.AsyncAPI.Models.Any;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal static class AnyFromJToken
    {
        public static IAny Map(JToken token)
        {
            return token.Type switch
            {
                // Unfortunately we cannot us the Object mapper for this. Newton will always from an exception using the value is null
                // see: ConvertUtils.ConvertResult.TryConvertInternal for details
                JTokenType.Null => default(AsyncAPINull),
                JTokenType.String => ObjectFromJToken.Map<AsyncAPIString>(token),
                JTokenType.Boolean => ObjectFromJToken.Map<AsyncAPIBoolean>(token),
                JTokenType.Integer => ObjectFromJToken.Map<AsyncAPILong>(token),
                JTokenType.Float => ObjectFromJToken.Map<AsyncAPIDouble>(token),
                JTokenType.Object => Map(token.Value<JObject>()),
                JTokenType.Array => Map(token.Value<JArray>()),
                _ => throw new JsonException("Value of type " + token.GetType() + " not handled for arrays")
            };
        }

        private static AsyncAPIObject Map(JObject value)
        {
            var output = new AsyncAPIObject();
            foreach (var jProperty in value.Properties())
            {
                output.Add(jProperty.Name, Map(jProperty.Value));
            }

            return output;
        }

        private static AsyncAPIArray Map(JArray value)
        {
            var output = new AsyncAPIArray();
            output.AddRange(value.Select(Map));
            return output;
        }
    }
}