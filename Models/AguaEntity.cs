namespace Apiaguas.Models;

// Entidade: representa a tabela "Aguas" no banco (uma linha = uma água).
public class AguaEntity
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;
}
