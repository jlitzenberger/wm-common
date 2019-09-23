using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace WM.Common.Serializers
{
    public enum HttpVerb
    {
        POST,
        GET
    }
    public static class JSONSerializer
    {
        public static HttpWebRequest CreateJsonHttpWebRequest(Uri uri, HttpVerb verb, string username, string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri.AbsoluteUri);
            JSONSerializer.SetBasicAuthHeader(request, username, password);
            request.Method = verb.ToString();
            request.ContentType = "application/json; charset=utf-8";

            return request;
        }
        public static T RestfulJsonPost<T>(dynamic obj, HttpWebRequest request)
        {
            //Convert to object type
            dynamic dObj = Convert.ChangeType(obj, obj.GetType());

            //write JSON payload to HTTP RequestStream
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(SerializeToJSON(dObj));
            }

            //read HTTP ResponseStream
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return DeserializeFromJSON<T>(reader.ReadToEnd());
            }
        }
        public static string SerializeToJSON<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }
        public static T DeserializeFromJSON<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);

                return obj;
            }
        }

        public static void SetBasicAuthHeader(WebRequest req, String userName, String userPassword)
        {
            string authInfo = userName + ":" + userPassword;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }
    }
}
