using Azure.Storage.Blobs;

namespace WebAPI.Utils
{
    // Define uma classe estática chamada AzureBlobStorageHelper
    public static class AzureBlobStorageHelper
    {
        // Método para upload de imagem que recebe a imagem, a string de conexão do blob e o nome do container
        public static async Task<string> UploadImagemBlobAsync(IFormFile arquivo, string connectionString, string containerName)
        {
            // Verifica se o arquivo é válido
            if (arquivo != null)
            {
                // Gera um nome único para o blob usando GUID e a extensão do arquivo
                var blobName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(arquivo.FileName);

                // Cria uma instância do cliente BlobServiceClient com a string de conexão fornecida
                var blobServiceClient = new BlobServiceClient(connectionString);

                // Obtém um cliente BlobContainerClient usando o nome do contêiner fornecido
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                // Obtém um cliente BlobClient usando o nome do blob gerado
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                // Abre o fluxo de entrada para o arquivo
                using (var stream = arquivo.OpenReadStream())
                {
                    // Carrega o arquivo para o blob no Azure Blob Storage de forma assíncrona
                    await blobClient.UploadAsync(stream, true);
                }

                // Retorna o URI do blob recém-carregado como uma string
                return blobClient.Uri.ToString();
            }
            else
            {
                // Retorna uma URL de imagem padrão se nenhum arquivo for enviado
                return "https://blobvitalhub.blob.core.windows.net/containervitalhub/profile.jpg";
            }
        }
    }
}