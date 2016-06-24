using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;


namespace openCaseApiTest
{
    [TestClass]
    public class topicTest
    {
        private HttpClient _httpClient;

   
      
        public topicTest()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:61693/");
        }

        
        [TestMethod]
        public void TestMethod1()
        {
            var token = getPasswordToken();
            addTopic(token);
    
        }

        [TestMethod]
        public void TestMethod2()
        {
            var token = getPasswordToken();
            int id = Convert.ToInt32( addTopic(token));
            addReply(token,id);
        }


        private string addTopic(TokenModel tm)
        {

            var parameters = new Dictionary<string, string>();
            parameters.Add("title", "tester");
            parameters.Add("nodeID", "201");
            parameters.Add("body", "测试一下");

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tm.access_token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tm.access_token);

            var response =  _httpClient.PostAsync("/topic/Add", new FormUrlEncodedContent(parameters)).Result;
            var responseValue = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(responseValue);

            if (response.StatusCode == (HttpStatusCode)422)
            {

                throw new Exception("model验证失败");
            } else if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("addTopic:" + response.StatusCode);
            }

            return responseValue;

            
        }

        private void addReply(TokenModel tm,int topicID)
        {

            var parameters = new Dictionary<string, string>();
            parameters.Add("body", "回复咯");

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tm.access_token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tm.access_token);

            var response = _httpClient.PostAsync("/topic/" + topicID + "/Reply", new FormUrlEncodedContent(parameters)).Result;
            var responseValue = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine(responseValue);

            if (response.StatusCode == (HttpStatusCode)422)
            {

                throw new Exception("model验证失败");
            }
            else if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("addTopic:" + response.StatusCode);
            }


        }



        private TokenModel getPasswordToken()
        {

            string clientId = "app";
            string clientSecret = "test";

            var parameters = new Dictionary<string, string>();
            parameters.Add("grant_type", "password");
            parameters.Add("username", "c_zhubo");
            parameters.Add("password", "Cpic1234");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(clientId + ":" + clientSecret)));


            var response = _httpClient.PostAsync("/token", new FormUrlEncodedContent(parameters)).Result;
            var responseValue = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<TokenModel>(responseValue);
            else
            {
                System.Console.WriteLine(responseValue);
                throw new Exception("获取token失败");
            }
        }
    }
}
