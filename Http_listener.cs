using System.Net;
using System.Text;
using System.IO;


namespace SimpleWebServer {
    public static class Program {
        public static async Task Main(){

            //make an http server program at http://localhost:5000/
            var server = new HttpListener();
            server.Prefixes.Add("http://localhost:5000/");

            // Start up the server
            server.Start();
            Console.WriteLine("Server is running... press CTRL+C to stop");

            while (server.IsListening){
                Console.WriteLine("Awaiting requests. . .");

                // getting response
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                // HTTP Type
                Console.WriteLine("Received a " + request.HttpMethod + " request");
                // var buffer = Encoding.UTF8.GetBytes(responseBody);
                
                ///////////////////////////////////////////////////////////////////////////////////
                // get request path
                if(request.HttpMethod == "GET"){
                    string baseDirectory = @"C:\Users\chuck\OneDrive\Documents\projects\c#\Web_server";
                    string requestedPath = request.Url.AbsolutePath;
                    string filePath = Path.Combine(baseDirectory, requestedPath.TrimStart('/'));
                    string fullPath = Path.GetFullPath(filePath);   //to directory test

                    //getting the content type requested
                    string extension = Path.GetExtension(filePath).ToLower();
                    string contentType;

                    //used to determine the type of file being retrieved. 
                    switch(extension){
                        case ".jpg":
                        case ".jpeg":
                            contentType = "image/jpeg";
                            break;

                        case ".html":
                            contentType = "text/html";
                            break;

                        case ".css":
                            contentType = "text/css";
                            break;

                        case ".js":
                            contentType = "application/javascript";
                            break;

                        default: 
                            contentType = "text/plain";
                            break;
                    }
                    response.ContentType = contentType;

                    // validate directory
                    if(!fullPath.StartsWith(baseDirectory)){
                        response.StatusCode = 400;
                        return;
                    }

                    // does file requested exist?
                    if(File.Exists(filePath)){
                        Console.WriteLine(filePath + "\n");
                        
                        //safely reading all bytes or throw server error
                        byte[] buffer;
                        try{ buffer = File.ReadAllBytes(filePath);
                        } catch (Exception x) {
                            response.StatusCode = 500;
                            Log("Error at byte[] arr: " + x.Message);
                            return;
                        }

                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

                        Console.WriteLine("Returned " + filePath);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        Console.WriteLine("File Not Found");
                        string responseBody = "File Not Found";
                        byte[] buffer = Encoding.UTF8.GetBytes(responseBody);
                        response.ContentLength64 = buffer.Length;
                        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }

                    LogResponse(response, filePath);
                }

                //not used yet, as I have to flesh out the site before I can use these. 

                else if(request.HttpMethod == "POST"){
                    using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                    var requestBody = await reader.ReadToEndAsync();

                    // TODO: 
                    string responseBody = "Posting...\n";
                    var buffer = Encoding.UTF8.GetBytes(responseBody);
                    response.ContentLength64 = buffer.Length;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }

                else if(request.HttpMethod == "HEAD"){}

                else if(request.HttpMethod == "PUT"){}

                else if(request.HttpMethod == "CONNECT"){}

                else if(request.HttpMethod == "OPTIONS"){}

                else if(request.HttpMethod == "TRACE"){}

                else if(request.HttpMethod == "PATCH"){}

                


                context.Response.Close();
            }
            server.Stop();
        }

        private static void Log(string message){
            string logFilePath = @"C:\Users\chuck\OneDrive\Documents\projects\c#\Web_server\ErrorLog.txt";
            string logMessage = DateTime.Now + ": " + message;
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }

        private static void LogResponse(HttpListenerResponse response, string filePath){
            Console.WriteLine(" Response Details: ");
            Console.WriteLine(" Status Code: " + response.StatusCode);
            Console.WriteLine(" Content Type: " + response.ContentType);
            Console.WriteLine(" Content Length: " + response.ContentLength64);
            Console.WriteLine(" File Path: " + filePath);
        }
    }
}