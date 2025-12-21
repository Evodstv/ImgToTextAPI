
using Microsoft.AspNetCore.Http;



public class ImgToTextRequest

{

    public IFormFile Image { get; set; }

    public string Language { get; set; } = "rus"; //по дефолту русский

}