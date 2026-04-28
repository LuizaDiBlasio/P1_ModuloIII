# Projeto Biblioteca - P1 Módulo III

Este projeto foi desenvolvido como parte da Avaliação P1 do Módulo III do curso **UpSkill**. Trata-se de um sistema de gestão de oficina de carros utilizando **C#** e **.NET 9*, focado na implementação de padrões de arquitetura modernos e persistência de dados em SQL Server.

## Tecnologias Utilizadas

* **Linguagem:** C#
* **Framework:** .NET 9
* **Base de Dados:** SQL Server
* **Padrão de Projeto:** Repository Pattern (Generic Repository)
* **Autenticação:** Token JWT bearer

## Arquitetura do Projeto

O projeto segue uma estrutura organizada em projetos separados para garantir a escalabilidade e facilidade de manutenção:

# 3 Tiers
* **Frontend:** Frontend em JavaScript
* **Camada de serviços:** API ASP NET Core, estabelece contato entre o frontend e backend por meio de protocolo HTTP
* **Camada de dados** Class Library que contém modelo de dados e repositórios com regras de negócios

* **Models:** Classes de domínio que representam as tabelas da base de dados (Carros, Contas, Marcas, Modelos).
* **ADONet:** Implementação de persistência de baixo nível utilizando ADO.NET (biblioteca DalPro) para manipulação direta de fluxos de dados.
* **Repositories:** Implementação de um Repositório Genérico para centralizar a lógica de acesso a dados (CRUD).
* **Helpers:** Classes com funcionalidades de autenticação e conexão à base de dados
* **Loggers:** Registro de erros
* **Dependency Injection:** Desacoplamento entre as camadas de serviço (API) e persistência (Repositories) por meio de Interfaces  

## Funcionalidades Principais
- [x] Gestão de Veículos (CRUD).
- [x] Filtro de buscas.


## Como Executar o Projeto

1. **Clonar o repositório:**
   ```bash
   git clone [https://github.com/LuizaDiBlasio/P1_ModuloIII.git](https://github.com/LuizaDiBlasio/P1_ModuloIII.git)
