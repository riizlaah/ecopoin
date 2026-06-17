using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography;
using System.Text;

namespace EcopoinAPI
{
    public class Helper
    {
        public static ObjectResult json(object? data, string message = "Success", int code = 200)
        {
            return new ObjectResult(new { message, data })
            {
                StatusCode = code
            };
        }

        public static ObjectResult msg(string message = "Success", int code = 200, object? data = null)
        {
            return new ObjectResult(new { message, data })
            {
                StatusCode = code
            };
        }

        public static ObjectResult err(string message, int code = 422)
        {
            return json(null, message, code);
        }

        public static (string err, int totalPage, int items, List<TRes> data) PaginateData<TModel, TRes>(IQueryable<TModel> query, int page, int size, Func<TModel, TRes> selector)
        {
            if (page < 1) return ("Page not valid", 0, 0, new List<TRes>());
            if (size < 1) return ("Size not valid", 0, 0, new List<TRes>());
            var count = query.Count();
            var totalPage = (int)Math.Ceiling((decimal)count / size);
            var items = query.Select(selector).AsQueryable().Skip((page - 1) * size).Take(size).ToList();
            return ("", totalPage, count, items);
        }

        public static (string err, int totalPage, int items, List<TRes> data) PaginateData<TModel, TRes>(IQueryable<TModel> query, int page, int size, Func<TModel, int, TRes> selector)
        {
            if (page < 1) return ("Page not valid", 0, 0, new List<TRes>());
            if (size < 1) return ("Size not valid", 0, 0, new List<TRes>());
            var count = query.Count();
            var totalPage = (int)Math.Ceiling((decimal)count / size);
            var items = query.Select(selector).AsQueryable().Skip((page - 1) * size).Take(size).ToList();
            return ("", totalPage, count, items);
        }

        public static ObjectResult PaginateRes(object data, int page, int totalPage, int items, string message = "Success", int code = 200)
        {
            return new ObjectResult(new { message, data, pagination = new { page, totalPage, items } })
            {
                StatusCode = code
            };
        }

        public static string hash(string str)
        {
            using(var alg = SHA256.Create())
            {
                var bytes = alg.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach(var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static bool isHashValid(string str, string hashedStr)
        {
            var str2 = hash(str);
            return StringComparer.OrdinalIgnoreCase.Compare(str2, hashedStr) == 0;
        }

        async public static Task<string> UploadFile(string path, IFormFile file, string? target = null)
        {
            var ext = Path.GetExtension(path);
            var uniqName = target ?? $"{DateTime.Now:ddMMMMyyyyHHmmss}_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(path, uniqName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return uniqName;
        }

        public static string RandStr(int len)
        {
            var rand = new Random();
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var str = "";
            for (var i = 0; i < len; i++) str += chars[rand.Next(chars.Length - 1)];
            return str;
        }
    }
}
