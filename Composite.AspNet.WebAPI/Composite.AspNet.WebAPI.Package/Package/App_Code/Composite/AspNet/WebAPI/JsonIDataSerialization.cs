using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using Composite.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Composite.AspNet.WebAPI
{
    internal static class JsonIDataSerialization
    {
        public static void WrapJsonContentResolver(HttpConfiguration configuration)
        {
            var settings = configuration.Formatters.JsonFormatter.SerializerSettings;
            var resolver = settings.ContractResolver;

            if (resolver.GetType() != typeof (JsonContractResolver))
            {
                return;
            }

            var fieldInfo = resolver.GetType().GetField("_formatter", BindingFlags.NonPublic | BindingFlags.Instance);

            var formatter = (MediaTypeFormatter)fieldInfo.GetValue(resolver);

            settings.ContractResolver = new CustomJsonContractResolver(formatter);
            settings.Converters.Add(new CompositeDataConverter());
        }

        private class CustomJsonContractResolver : JsonContractResolver
        {
            public CustomJsonContractResolver(MediaTypeFormatter formatter)
                : base(formatter)
            {
            }

            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var result = base.GetSerializableMembers(objectType);

                if (typeof (IData).IsAssignableFrom(objectType))
                {
                    result.RemoveAll(m => m.Name == "DataSourceId" || m.Name == "WrappedData");
                }

                return result;
            }
        }


        public class CompositeDataConverter : CustomCreationConverter<IData>
        {
            static readonly ConcurrentDictionary<Type, Type> _wrapperTypeCache = new ConcurrentDictionary<Type, Type>();

            public override bool CanConvert(Type objectType)
            {
                return objectType.IsInterface && typeof (IData).IsAssignableFrom(objectType);
            }

            public override IData Create(Type objectType)
            {
                throw new InvalidOperationException("Not supposed to be called.");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken token = JObject.Load(reader);

                var type = _wrapperTypeCache.GetOrAdd(objectType, t => DataFacade.BuildNew(t).GetType());

                return JsonConvert.DeserializeObject(token.ToString(), type);;
            }
        }
    }
}
