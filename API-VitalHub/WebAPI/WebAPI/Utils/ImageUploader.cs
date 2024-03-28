namespace WebAPI.Utils
{
    public static class ImageUploader
    {
        // Método para upload de imagem que recebe a imagem e o diretório de destino para salvar a imagem
        public static string UploadImage(IFormFile imageFile, string destinationDirectory)
        {
            // Verifica se o arquivo de imagem é nulo ou tem tamanho zero
            if (imageFile == null || imageFile.Length == 0)
            {
                // Retorna nulo se o arquivo for inválido
                return null!;
            }

            // Gera um nome único para o arquivo
            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(imageFile.FileName);

            // Verifica se o diretório de destino especificado existe.
            if (!Directory.Exists(destinationDirectory))
            {
                // Cria o diretório se não existir
                Directory.CreateDirectory(destinationDirectory);
            }

            // Gera o caminho completo até o arquivo
            var path = Path.Combine(destinationDirectory, fileName);

            // Usa a instrução 'using' para garantir que o recurso FileStream seja fechado corretamente após o uso
            using (var stream = new FileStream(path, FileMode.Create))
            {
                // Copia o conteúdo do arquivo para o FileStream
                imageFile.CopyTo(stream); 
            }

            // Retorna o nome do arquivo que foi carregado com sucesso
            return fileName; // Retorna o nome do arquivo que foi carregado com sucesso.
        }
    }
}
