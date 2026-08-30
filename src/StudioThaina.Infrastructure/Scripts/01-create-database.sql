USE master;
GO

IF DB_ID(N'StudioThaina') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [StudioThaina]');
END;
GO

USE StudioThaina;
GO

IF OBJECT_ID(N'dbo.Cliente', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cliente
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Cliente PRIMARY KEY,
        Nome NVARCHAR(120) NOT NULL,
        Telefone NVARCHAR(30) NOT NULL,
        Observacao NVARCHAR(500) NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Cliente_Ativo DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.Servico', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Servico
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Servico PRIMARY KEY,
        Nome NVARCHAR(120) NOT NULL,
        Descricao NVARCHAR(500) NULL,
        DuracaoMinutos INT NOT NULL,
        Valor DECIMAL(18,2) NOT NULL,
        Ativo BIT NOT NULL CONSTRAINT DF_Servico_Ativo DEFAULT (1)
    );
END;
GO

IF OBJECT_ID(N'dbo.Agendamento', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Agendamento
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Agendamento PRIMARY KEY,
        ClienteId INT NOT NULL,
        ServicoId INT NOT NULL,
        DataHora DATETIME2 NOT NULL,
        ValorCobrado DECIMAL(18,2) NOT NULL,
        Status INT NOT NULL,
        Observacao NVARCHAR(500) NULL,
        CONSTRAINT FK_Agendamento_Cliente
            FOREIGN KEY (ClienteId) REFERENCES dbo.Cliente(Id),
        CONSTRAINT FK_Agendamento_Servico
            FOREIGN KEY (ServicoId) REFERENCES dbo.Servico(Id)
    );
END;
GO

-- Dados exclusivamente fictícios para demonstração acadêmica.
IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0001')
    INSERT dbo.Cliente (Nome, Telefone, Observacao, Ativo)
    VALUES (N'Ana Lima', N'(11) 90000-0001', N'Cliente fictícia', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0002')
    INSERT dbo.Cliente (Nome, Telefone, Observacao, Ativo)
    VALUES (N'Bianca Souza', N'(11) 90000-0002', N'Cliente fictícia', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0003')
    INSERT dbo.Cliente (Nome, Telefone, Observacao, Ativo)
    VALUES (N'Carla Mendes', N'(11) 90000-0003', N'Cliente fictícia', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0004')
    INSERT dbo.Cliente (Nome, Telefone, Observacao, Ativo)
    VALUES (N'Daniela Alves', N'(11) 90000-0004', N'Cliente fictícia', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0005')
    INSERT dbo.Cliente (Nome, Telefone, Observacao, Ativo)
    VALUES (N'Elisa Rocha', N'(11) 90000-0005', N'Cliente fictícia inativa', 0);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Servico WHERE Nome = N'Extensão clássica')
    INSERT dbo.Servico (Nome, Descricao, DuracaoMinutos, Valor, Ativo)
    VALUES (N'Extensão clássica', N'Aplicação fio a fio', 120, 160.00, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Servico WHERE Nome = N'Volume brasileiro')
    INSERT dbo.Servico (Nome, Descricao, DuracaoMinutos, Valor, Ativo)
    VALUES (N'Volume brasileiro', N'Técnica de volume para cílios', 150, 210.00, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Servico WHERE Nome = N'Manutenção de extensão')
    INSERT dbo.Servico (Nome, Descricao, DuracaoMinutos, Valor, Ativo)
    VALUES (N'Manutenção de extensão', N'Manutenção periódica da extensão', 90, 110.00, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Servico WHERE Nome = N'Lash lifting')
    INSERT dbo.Servico (Nome, Descricao, DuracaoMinutos, Valor, Ativo)
    VALUES (N'Lash lifting', N'Curvatura e alinhamento dos cílios naturais', 75, 130.00, 1);
GO

-- O marcador abaixo impede a duplicação da massa de agendamentos.
IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Agendamento
    WHERE Observacao = N'DEMONSTRACAO-STUDIO-THAINA-01'
)
BEGIN
    DECLARE @Ana INT = (SELECT Id FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0001');
    DECLARE @Bianca INT = (SELECT Id FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0002');
    DECLARE @Carla INT = (SELECT Id FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0003');
    DECLARE @Daniela INT = (SELECT Id FROM dbo.Cliente WHERE Telefone = N'(11) 90000-0004');

    DECLARE @Classica INT = (SELECT Id FROM dbo.Servico WHERE Nome = N'Extensão clássica');
    DECLARE @Volume INT = (SELECT Id FROM dbo.Servico WHERE Nome = N'Volume brasileiro');
    DECLARE @Manutencao INT = (SELECT Id FROM dbo.Servico WHERE Nome = N'Manutenção de extensão');
    DECLARE @Lifting INT = (SELECT Id FROM dbo.Servico WHERE Nome = N'Lash lifting');
    DECLARE @Hoje DATETIME2 = CAST(CAST(GETDATE() AS DATE) AS DATETIME2);

    INSERT dbo.Agendamento
        (ClienteId, ServicoId, DataHora, ValorCobrado, Status, Observacao)
    VALUES
        (@Ana,     @Classica,   DATEADD(HOUR,  9, DATEADD(DAY, -28, @Hoje)), 160.00, 2, N'DEMONSTRACAO-STUDIO-THAINA-01'),
        (@Bianca,  @Volume,     DATEADD(HOUR, 14, DATEADD(DAY, -24, @Hoje)), 210.00, 2, N'Atendimento fictício concluído'),
        (@Carla,   @Classica,   DATEADD(HOUR, 10, DATEADD(DAY, -20, @Hoje)), 160.00, 2, N'Atendimento fictício concluído'),
        (@Daniela, @Manutencao, DATEADD(HOUR, 15, DATEADD(DAY, -16, @Hoje)), 110.00, 2, N'Atendimento fictício concluído'),
        (@Ana,     @Volume,     DATEADD(HOUR,  9, DATEADD(DAY, -12, @Hoje)), 210.00, 2, N'Atendimento fictício concluído'),
        (@Bianca,  @Classica,   DATEADD(HOUR, 13, DATEADD(DAY,  -8, @Hoje)), 160.00, 2, N'Atendimento fictício concluído'),
        (@Carla,   @Lifting,    DATEADD(HOUR, 16, DATEADD(DAY,  -5, @Hoje)), 130.00, 2, N'Atendimento fictício concluído'),
        (@Daniela, @Manutencao, DATEADD(HOUR, 11, DATEADD(DAY,  -2, @Hoje)), 110.00, 3, N'Agendamento fictício cancelado'),
        (@Ana,     @Classica,   DATEADD(HOUR,  9, DATEADD(DAY,   1, @Hoje)), 160.00, 1, N'Agendamento fictício futuro'),
        (@Bianca,  @Volume,     DATEADD(HOUR, 14, DATEADD(DAY,   2, @Hoje)), 210.00, 1, N'Agendamento fictício futuro'),
        (@Carla,   @Manutencao, DATEADD(HOUR, 10, DATEADD(DAY,   4, @Hoje)), 110.00, 1, N'Agendamento fictício futuro'),
        (@Daniela, @Lifting,    DATEADD(HOUR, 15, DATEADD(DAY,   6, @Hoje)), 130.00, 3, N'Agendamento fictício cancelado');
END;
GO
