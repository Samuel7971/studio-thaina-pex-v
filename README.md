# Studio Thainá - PEX V

Protótipo acadêmico de um sistema de gestão de agendamentos e indicadores
desenvolvido para o Studio Thainá Santos.

O projeto foi desenvolvido como parte do Projeto de Extensão V do curso
de Análise e Desenvolvimento de Sistemas.

## Sobre o projeto

O Studio Thainá Santos realiza o controle de clientes, serviços,
agendamentos e informações financeiras predominantemente de forma manual,
utilizando WhatsApp, agenda e anotações.

O objetivo deste protótipo é demonstrar uma solução capaz de centralizar
essas informações e disponibilizar indicadores simples para apoio à gestão.

## Funcionalidades

- Cadastro e consulta de clientes
- Cadastro e consulta de serviços
- Gestão de agendamentos
- Consulta da agenda
- Alteração do status dos agendamentos
- Dashboard gerencial
- Faturamento por período
- Quantidade de atendimentos
- Ticket médio
- Serviços mais realizados
- Projeção simples de ganhos

## Tecnologias

- .NET 8
- C#
- ASP.NET Core Web API
- Blazor
- Dapper
- SQL Server
- Swagger / OpenAPI

## Arquitetura

A solução foi organizada em cinco projetos:

- `StudioThaina.Domain` - entidades e regras de domínio
- `StudioThaina.Application` - contratos, DTOs e serviços da aplicação
- `StudioThaina.Infrastructure` - persistência com Dapper e SQL Server
- `StudioThaina.Api` - API REST e configuração da aplicação
- `StudioThaina.Web` - interface desenvolvida em Blazor

A interface Blazor não acessa diretamente o banco de dados. A comunicação
é realizada através da API REST.

## Banco de dados

O projeto utiliza SQL Server.

O script para criação do banco, tabelas e massa de dados fictícia está em:

src/StudioThaina.Infrastructure/Scripts/01-create-database.sql

O script cria o banco `StudioThaina` e adiciona dados fictícios para
demonstração acadêmica.

## Execução local

1. Executar o script `01-create-database.sql` no SQL Server.
2. Conferir a connection string da API.
3. Executar `StudioThaina.Api`.
4. Executar `StudioThaina.Web`.
5. Acessar a aplicação pelo navegador.

O Swagger pode ser utilizado para consultar e testar os endpoints da API.

## Dados de demonstração

Todos os clientes, serviços, agendamentos e valores presentes na massa
inicial são fictícios e utilizados exclusivamente para demonstração
acadêmica.

Nenhum dado real de cliente é disponibilizado neste repositório.

## Observações

Este projeto é um protótipo acadêmico.

A integração oficial com WhatsApp não faz parte desta versão.

A projeção de ganhos é demonstrativa e utiliza uma média baseada no
histórico informado, não representando uma previsão financeira garantida.

O projeto não foi desenvolvido com objetivo de utilização em ambiente
de produção.

## Autor

Samuel Rodrigues de Souza

Projeto de Extensão V - Análise e Desenvolvimento de Sistemas