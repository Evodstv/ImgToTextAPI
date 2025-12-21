using Tesseract;

public class ImgToTextService
{
    private readonly IWebHostEnvironment _env;

    public ImgToTextService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public string RecognizeText(string imagePath, string language)
    {
        // если язык не передан — по умолчанию rus
        if (string.IsNullOrWhiteSpace(language))
            language = "rus";

        // нормализация: rus, eng → rus+eng
        language = language
            .Replace(",", "+")
            .Replace(" ", "")
            .ToLower();

        var tessDataPath = Path.Combine(
            _env.ContentRootPath,
            "tessdata"
        );

        // проверка: существуют ли модели
        foreach (var lang in language.Split('+'))
        {
            var file = Path.Combine(tessDataPath, $"{lang}.traineddata");
            if (!File.Exists(file))
            {
                throw new Exception($"Язык '{lang}' не найден в tessdata");
            }
        }

        using var engine = new TesseractEngine(
            tessDataPath,
            language,
            EngineMode.Default
        );

        engine.DefaultPageSegMode = PageSegMode.Auto;

        using var img = Pix.LoadFromFile(imagePath);
        using var page = engine.Process(img);

        return page.GetText();
    }
}
