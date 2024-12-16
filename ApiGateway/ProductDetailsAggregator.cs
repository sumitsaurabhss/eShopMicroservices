using Newtonsoft.Json;
using Ocelot.Configuration;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ApiGateway
{
    public class ProductDetailsAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var one = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var two = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

            var contentBuilder = new StringBuilder();
            contentBuilder.Append(one);
            contentBuilder.Append(two);

            var stringContent = new StringContent(contentBuilder.ToString())
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
        }
    }
}