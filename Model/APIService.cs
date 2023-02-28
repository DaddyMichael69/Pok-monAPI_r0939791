using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;

namespace Pokémon.Model
{
    public class APIService
    {
        /*FIELD*/
        private HttpClient _httpClient;
        private Uri _baseUri;
        private string _jsonFilePath;
        private bool _isValidatedAPICall;



        /*CONSTRUCTOR*/
        public APIService(Uri baseUri)
        {
            _baseUri = baseUri;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUri;             
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            _jsonFilePath = AppDomain.CurrentDomain.BaseDirectory + @"file.json";
            ValidateJson(_jsonFilePath);
            
            _isValidatedAPICall = false;
        }




        //  VALIDATE JSON FILE
        private void ValidateJson(string strPath)
        {
            // Create a new JArray object
            JArray jsonArray = new JArray();

            // check if file exists
            if (File.Exists(strPath))
            {
                // check if file = empty
                if (new FileInfo(strPath).Length == 0)
                {
                    // Serialize the JArray to a JSON string
                    string json = jsonArray.ToString();
                    File.WriteAllText(strPath, json);
                }
            }
            else
            {
                //create empty file
                string json = jsonArray.ToString();
                File.WriteAllText(strPath, json);
            }
        }



        //  API CALL MAKEN
        public HttpResponseMessage APICall(string strEndpoint, string strInputSearchbar)
        {

            // Async (wait for server response)
            HttpResponseMessage response = _httpClient.GetAsync(strEndpoint + "/" + strInputSearchbar).Result;

            // ValideAPIRespons
            _isValidatedAPICall = ValidateAPIResponse(response);

            //  validated response return
            if (_isValidatedAPICall)
            {
                return response;

            }
            else
            {
                return null;        // eventuele foutmelding meegeven
            }


        }
                       


        //  API RESPONSE VALIDATION (+ status error msg)
        private bool ValidateAPIResponse(HttpResponseMessage response) 
        {
            // check server status
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            else
            {
                MessageBox.Show("ERROR..." + Environment.NewLine +                      // dit moet probably naar de front-end
                                response.StatusCode + Environment.NewLine +
                                response.Headers.Date);
                return false;
            }
        }



        //  convert naar pokemon object
        public T ConvertAPIResponse<T>(HttpResponseMessage response) where T : class
        {
            // respons uit Async als string omzetten
            var jsonString = response.Content.ReadAsStringAsync().Result;

            // deserialized T (omzetten naar .net object/type)
            var Object = JsonConvert.DeserializeObject<T>(jsonString);

            // write to console
            Debug.WriteLine(Object);

            return Object;
        }



    }
}
