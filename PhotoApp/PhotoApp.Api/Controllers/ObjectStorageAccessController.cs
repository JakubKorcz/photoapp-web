using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using PhotoApp.Api.Repository;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("storage")]
    public class ObjectStorageAccessController(IMinioClient minioClient, MediaRepository mediaRepository) : ControllerBase
    {
        private readonly MediaRepository _mediaRepository = mediaRepository;
        private readonly IMinioClient _minioClient = minioClient;

        [HttpGet("download/{mediaId}")]
        public async Task<ActionResult<string>> GetDownloadUrl([FromRoute] Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null) 
            { 
                return NotFound("Media not found");
            }

            var args = new PresignedGetObjectArgs()
             .WithBucket(media.Type.ToString().ToLower())
             .WithObject(media.ObjectKey)
             .WithExpiry(60 * 60);

            var url = await _minioClient.PresignedGetObjectAsync(args);
            return Ok(url);
        }

        [HttpGet("upload/{mediaId}")]
        public async Task<ActionResult<string>> GetUploadUrl([FromRoute] Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null)
            {
                return NotFound("Media not found");
            }

            var args = new PresignedPutObjectArgs()
            .WithBucket(media.Type.ToString().ToLower())
            .WithObject(media.ObjectKey)
            .WithExpiry(15 * 60);

            var url = await _minioClient.PresignedPutObjectAsync(args);
            return Ok(url);
        }

        [HttpDelete("delete/{mediaId}")]
        public async Task<ActionResult<string>> DeleteMedia([FromRoute] Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null)
            {
                return NotFound("Media not found");
            }

            var args = new RemoveObjectArgs()
            .WithBucket(media.Type.ToString().ToLower())
            .WithObject(media.ObjectKey);

            await _minioClient.RemoveObjectAsync(args);

            //TODO Oznacz media jako usunięte w bazie danych, żeby nie było problemów z późniejszymi próbami dostępu do tego media

            return NoContent();
        }
    }
}