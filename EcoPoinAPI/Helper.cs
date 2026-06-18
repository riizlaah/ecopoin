using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EcoPoinAPI
{
    public class ExtBaseController : ControllerBase
    {
        protected int getUserId()
        {
            return Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        protected static ObjectResult json(object? data, string message, int code = 200)
        {
            return new ObjectResult(new { message, data })
            {
                StatusCode = code
            };
        }

        protected static ObjectResult err(string message, int code = 422)
        {
            return json(null, message, code);
        }

        protected static ObjectResult msg(string message, int code = 200)
        {
            return json(null, message, code);
        }

        protected static string hash(string str)
        {
            using(var alg = SHA256.Create())
            {
                var bytes = alg.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        protected static bool isHashValid(string str, string hashedStr)
        {
            var str2 = hash(str);
            return StringComparer.OrdinalIgnoreCase.Compare(str2, hashedStr) == 0;
        }

        protected static ObjectResult PaginateQuery<TRes, TModel>(IQueryable<TModel> query, int page, int size, Func<TModel, TRes> selector, string message)
        {
            if (page < 1) return err("Page not valid");
            if (size < 1) return err("Size not valid");
            var query2 = query.Select(selector);
            var items = query2.Count();
            var data = query2.Skip((page - 1) * size).Take(size);
            var totalPage = (int)Math.Ceiling((decimal)items / size);
            return json(new
            {
                data,
                message,
                pagination = new
                {
                    page,
                    totalPage,
                    items
                }
            }, message);
        }

        protected static ObjectResult PaginateQuery<TRes, TModel>(IQueryable<TModel> query, int page, int size, Func<TModel, int, TRes> selector, string message)
        {
            if (page < 1) return err("Page not valid");
            if (size < 1) return err("Size not valid");
            var query2 = query.Select(selector);
            var items = query2.Count();
            var data = query2.Skip((page - 1) * size).Take(size);
            var totalPage = (int)Math.Ceiling((decimal)items / size);
            return json(new
            {
                data,
                message,
                pagination = new
                {
                    page,
                    totalPage,
                    items
                }
            }, message);
        }

        protected string RandStr(int len)
        {
            var rand = new Random();
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var str = "";
            for(var i = 0; i < len; i++) str += chars[rand.Next(chars.Length - 1)];
            return str;
        }

        async protected Task<string> Upload(string dir, IFormFile file, string? target = null)
        {
            var ext = Path.GetExtension(file.FileName);
            var uniqName = target ?? $"{DateTime.Now:yyyyMMdd-HHmmss}_{Guid.NewGuid()}{ext}";
            var path = Path.Combine(dir, uniqName);
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return uniqName;
        }
    }
}
