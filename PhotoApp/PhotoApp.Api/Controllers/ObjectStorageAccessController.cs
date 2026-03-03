using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Service;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;
using System.Data.Entity.Core.Objects;
using System.Net.Mime;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("storage")]
    public class ObjectStorageAccessController(IConfiguration configuration, MediaRepository mediaRepository, IAmazonS3 amazonS3) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly MediaRepository _mediaRepository = mediaRepository;
        private readonly IAmazonS3 _s3Client = amazonS3;

        [HttpGet("download/{mediaId}")]
        public async Task<ActionResult<string>> GetDownloadUrl([FromRoute] Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null) 
            { 
                return NotFound("Media not found");
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = media.Type.ToString(),
                Key = media.ObjectKey,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Verb = HttpVerb.GET
            };
           
            return _s3Client.GetPreSignedURL(request);
        }

        [HttpGet("upload/{mediaId}")]
        public async Task<ActionResult<string>> GetUploadUrl([FromRoute] Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null)
            {
                return NotFound("Media not found");
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = media.Type.ToString(),
                Key = media.ObjectKey,
                Expires = DateTime.UtcNow.AddMinutes(15),
                Verb = HttpVerb.PUT
            };

            return _s3Client.GetPreSignedURL(request);
        }

        [HttpGet("delete/{mediaId}")]
        public async Task<ActionResult<string>> GetDeleteUrl([FromRoute] Guid mediaId)
        {
            var media = await _mediaRepository.GetMediaByIdAsync(mediaId);
            if (media == null)
            {
                return NotFound("Media not found");
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = media.Type.ToString(),
                Key = media.ObjectKey,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Verb = HttpVerb.DELETE
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}