# Manifesto — Repositório Git

Bem-vindo ao repositório do ERP Andreense. Se você acabou de clonar este projeto, percebeu que ele não vai compilar de primeira se você tentar rodar o banco de dados.

Isso acontece porque **nós não enviamos segredos para o GitHub**.

## O que foi ignorado pelo `.gitignore`?

Para manter a segurança do ecossistema, as seguintes pastas e arquivos estão bloqueados de subir para o Git:

1. `appsettings.Development.json` e `appsettings.Production.json` (onde ficam as strings de conexão do banco de produção e senhas de e-mail/Cloudflare).
2. A pasta `Documentação/Cofre_Privado/` (onde o Titular do projeto guarda backups, anotações de servidores KingHost e acessos Google).
3. Lixos de compilação padrão do .NET (`bin`, `obj`, `Logs`).

## Como Rodar o Projeto numa Máquina Nova?

1. Peça ao Titular do projeto a string de conexão do Banco de Dados (Supabase ou Localhost).
2. Peça a senha de aplicativo do Gmail (`EmailSettings:SenderPassword`).
3. Vá na pasta `c:\Projeto\Sistema\Modulo_Monolitico\ModuloMonolitico.Web\` e crie um arquivo chamado `appsettings.Development.json`.
4. Copie o conteúdo de `appsettings.json` para dentro dele.
5. Preencha as senhas reais no `appsettings.Development.json` recém-criado.

> **Importante:** O arquivo `appsettings.json` (que é comitado no Git) deve **sempre** ficar com as senhas vazias ou falsas. Ele serve apenas como um "template" para outros programadores saberem quais chaves existem.
