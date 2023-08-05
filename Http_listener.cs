using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebServer{
    public static class Program{
        public static async Task Main(string[] args){
            var server = new HttpListener();
            server.Prefixes.Add("http://localhost:5000/");

            server.Start();
            Console.WriteLine("Server is running... press CTRL+C to stop");

            while (server.IsListening){

                // getting response
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                // HTTP Type
                Console.WriteLine("Received a " + request.HttpMethod + " request");
                
                // get request path
                if(request.HttpMethod == "POST"){
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    var requestBody = await reader.ReadToEndAsync();

                    // TODO: 

                    string responseBody = "Received POST request";
                    var buffer = Encoding.UTF8.GetBytes(responseBody);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }

                else if(request.HttpMethod == "GET"){
                    string responseBody = "Hello, World!";
                    var buffer = Encoding.UTF8.GetBytes(responseBody);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }

                context.Response.Close();
            }
            server.Stop();
        }
    }
}