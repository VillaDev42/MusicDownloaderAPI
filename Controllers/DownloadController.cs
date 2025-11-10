using Microsoft.AspNetCore.Mvc;
using MusicDownloaderAPI.Services;

namespace MusicDownloaderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly DownloaderService _downloader;

        public DownloadController(DownloaderService downloader)
        {
            _downloader = downloader;
        }

        // Endpoint: POST /api/download/mp3
        [HttpPost("mp3")]
        public async Task<IActionResult> DownloadAsMp3([FromBody] DownloadRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("Debe enviar un enlace válido.");

            var result = await _downloader.DownloadAndConvertToMp3Async(request.Url);

            if (result == null)
                return StatusCode(500, "Error al descargar o convertir el audio.");

            // Devuelve el archivo directamente como descarga
            var fileName = Path.GetFileName(result);

            var fileBytes = await System.IO.File.ReadAllBytesAsync(result);

            // Eliminar archivo temporal
            System.IO.File.Delete(result);

            // Devolver el archivo al cliente
            return File(fileBytes, "audio/mpeg", fileName);

        }
    }

    public class DownloadRequest
    {
        public string Url { get; set; }
    }
}

