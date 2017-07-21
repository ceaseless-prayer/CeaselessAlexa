using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace Ceaseless
{
    public class Engine : IEngine
    {

        internal const string CeaselessBucketName = "ceaseless";

        private readonly IAmazonS3 amazonS3Client;

        public Engine(AWSCredentials awsCredentials, RegionEndpoint awsRegionEndpoint) :
            this(new AmazonS3Client(awsCredentials, awsRegionEndpoint))
        {
        }

        internal Engine(IAmazonS3 amazonS3Client)
        {
            this.amazonS3Client = amazonS3Client;
        }

        public async Task AddPersonToPray(string userId, string name)
        {
            var peopleToPray = await GetPeopleToPray(userId);
            peopleToPray.Add(name);

            var peopleToPrayBytes = Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(peopleToPray));
            var peopleToPrayStream = new MemoryStream(peopleToPrayBytes);
            await amazonS3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = CeaselessBucketName,
                Key = userId,
                InputStream = peopleToPrayStream
            });
        }

        public async Task<IList<string>> GetPeopleToPray(string userId)
        {
            try
            {
                using (var getObjectResponse = await amazonS3Client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = CeaselessBucketName,
                    Key = userId,
                }))
                {
                    using (var streamReader = new StreamReader(getObjectResponse.ResponseStream, Encoding.UTF32))
                    {
                        return JsonConvert.DeserializeObject<List<string>>(streamReader.ReadToEnd());
                    }
                }
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.ErrorCode == "NotFound")
                    return new List<string>();
                throw;
            }
        }
        
    }
}
