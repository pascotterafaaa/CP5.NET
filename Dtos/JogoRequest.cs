using System.ComponentModel.DataAnnotations;

namespace JogosApi.Dtos;

/// <summary>Payload de entrada para POST e PUT (o Id nunca vem do cliente).</summary>
public class JogoRequest
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A plataforma é obrigatória.")]
    [StringLength(60)]
    public string Plataforma { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [StringLength(60)]
    public string Genero { get; set; } = string.Empty;

    [Range(0, 100000, ErrorMessage = "O preço deve ser maior ou igual a zero.")]
    public decimal Preco { get; set; }

    [Range(1950, 2100, ErrorMessage = "Ano de lançamento inválido.")]
    public int AnoLancamento { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque não pode ser negativo.")]
    public int Estoque { get; set; }
}
