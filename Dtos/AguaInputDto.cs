namespace Apiaguas.Dtos;

// Tipo de entrada (DTO): só o campo que o cliente pode enviar no POST e no PUT.
// O Id não está aqui de propósito: quem gera o identificador é o banco.
public record AguaInputDto(string Titulo = "");
