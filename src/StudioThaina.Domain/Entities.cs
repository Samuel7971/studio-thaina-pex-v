namespace StudioThaina.Domain;

public enum StatusAgendamento { Agendado = 1, Concluido = 2, Cancelado = 3 }

public sealed class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string? Observacao { get; set; }
    public bool Ativo { get; set; } = true;
    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome)) throw new ArgumentException("O nome do cliente é obrigatório.");
        if (string.IsNullOrWhiteSpace(Telefone)) throw new ArgumentException("O telefone do cliente é obrigatório.");
    }
}

public sealed class Servico
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string? Descricao { get; set; }
    public int DuracaoMinutos { get; set; }
    public decimal Valor { get; set; }
    public bool Ativo { get; set; } = true;
    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nome)) throw new ArgumentException("O nome do serviço é obrigatório.");
        if (DuracaoMinutos <= 0) throw new ArgumentException("A duração deve ser maior que zero.");
        if (Valor < 0) throw new ArgumentException("O valor não pode ser negativo.");
    }
}

public sealed class Agendamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int ServicoId { get; set; }
    public DateTime DataHora { get; set; }
    public decimal ValorCobrado { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
    public string? Observacao { get; set; }
    public void Validar()
    {
        if (ClienteId <= 0) throw new ArgumentException("Cliente inválido.");
        if (ServicoId <= 0) throw new ArgumentException("Serviço inválido.");
        if (ValorCobrado < 0) throw new ArgumentException("O valor cobrado não pode ser negativo.");
        if (!Enum.IsDefined(Status)) throw new ArgumentException("Status inválido.");
    }
}
