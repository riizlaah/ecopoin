using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace EcoPoinDesktop
{
    public class Helper
    {
        private const string addr = "http://localhost:5000/";
        public static readonly HttpClient _httpClient = new HttpClient();
        public static ProfileRes? session { get; set; }
        public static string loginToken { get; set; } = "";


        async public static Task<(bool isSuccess, string message, TRes? res)> JsonReq<TRes, TReq>(string route, string method = "get", TReq? req = default) where TRes : class where TReq : class
        {
            HttpResponseMessage? res = null;
            var url = $"{addr}ecopoin-api-v1/{route}";
            method = method.ToLower();
            if (session != null) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);
            try
            {
                switch (method)
                {
                    case "post":
                        res = await _httpClient.PostAsJsonAsync(url, req);
                        break;
                    case "put":
                        res = await _httpClient.PutAsJsonAsync(url, req);
                        break;
                    case "patch":
                        res = await _httpClient.PatchAsJsonAsync(url, req);
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
                Debug.WriteLine($"Code : {res?.StatusCode}");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return (false, ex.Message, null);
            }
        }

        async public static Task<(bool isSuccess, string message, TRes? res)> JsonReq<TRes>(string route, string method = "get") where TRes : class
        {
            HttpResponseMessage? res = null;
            var url = $"{addr}ecopoin-api-v1/{route}";
            method = method.ToLower();
            if (loginToken != "") _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);
            try
            {
                switch (method)
                {
                    case "get":
                        res = await _httpClient.GetAsync(url);
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
                Debug.WriteLine($"Code : {res?.StatusCode}");
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
            if (session != null) _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);
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

        async public static Task<bool> Profile()
        {
            var (isSuccess, msg, res) = await JsonReq<ProfileRes>("users/me");
            if (isSuccess && res != null)
            {
                session = res;
            } else
            {
                Debug.WriteLine(msg);
            }
            return isSuccess && res != null;
        }

        public static void LockWindow(Form window)
        {
            window.MaximumSize = window.Size;
            window.MinimumSize = window.Size;
            window.MaximizeBox = false;
            window.MinimizeBox = false;
            window.StartPosition = FormStartPosition.CenterScreen;
        }

        public static void GenerateColumns(DataGridView table, string[] headers, string[] bindings)
        {
            table.RowHeadersVisible = false;
            table.AutoGenerateColumns = false;
            for (var i = 0; i < headers.Length; i++)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    HeaderText = headers[i],
                    Name = headers[i],
                    ReadOnly = true,
                    DataPropertyName = bindings[i]
                };
                table.Columns.Add(col);
            }
        }

        async public static Task<Bitmap?> FetchImg(string url)
        {
            try
            {
                var actualUrl = $"{addr}uploads/{url}";
                var result = await _httpClient.GetByteArrayAsync(actualUrl);
                using (var ms = new MemoryStream(result))
                {
                    var img = Image.FromStream(ms);
                    return new Bitmap(img);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return null;
            }
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



    


}
