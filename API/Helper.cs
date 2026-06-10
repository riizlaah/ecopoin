using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Cryptography;
using System.Text;

namespace EcoPoinAPI
{
    public class Helper
    {
        public static ObjectResult json(object? data, string message = "Success", int code = 200)
        {
            return new ObjectResult(new
            {
                data,
                message
            })
            {
                StatusCode = code
            };
        }

        public static ObjectResult msg(string message = "Success", int code = 200, object? data = null)
        {
            return new ObjectResult(new
            {
                data,
                message
            })
            {
                StatusCode = code
            };
        }

        public static ObjectResult paginate(object? data, int page, int size, int totalPage, string message = "Success", int code = 200)
        {
            return new ObjectResult(new
            {
                data,
                message,
                pagination = new
                {
                    page,
                    size,
                    totalPage
                }
            })
            {
                StatusCode = code
            };
        }

        public static ObjectResult err(string message, int code = 429)
        {
            return json(null, message, code);
        }

        public static string Sha256(string str)
        {
            using (var alg = SHA256.Create())
            {
                var bytes = alg.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool VerifySha256(string str, string hashedStr)
        {
            var str2 = Sha256(str);
            return StringComparer.OrdinalIgnoreCase.Compare(str2, hashedStr) == 0;
        }

        public static (bool isSuccess, List<TRes>? results, (int current, int total)? paging, string error) Paginate<TRes, TModel>(IQueryable<TModel> query, Func<TModel, TRes> selector, int page, int size)
        {
            if (size < 1) return (false, null, null, "Size not valid");
            if (page < 1) return (false, null, null, "Size not valid");
            var totalPage = (int)Math.Ceiling((decimal)query.Count() / size);
            query = query.Skip((page - 1) * size).Take(size);
            var datas = query.Select(selector).ToList();
            return (true, datas, (page, totalPage), "");
        }
    }
}
