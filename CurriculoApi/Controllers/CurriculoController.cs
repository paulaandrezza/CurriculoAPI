using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CurriculoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurriculoController : Controller
    {
        private readonly IContentTypeProvider _contentTypeProvider;

        public CurriculoController(IContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        [HttpPost]
        public async Task<IActionResult> PostCurriculo(IFormFile curriculo)
        {
            var caminho = $"wwwroot/{curriculo.FileName}";

            if (ValidateFile(curriculo) != null)
            {
                return ValidateFile(curriculo);
            }

            using (var stream = System.IO.File.Create(caminho))
            {
                await curriculo.CopyToAsync(stream);
            }

            return Ok(new { Message = "Curriculo enviado com sucesso!" });
        }

        [HttpGet]
        public IActionResult GetCurriculo(string nomeArquivo)
        {
            var caminho = $"wwwroot/{nomeArquivo}";
            if (!System.IO.File.Exists(caminho))
            {
                return BadRequest(new { Message = $"Arquivo não existe: {nomeArquivo}" });
            }

            var fileInBytes = System.IO.File.ReadAllBytes(caminho);

            var contentType = "application/octet-stream";
            if (_contentTypeProvider.TryGetContentType(caminho, out var contentTypeProvided))
            {
                contentType = contentTypeProvided;
            }

            return File(fileInBytes, contentType);
        }

        private IActionResult? ValidateFile(IFormFile curriculo)
        {
            if (curriculo.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { ErrorMessage = "Tamanho excede o limite permitido de 5MB" });
            }

            string[] extensoesPermitidas = [".pdf", ".doc", ".docx"];

            var extensaoArquivo = Path.GetExtension(curriculo.FileName);
            if (!extensoesPermitidas.Contains(extensaoArquivo))
            {
                return BadRequest(new { ErrorMessage = "Extensão de arquivo inválida. Por favor envie um arquivo em pdf, doc ou docx" });
            }

            return null;
        }
    }
}
