using Microsoft.AspNetCore.Cors.Infrastructure;

using Microsoft.AspNetCore.Mvc;



[ApiController]

[Route("api/[controller]")]

public class ImgToTextController : ControllerBase

{

    private readonly ImgToTextService _ImgTotextService;



    public ImgToTextController(ImgToTextService ImgToTextService)

    {

        _ImgTotextService = ImgToTextService;

    }



    // GET: api/imgtotext



    /// <summary>

    /// Проверка доступности API

    /// </summary>

    /// <returns>Статус сервиса</returns>

    /// 

    [HttpGet]

    public IActionResult Get()

    {

        return Ok("ImgToText API работает");

    }



    // POST: api/imgtotext



    /// <summary>

    /// Распознаёт текст на загруженном изображении

    /// </summary>

    /// <param name="request">Изображение с текстом и язык распознавания</param>

    /// <returns>Распознанный текст</returns>

    /// 

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Post([FromForm] ImgToTextRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
            return BadRequest("Изображение не загружено");

        var tempFile = Path.GetTempFileName();

        try
        {
            // Сохраняем файл
            using (var stream = new FileStream(tempFile, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            // Пытаемся распознать
            var text = _ImgTotextService.RecognizeText(tempFile, request.Language);

            return Ok(new
            {
                Language = request.Language,
                RecognizedText = text
            });
        }
        catch (Exception ex)
        {
            // ex.InnerException — это то, что нам нужно
            var realMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return StatusCode(500, new
            {
                error = "Ошибка при обработке изображения",
                details = realMessage
            });
        }
        finally
        {
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }
        }
    }

}