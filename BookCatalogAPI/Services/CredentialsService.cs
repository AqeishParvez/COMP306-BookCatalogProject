using Amazon.SimpleSystemsManagement;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement.Model;

namespace BookCatalogAPI.Services
{
    public class CredentialsService
    {
        // Retrieve AWS Credentials from Parameter Store
        public AWSCredentials GetAWSCredentials(IConfiguration configuration)
        {
            var awsAccessKeyParameter = "/BookCatalogApi/S3AccessKey";
            var awsSecretKeyParameter = "/BookCatalogApi/S3SecretKey";

            var accessKey = configuration.GetValue<string>(awsAccessKeyParameter);
            var secretKey = configuration.GetValue<string>(awsSecretKeyParameter);

            return new BasicAWSCredentials(accessKey, secretKey);
        }

        // Retrieve MongoDB connection string from Parameter Store
        public string GetMongoDbConnectionString(IConfiguration configuration, IAmazonSimpleSystemsManagement ssmClient)
        {
            var parameterName = "/BookCatalogApi/MongoDbConnectionString";

            var parameterRequest = new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = true // Decrypt if the parameter is a SecureString
            };

            var parameterResponse = ssmClient.GetParameterAsync(parameterRequest).Result;
            return parameterResponse.Parameter.Value;
        }

        // Retrieve MongoDB database name from Parameter Store
        public string GetMongoDbDatabaseName(IConfiguration configuration, IAmazonSimpleSystemsManagement ssmClient)
        {
            var parameterName = "/BookCatalogApi/MongoDbDatabaseName";

            var parameterRequest = new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = false // Database name is typically not a secret
            };

            var parameterResponse = ssmClient.GetParameterAsync(parameterRequest).Result;
            return parameterResponse.Parameter.Value;
        }
    }
}
