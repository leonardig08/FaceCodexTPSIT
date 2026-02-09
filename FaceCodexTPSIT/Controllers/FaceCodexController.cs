using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FaceCodexTPSIT.Models;
using FaceCodexTPSIT.Services;

namespace FaceCodexTPSIT.Controllers
{
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
        [HttpPost("aggiungiPersonaReale")]
        public async Task<IActionResult> AggiungiPersonaReale([FromBody] AddPersonaRequest request)
        {
            var uid = $"{request.Nome.ToLower()}.{request.Cognome.ToLower()}@{NamespaceName}";

            var imageUploadUrl = await _skyBiometryService.uploadToImgBb(request.ImageUrl, request.Nome, request.Cognome);

            // 1. Face detect
            var detectResult = await _skyBiometryService.DetectFacesAsync(imageUploadUrl);

            var tags = detectResult.RootElement
                .GetProperty("photos")[0]
                .GetProperty("tags");

            if (tags.GetArrayLength() == 0)
            {
                return BadRequest("Nessun volto rilevato nell'immagine.");
            }

            var tid = tags[0].GetProperty("tid").GetString();

            // 2. Save tag
            await _skyBiometryService.SaveTagAsync(uid, tid);

            // 3. Train user
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
        [HttpPost("CheckPersona")]
        public async Task<IActionResult> CheckPersona([FromBody] StaticCheckRequest request)
        {
            

            var imageUploadUrl = await _skyBiometryService.uploadToImgBb(request.ImageUrl, randString(4), randString(4));
            var recognizeResult = await _skyBiometryService.RecognizeAsync(
                imageUploadUrl,
                NamespaceName
            );

            return Ok(recognizeResult.RootElement);
        }
    }
}
