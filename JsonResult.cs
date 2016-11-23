using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace TingVoa.WebView
{
    public class JsonResult
    {
        public JsonResult()
        {
            
        }

        public static JsonResult Fail(string message)
        {
            return new JsonResult() {Success = false,Message = message};
        }

        public static JsonResult Done(object data)
        {
            return new JsonResult() {Success = true, Data = data};
        }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static implicit operator string(JsonResult result)
        {
            return result.ToString();
        }
    }
}