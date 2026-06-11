using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;

namespace EcoPoinDesktop
{
    public class Helper
    {
        private const string addr = "http://localhost:5000/";
        public static readonly HttpClient _httpClient = new HttpClient();

        public static LoginRes? session { get; set; }


        async public static Task<(bool isSuccess, string message, TRes? res)> JsonReq<TRes, TReq>(string route, string method = "get", TReq? req = default) where TRes : class where TReq : class
        {
            HttpResponseMessage res;
            var url = $"{addr}ecopoin-api-v1/{route}";
            method = method.ToLower();
            if (session != null) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.token);
            try
            {
                switch (method)
                {
                    case "get":
                        res = await _httpClient.GetAsync(url);
                        break;
                    case "post":
                        res = await _httpClient.PostAsJsonAsync(url, req);
                        break;
                    case "put":
                        res = await _httpClient.PatchAsJsonAsync(url, req);
                        break;
                    case "patch":
                        res = await _httpClient.PatchAsJsonAsync(url, req);
                        break;
                    case "delete":
                        res = await _httpClient.DeleteAsync(url);
                        break;
                    default:
                        return (false, "Unsupported request method", null);
                }
                var res2 = await res.Content.ReadFromJsonAsync<ApiRes<TRes>>();
                if (res2 == null) return (false, "Network error", null);
                return (res.IsSuccessStatusCode, res2.message, res2.data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return (false, ex.Message, null);
            }
        }
        async public static Task<(bool isSuccess, string message, List<TRes> res, Pagination? paging)> PaginatedReq<TRes>(string route) where TRes : class
        {
            HttpResponseMessage res;
            var url = $"{addr}ecopoin-api-v1/{route}";
            var method = "get";
            if (session != null) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.token);
            try
            {
                res = await _httpClient.GetAsync(url);
                var res2 = await res.Content.ReadFromJsonAsync<PaginatedRes<List<TRes>>>();
                if (res2 == null) return (false, "Network error", new List<TRes>(), null);
                return (res.IsSuccessStatusCode, "", res2.data ?? new List<TRes>(), res2.pagination);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return (false, ex.Message, new List<TRes>(), null);
            }
        }



        public static void LockWindow(Form window)
        {
            window.MaximumSize = window.Size;
            window.MinimumSize = window.Size;
            window.MaximizeBox = false;
            window.MinimizeBox = false;
            window.StartPosition = FormStartPosition.CenterScreen;
        }
    }


    public class ApiRes<T> where T : class
    {
        public T? data { get; set; } = default;
        public string message { get; set; } = "";
    }

    public class PaginatedRes<T> : ApiRes<T> where T : class
    {
        
        public Pagination pagination { get; set; } = null!;
    }

    public class Pagination
    {
        public int page { get; set; }
        public int totalPage { get; set; }
        public int items { get; set; }
    }



    public class LoginRes
    {
        public int id { get; set; }
        public string fullName { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string token { get; set; }
    }


}
