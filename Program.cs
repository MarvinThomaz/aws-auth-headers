using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace set_aws_headers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Digite o caminho do arquivo do swagger: ");

            var file = Console.ReadLine();

            if (File.Exists(file))
            {
                const string defaultHeadersPath = "default_headers.json";

                var document = File.ReadAllText(file);
                var jsonSwagger = JsonConvert.DeserializeObject<dynamic>(document);
                var defaultHeadersDoc = File.ReadAllText(defaultHeadersPath);
                var jsonDefaultHeaders = JsonConvert.DeserializeObject<dynamic>(defaultHeadersDoc);
                var paths = jsonSwagger.paths;

                foreach (var path in paths)
                {
                    var endpoints = path.Value;

                    foreach (var endpoint in endpoints)
                    {
                        var responses = endpoint.Value.responses;

                        foreach (var response in responses)
                        {
                            if (response.Value.headers == null)
                            {
                                response.Value.headers = new JObject();
                            }

                            var headers = response.Value.headers;

                            foreach (var header in jsonDefaultHeaders)
                            {
                                if (!Contains(header, headers))
                                {
                                    headers.Add(header);
                                }
                            }
                        }
                    }
                }

                var stringResult = JsonConvert.SerializeObject(jsonSwagger);

                File.WriteAllText(file, stringResult);
            }
        }

        static bool Contains(dynamic source, dynamic headers)
        {
            if (source == null)
            {
                return false;
            }

            foreach (var header in headers)
            {
                if (source.Name == header.Name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
