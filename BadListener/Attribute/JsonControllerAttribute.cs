using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BadListener.Attribute
{
	public class JsonControllerAttribute : BaseControllerAttribute
	{
		public override void Render(object model, HttpListenerResponse response, HttpServer server)
		{
			var settings = new JsonSerializerSettings 
			{ 
				ContractResolver = new CamelCasePropertyNamesContractResolver() 
			};
            string json = JsonConvert.SerializeObject(model, settings);
            response.SetStringResponse(json, "application/json");
		}
	}
}
