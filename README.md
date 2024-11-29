# Sistema de Reserva de Sala de Reuni�o (API)

Este projeto � uma API para gerenciamento de reservas de salas de reuni�o, desenvolvida em .NET 8 com MongoDB como banco de dados. A aplica��o foi containerizada para ser executada em Docker, incluindo tamb�m o Mongo-Express para visualiza��o e gerenciamento do banco de dados.

## Tecnologias Utilizadas
- .NET 8
- MongoDB
- Mongo-Express
- Docker

## Funcionalidades
- CRUD para reserva de salas de reuni�o (criar, visualizar, atualizar e excluir reservas)
- Visualiza��o das reservas no banco de dados atrav�s de Mongo-Express

## Requisitos

- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/install/)

## Configura��o do Ambiente

1. **Clone o reposit�rio:**
   
    git clone https://github.com/carlosdutraunisales/MeetingHub
    cd MeetingHub
    

2. **Execute o Docker Compose:**

    Este comando cria os containers para a API, MongoDB e Mongo-Express.

   
    docker-compose up -d
    

3. **Acessar a API:**

    A API estar� dispon�vel na URL `http://localhost:5000`.

4. **Acessar o Mongo-Express:**

    O Mongo-Express estar� acess�vel na URL `http://localhost:8081` usando as credenciais:
    - Usu�rio: **admin**
    - Senha: **pass**

## Estrutura do Projeto

- /MeetingHub: C�digo fonte da API.
- /MeetingHub.sln: Arquivo do projeto .net.
- /MeetingHub.Teste: Projeto de Testes
- /docker-compose.yml**: Arquivo para configura��o dos containers Docker.
- /README.md: Este arquivo README.

## Endpoints da API

- **GET /api/salas**: Retorna todas as salas de reuni�o.
- **GET /api/salas/{id}**: Retorna uma salas de reuni�o espec�fica.
- **GET /api/salas/disponiveis**: Retorna as salas de reuni�o 
      disponiveis em determinada data/horario, precisa passar 
      pelo body a data, a capacida e uma lista de recursos.
- **POST /api/salas**: Cria uma nova sala de reuni�o.
- **PUT /api/salas/{id}**: Atualiza uma salas de reuni�o espec�fica.
- **DELETE /api/salas/{id}**: Exclui uma salas de reuni�o espec�fica.

- **GET /api/usuarios**: Retorna todas as salas de reuni�o.
- **POST /api/usuarios**: Cria uma nova sala de reuni�o.
- **GET /api/usuarios/{id}**: Retorna todas as salas de reuni�o.
- **GET /api/usuarios/{email}**: Retorna todas as salas de reuni�o.
- **PUT /api/usuarios/{id}**: Atualiza uma salas de reuni�o espec�fica.
- **DELETE /api/usuarios/{id}**: Exclui uma salas de reuni�o espec�fica.
- **POST /api/usuairos/login**: Cria uma nova sala de reuni�o.
- **GET /api/usuarios/me**: Retorna todas as salas de reuni�o.

- **POST /api/reservas**: Cria uma nova reserva.
- **GET /api/reservas/{id}**: Retorna uma reserva em especifico.
- **PUT /api/reservas/{id}**: Atualiza uma reserva existente.
- **DELETE /api/reservas/{id}**: Exclui uma reserva.
- **GET /api/reservas/todas**: Retorna todas as reservas.
- **GET /api/reservas/proprias**: Retorna todas as reservas do usu�rio logado.

## Executando Testes

Para executar os testes, use o seguinte comando:

Todos os teste podem ser realizados utilizando o postman atraves do endere�o

http://localhost:5029