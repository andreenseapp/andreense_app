namespace ModuloMonolitico.Infraestrutura.Configuracoes
{
    public class CloudflareConfig
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string S3ApiEndpoint { get; set; } = string.Empty;
        public string PublicBucketUrl { get; set; } = string.Empty;
    }
}
