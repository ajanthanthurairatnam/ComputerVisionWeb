using ComputerVisionWeb.Helper;
using ComputerVisionWeb.Models;
using ComputerVisionWeb.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ComputerVisionWeb.Services
{
    public interface IImageProcessor
    {
        public Task<ImageProcessModel> ReadImageTextAsync(IFormFile File);       
    }
    public class MicrosoftVision : IImageProcessor
    {
        private readonly IConfiguration _config;

        public MicrosoftVision(IConfiguration config)
        {
            _config = config;
        }
    
        public async Task<ImageProcessModel> ReadImageTextAsync(IFormFile File)
        {
            if (File == null && File.Length <= 0)
            {
                return (new ImageProcessModel { Status = Status.Failure, Message = "please select a file" });
            }

            byte[] FileData = HelperClass.ReadFully(File);

            var endpoint = _config["Microsoft:Vision:COMPUTER_VISION_ENDPOINT"];
            var subscriptionKey = _config["Microsoft:Vision:COMPUTER_VISION_SUBSCRIPTION_KEY"];
            string uriBase = endpoint + "vision/v2.1/read/core/asyncBatchAnalyze";

            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Assemble the URI for the REST API method.
                string uri = uriBase;

                HttpResponseMessage response;

                // Two REST API methods are required to extract text.
                // One method to submit the image for processing, the other method
                // to retrieve the text found in the image.

                // operationLocation stores the URI of the second REST API method,
                // returned by the first REST API method.
                string operationLocation;

                // Reads the contents of the specified local image
                // into a byte array.
                byte[] byteData = FileData;

                // Adds the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // The first REST API method, Batch Read, starts
                    // the async process to analyze the written text in the image.
                    response = await client.PostAsync(uri, content);
                }

                // The response header for the Batch Read method contains the URI
                // of the second method, Read Operation Result, which
                // returns the results of the process in the response body.
                // The Batch Read operation does not return anything in the response body.
                if (response.IsSuccessStatusCode)
                    operationLocation =
                        response.Headers.GetValues("Operation-Location").FirstOrDefault();
                else
                {
                    // Display the JSON error data.
                    string errorString = await response.Content.ReadAsStringAsync();

                    return (new ImageProcessModel { Status = Status.Failure, Message = errorString });


                }

                // If the first REST API method completes successfully, the second 
                // REST API method retrieves the text written in the image.
                //
                // Note: The response may not be immediately available. Text
                // recognition is an asynchronous operation that can take a variable
                // amount of time depending on the length of the text.
                // You may need to wait or retry this operation.
                //
                // This example checks once per second for ten seconds.
                string contentString;
                int i = 0;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    response = await client.GetAsync(operationLocation);
                    contentString = await response.Content.ReadAsStringAsync();
                    ++i;
                }
                while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

                if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
                {
                    return (new ImageProcessModel { Status = Status.Failure, Message  = "Timeout error" });
                }

                MicrosoftVisionModel account = JsonConvert.DeserializeObject<MicrosoftVisionModel>(contentString);

                if(account.status=="Succeeded" & account.recognitionResults.Count() > 0) 
                {
                    StringBuilder stringBuilder = new StringBuilder();
                  foreach(var line in account.recognitionResults.FirstOrDefault().lines)
                    {
                        stringBuilder.Append($"{line.text} ");
                    }
                    return (new ImageProcessModel { Status = Status.Success, OutputText = stringBuilder.ToString() });

                }

                return (new ImageProcessModel { Status = Status.Failure, Message="Error Proccessing Image" });

            }
            catch (Exception e)
            {
                 return (new ImageProcessModel { Status = Status.Success, Message = e.StackTrace });
            }

        }
    }
}
