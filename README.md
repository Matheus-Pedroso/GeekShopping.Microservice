# GeekShopping - Microsserviços com .NET 8

Este projeto é um sistema de e-commerce desenvolvido com arquitetura de microsserviços utilizando **.NET 8**, com integração de autenticação, mensageria e API Gateway.

## 🚀 Tecnologias Utilizadas

- **.NET 8** (ASP.NET Core Web API, Minimal APIs)
- **Entity Framework Core** (MySQL)
- **IdentityServer** (Autenticação e Autorização com JWT)
- **RabbitMQ** (Mensageria e processamento assíncrono)
- **Polly** (Resiliência e Retry Policies)
- **Ocelot** (API Gateway)
- **Swagger / OpenAPI** (Documentação das APIs)
- **Docker** (Containerização e orquestração dos serviços)

## 📦 Microsserviços Implementados

- **ProductAPI** → Gerenciamento de produtos
- **CartAPI** → Carrinho de compras e integração com cupons
- **CouponAPI** → Validação e gerenciamento de cupons de desconto
- **OrderAPI** → Processamento de pedidos
- **PaymentAPI** → Processamento de pagamentos
- **IdentityServer** → Autenticação e Autorização
- **API Gateway (Ocelot)** → Roteamento e controle centralizado

## 🔗 Comunicação Entre Serviços

- **Síncrona**: HTTP REST entre APIs
- **Assíncrona**: RabbitMQ para processamento de pedidos e pagamentos

## 📖 Funcionalidades

- Registro e autenticação de usuários (IdentityServer)
- Cadastro, consulta e gerenciamento de produtos
- Carrinho de compras integrado com cupons de desconto
- Criação e processamento de pedidos
- Processamento de pagamento via RabbitMQ
- Resiliência de conexão com RabbitMQ utilizando Polly
- API Gateway centralizando todas as rotas

