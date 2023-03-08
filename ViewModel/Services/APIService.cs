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
using System.Net;
using System.Windows.Markup;

namespace Pokémon.Model
{
    public class APIService
    {
        /*FIELD*/
        private HttpClient _httpClient;
        private Uri _baseUri;
        private string _jsonPlayerDataFilePath;
        private string _jsonPokeDataFilePath;
        private string _jsonErrorLogFilePath;
        private bool _isValidatedAPICall;



        /*CONSTRUCTOR*/
        public APIService(Uri baseUri)
        {
            //set up base adress
            _baseUri = baseUri;                                                     
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUri;             
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            //create json + set filepath
            _jsonPlayerDataFilePath = AppDomain.CurrentDomain.BaseDirectory + @"PlayerData.json";
            ValidateJson(_jsonPlayerDataFilePath);
            _jsonPokeDataFilePath = AppDomain.CurrentDomain.BaseDirectory + @"PokeData.json";
            ValidateJson(_jsonPokeDataFilePath);
            _jsonErrorLogFilePath = AppDomain.CurrentDomain.BaseDirectory + @"ErrorLog.json";
            ValidateJson(_jsonErrorLogFilePath);

            _isValidatedAPICall = false;
        }


        /*PROPERTIES*/
        public string JsonPlayerDataFilePath { get => _jsonPlayerDataFilePath; set => _jsonPlayerDataFilePath = value; }
        public string JsonPokeDataFilePath { get => _jsonPokeDataFilePath; set => _jsonPokeDataFilePath = value; }
        public string JsonErrorLog { get => _jsonErrorLogFilePath; set => _jsonErrorLogFilePath = value; }



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

        public string ReadFromJson(string jsonPath) 
        {
            // Read the JSON file
            return File.ReadAllText(_jsonPlayerDataFilePath);
        }

        // Write to jsonfile
        public void WriteToJson(object obj, string jsonPath) 
        {
            // Serialize to a JSON string
            string jsonSerialized = JsonConvert.SerializeObject(obj, Formatting.Indented);

            // Write the JSON string to the file
            File.WriteAllText(jsonPath, jsonSerialized);
        }

        public void AddToJson(object obj, string jsonPath) 
        {
            // Serialize to a JSON string
            string jsonSerialized = JsonConvert.SerializeObject(obj, Formatting.Indented);

            // Write the JSON string to the file
            File.AppendAllText(jsonPath, jsonSerialized + Environment.NewLine);
        }

        public void ResetJson() 
        {
            // Write the JSON string to the file
            File.WriteAllText(_jsonPlayerDataFilePath, string.Empty);
        }

        //  API CALL
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

        //  API CONVERTER
        public T ConvertAPIResponse<T>(HttpResponseMessage response) where T : class
        {
            if (response.IsSuccessStatusCode && response != null)
            {
                // respons uit Async als string omzetten
                var jsonString = response.Content.ReadAsStringAsync().Result;

                // deserialized T (omzetten naar .net object/type)
                var Object = JsonConvert.DeserializeObject<T>(jsonString);

                // write to console
                Debug.WriteLine(Object);

                return Object;
            }

            else 
            { 
                return null; 
            }
        }
    }
}
