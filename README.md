# Sistema de Reserva de Sala de Reunião (API)

Este projeto é uma API para gerenciamento de reservas de salas de reunião, desenvolvida em .NET 8 com MongoDB como banco de dados. A aplicação foi containerizada para ser executada em Docker, incluindo também o Mongo-Express para visualização e gerenciamento do banco de dados.

## Tecnologias Utilizadas
- .NET 8
- MongoDB
- Mongo-Express
- Docker

## Funcionalidades
- CRUD para reserva de salas de reunião (criar, visualizar, atualizar e excluir reservas)
- Visualização das reservas no banco de dados através de Mongo-Express

## Requisitos

- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/)

## Configuração do Ambiente

1. **Clone o repositório:**
   
    git clone https://github.com/carlosdutraunisales/MeetingHub
    cd MeetingHub
    

2. **Execute o Docker Compose:**

    Este comando cria os containers para a API, MongoDB e Mongo-Express.

   
    docker-compose up -d
    

3. **Acessar a API:**

    A API estará disponível na URL `http://localhost:5000`.

4. **Acessar o Mongo-Express:**

    O Mongo-Express estará acessível na URL `http://localhost:8081` usando as credenciais:
    - Usuário: **admin**
    - Senha: **pass**

## Estrutura do Projeto

- /MeetingHub: Código fonte da API.
- /MeetingHub.sln: Arquivo do projeto .net.
- /MeetingHub.Teste: Projeto de Testes
- /docker-compose.yml**: Arquivo para configuração dos containers Docker.
- /README.md: Este arquivo README.

## Endpoints da API

- **GET /api/salas**: Retorna todas as salas de reunião.
- **GET /api/salas/{id}**: Retorna uma salas de reunião específica.
- **GET /api/salas/disponiveis**: Retorna as salas de reunião 
      disponiveis em determinada data/horario, precisa passar 
      pelo body a data, a capacida e uma lista de recursos.
- **POST /api/salas**: Cria uma nova sala de reunião.
- **PUT /api/salas/{id}**: Atualiza uma salas de reunião específica.
- **DELETE /api/salas/{id}**: Exclui uma salas de reunião específica.

- **GET /api/usuarios**: Retorna todas as salas de reunião.
- **POST /api/usuarios**: Cria uma nova sala de reunião.
- **GET /api/usuarios/{id}**: Retorna todas as salas de reunião.
- **GET /api/usuarios/{email}**: Retorna todas as salas de reunião.
- **PUT /api/usuarios/{id}**: Atualiza uma salas de reunião específica.
- **DELETE /api/usuarios/{id}**: Exclui uma salas de reunião específica.
- **POST /api/usuairos/login**: Cria uma nova sala de reunião.
- **GET /api/usuarios/me**: Retorna todas as salas de reunião.

- **POST /api/reservas**: Cria uma nova reserva.
- **GET /api/reservas/{id}**: Retorna uma reserva em especifico.
- **PUT /api/reservas/{id}**: Atualiza uma reserva existente.
- **DELETE /api/reservas/{id}**: Exclui uma reserva.
- **GET /api/reservas/todas**: Retorna todas as reservas.
- **GET /api/reservas/proprias**: Retorna todas as reservas do usuário logado.

## Executando Testes

Para executar os testes, use o seguinte comando:

Todos os teste podem ser realizados utilizando o postman atraves do endereço

http://localhost:5029