using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_202.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        [HttpGet]
        public object Get()
        {
            return new { result = "Надійшов запит методом GET" };
        }

        [HttpPost]
        public object Post([FromBody] RequestData data)
        {
            return new { result = $"Надійшов запит методом POST з value={data.Value}" };
        }

        [HttpPut]
        public object Put([FromBody] RequestData data)
        {
            return new { result = $"Надійшов запит методом PUT з value={data.Value}" };
        }
        
        public object Default()
        {
            String str;
            using (var stream = new StreamReader(HttpContext.Request.Body))
            {
                str = stream.ReadToEndAsync().Result;
            }
            
            return new
            {
                result = $"Надійшов запит методом {HttpContext.Request.Method} {str}" 
            };
        }
    }
    public class RequestData
    {
        public string Value { get; set; } = null!;
    }
    /* Д.З. Реалізувати обмін данними з API методами PATCH та UNLINK */
}
