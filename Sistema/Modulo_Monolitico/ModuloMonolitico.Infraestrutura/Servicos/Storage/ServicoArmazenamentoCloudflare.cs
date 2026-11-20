using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModuloMonolitico.Dominio.InterfacesDeServico;
using ModuloMonolitico.Infraestrutura.Configuracoes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ModuloMonolitico.Infraestrutura.Servicos.Storage
{
    public class ServicoArmazenamentoCloudflare : IServicoArmazenamento, IDisposable
    {
        private readonly CloudflareConfig _config;
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<ServicoArmazenamentoCloudflare> _logger;

        public ServicoArmazenamentoCloudflare(IOptions<CloudflareConfig> config, ILogger<ServicoArmazenamentoCloudflare> logger)
        {
            _config = config.Value;
            _logger = logger;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = _config.S3ApiEndpoint,
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            };

            var credentials = new BasicAWSCredentials(_config.AccessKey, _config.SecretKey);
            _s3Client = new AmazonS3Client(credentials, s3Config);
        }

        public async Task<string> EnviarArquivoAsync(Stream stream, string nomeArquivo, string tipoConteudo)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = _config.BucketName,
                    Key = nomeArquivo,
                    InputStream = stream,
                    ContentType = tipoConteudo,
                    DisablePayloadSigning = true,
                    DisableDefaultChecksumValidation = true
                };

                await _s3Client.PutObjectAsync(request);
                return ObterUrlPublica(nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CloudflareStorage] Erro ao fazer upload do arquivo {FileName}", nomeArquivo);
                throw;
            }
        }

        public async Task DeletarFisicoArquivoAsync(string nomeArquivo)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _config.BucketName,
                    Key = nomeArquivo
                };

                await _s3Client.DeleteObjectAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CloudflareStorage] Erro ao deletar o arquivo {FileName}", nomeArquivo);
                throw;
            }
        }

        public string ObterUrlPublica(string nomeArquivo)
        {
            var baseUrl = _config.PublicBucketUrl.TrimEnd('/');
            var chave = nomeArquivo.TrimStart('/');
            return $"{baseUrl}/{chave}";
        }

        public void Dispose()
        {
            _s3Client?.Dispose();
        }
    }
}
