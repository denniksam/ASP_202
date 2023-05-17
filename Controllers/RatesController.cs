using ASP_202.Data;
using ASP_202.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_202.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public RatesController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public object Get()
        {
            return new { result = "Надійшов запит методом GET" };
        }

        [HttpPost]
        public object Post([FromBody] RequestData data)
        {
            String result = null!;
            if (data == null
             || data.ItemId == null
             || data.Value == null
             || data.UserId == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                result = $"Недостатня кількість параметрів: value={data?.Value} user={data?.UserId} item={data?.ItemId}";
            }
            else
            {
                Guid itemId, userId;
                int value;
                try
                {
                    itemId = Guid.Parse(data.ItemId);
                    userId = Guid.Parse(data.UserId);
                    value = Convert.ToInt32(data.Value);

                    // шукаємо чи є оцінка з даними параметрами
                    Rate? rate = _dataContext.Rates
                        .FirstOrDefault(r => r.ItemId == itemId && r.UserId == userId);
                    if (rate == null)  // нова оцінка -- створюємо
                    {
                        _dataContext.Rates.Add(new()
                        {
                            ItemId = itemId,
                            UserId = userId,
                            Rating = value
                        });
                        _dataContext.SaveChanges();
                        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
                        result = $"Дані внесені";
                    }
                    else if(rate.Rating != value)  // оцінка є, але приходять новий рейтинг (зміна)
                    {
                        rate.Rating = value;
                        _dataContext.SaveChanges();
                        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
                        result = $"Дані оновлено";
                    }
                    else  // оцінка існує з тим самим рейтингом -- ігноруємо
                    {
                        HttpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                        result = $"Дані вже наявні user={data?.UserId} item={data?.ItemId}";
                    }  
                }
                catch
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    result = $"Параметри не пройшли валідацію: value={data?.Value} user={data?.UserId} item={data?.ItemId}";
                }
            }
            return new { result };
        }

        [HttpPut]
        public object Put([FromBody] RequestData data)
        {
            return new { result = $"Надійшов запит методом PUT з value={data.Value}" };
        }

        [HttpDelete]
        public object Delete([FromBody] RequestData data)
        {
            String result = null!;
            if (data == null
             || data.ItemId == null
             || data.Value == null
             || data.UserId == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                result = $"Недостатня кількість параметрів: value={data?.Value} user={data?.UserId} item={data?.ItemId}";
            }
            else
            {
                Guid itemId, userId;
                int value;
                try
                {
                    itemId = Guid.Parse(data.ItemId);
                    userId = Guid.Parse(data.UserId);
                    value = Convert.ToInt32(data.Value);

                    Rate? rate = _dataContext.Rates
                        .FirstOrDefault(r => r.ItemId == itemId && r.UserId == userId);
                    if (rate is null)
                    {
                        HttpContext.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                        result = $"Дані відсутні у БД (не можна видалити) user={data?.UserId} item={data?.ItemId}";
                    }
                    else
                    {
                        _dataContext.Rates.Remove(rate);
                        _dataContext.SaveChanges();

                        HttpContext.Response.StatusCode = StatusCodes.Status202Accepted;
                        result = $"Дані видалені";
                    }
                }
                catch
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    result = $"Параметри не пройшли валідацію: value={data?.Value} user={data?.UserId} item={data?.ItemId}";
                }
            }
            return new { result };
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
        public String? ItemId { get; set; }
        public String? UserId { get; set; }
        public String? Value  { get; set; }
    }
    /* Д.З. Додати перевірку статусу відповіді після запиту на додавання рейтингу.
     * При успішному статусі оновлювати сторінку (оновлювати дані рейтингу) */
}
