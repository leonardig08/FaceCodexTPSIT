using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FaceCodexTPSIT.Models;
using FaceCodexTPSIT.Services;
using Microsoft.AspNetCore.Authorization;

namespace FaceCodexTPSIT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/facecodex")]
    public class FaceCodexController : ControllerBase
    {
        private readonly SkyBiometryService _skyBiometryService;
        private const string NamespaceName = "FaceCodexDataSpace";

        public FaceCodexController(SkyBiometryService skyBiometryService)
        {
            _skyBiometryService = skyBiometryService;
        }

        private string randString(int size)
        {
            Random res = new Random();

            // String that contain both alphabets and numbers
            String str = "abcdefghijklmnopqrstuvwxyz0123456789";

            // Initializing the empty string
            String randomstring = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly
                int x = res.Next(str.Length);

                // Appending the character at the 
                // index to the random alphanumeric string.
                randomstring = randomstring + str[x];
            }
            return randomstring;
        }

        /// <summary>
        /// Aggiunge una persona reale al database biometrico
        /// </summary>
        [HttpPost("AddPerson")]
        public async Task<IActionResult> AggiungiPersonaReale([FromBody] AddPersonaRequest request)
        {
            var uid = $"{request.Nome.ToLower()}.{request.Cognome.ToLower()}@{NamespaceName}";

            // Costruisci percorso fisico del file in wwwroot/imgs
            var localPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", request.ImageUrl);

            if (!System.IO.File.Exists(localPath))
                return BadRequest($"Immagine '{request.ImageUrl}' non trovata nella cartella imgs.");

            // Costruisci URL pubblico
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/imgs/{request.ImageUrl}";

            // Carica su ImgBB
            var imageUploadUrl = await _skyBiometryService.uploadToImgBb(imageUrl, request.Nome, request.Cognome);

            var detectResult = await _skyBiometryService.DetectFacesAsync(imageUploadUrl);

            var tags = detectResult.RootElement
                .GetProperty("photos")[0]
                .GetProperty("tags");

            if (tags.GetArrayLength() == 0)
                return BadRequest("Nessun volto rilevato nell'immagine.");

            var tid = tags[0].GetProperty("tid").GetString();

            await _skyBiometryService.SaveTagAsync(uid, tid);
            await _skyBiometryService.TrainUserAsync(uid);

            return Ok(new
            {
                uid,
                message = "Persona inserita e addestrata con successo"
            });
        }


        /// <summary>
        /// Riconoscimento rapido senza inserimento nel database
        /// </summary>
        [HttpPost("CheckPerson")]
        public async Task<IActionResult> CheckPersona([FromBody] StaticCheckRequest request)
        {
            // Costruisci percorso fisico del file in wwwroot/imgs
            var localPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imgs", request.ImageUrl);

            if (!System.IO.File.Exists(localPath))
                return BadRequest($"Immagine '{request.ImageUrl}' non trovata nella cartella imgs.");

            // Costruisci URL pubblico
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/imgs/{request.ImageUrl}";

            // Carica su ImgBB
            var imageUploadUrl = await _skyBiometryService.uploadToImgBb(imageUrl, randString(4), randString(4));

            // Riconosci la persona
            var recognizeResult = await _skyBiometryService.RecognizeAsync(
                imageUploadUrl,
                NamespaceName
            );

            return Ok(recognizeResult.RootElement);
        }

    }
}
