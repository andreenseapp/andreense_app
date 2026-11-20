# Arquitetura de Autenticação e Login

Este documento descreve detalhadamente o funcionamento dos fluxos de autenticação do ERP Monolítico, englobando o login tradicional (E-mail e Senha) e a integração de login social (Google OAuth). 

Serve como referência arquitetural e guia de troubleshooting para os desenvolvedores e mantenedores do projeto.

---

## 1. Fluxo de Login Tradicional (E-mail e Senha)

O fluxo de login padrão foi desenhado para ser seguro, stateless (via JWT) e capaz de rastrear dispositivos (Sessões e Fingerprints).

1. **Recepção:** O frontend envia e-mail, senha, IP e User-Agent para o backend via `LoginCommand`.
2. **Validação de Credenciais:** 
   - O `LoginCommandHandler` busca o usuário no banco de dados via `UsuarioRepositorio`.
   - A senha fornecida é hasheada usando BCrypt e comparada com o `SenhaHash` persistido.
   - Valida-se se o usuário está `Ativo`.
3. **Mapeamento de Contexto:** 
   - O sistema busca a `Conta` principal (Workspace) vinculada ao usuário.
   - Lista todas as `Empresas` vinculadas a essa conta que o usuário tem acesso.
4. **Segurança e Rastreabilidade:**
   - Um **Fingerprint** único é gerado a partir do IP e User-Agent, garantindo que o token não seja facilmente roubado e utilizado em outro dispositivo.
   - Uma **Sessão** é criada na tabela `sessoes_usuarios`, contendo um `RefreshToken` assinado. Isso permite que o usuário permaneça logado por vários dias, além de possibilitar a revogação de acessos remotamente.
5. **Emissão do Token:** O `ITokenService` gera um JWT (`AccessToken`) contendo os *claims* de ID do Usuário, Conta, Empresas e Fingerprint.
6. **Entrega:** O AccessToken (curta duração) e o RefreshToken (longa duração) são devolvidos ao Frontend (via Cookies HTTP Only, protegendo contra ataques XSS).

---

## 2. Fluxo de Login com Google (SSO / Identity Services)

A integração com o Google utiliza o moderno fluxo de `id_token` fornecido pelo **Google Identity Services**, ideal para aplicações web (Single Page Applications ou Razor Pages com Fetch).

### A. Requisitos de Infraestrutura (Google Cloud Console)
Para que o botão do Google sequer apareça habilitado na interface web, os seguintes passos são estritamente necessários "por trás dos panos":

1. **Credencial do Tipo Correto:** É obrigatório utilizar um Client ID do tipo **"Web application"** no Google Cloud Console. Chaves geradas para Android/iOS não possuem suporte a chamadas de navegadores web e retornarão erros como `invalid_client`.
2. **Origens JavaScript Autorizadas (CORS/Origem):** A URL exata da aplicação (ex: `http://localhost:5081` ou `https://andreense.com.br`) **deve** estar cadastrada na lista de origens autorizadas daquela credencial no Google. Caso contrário, o frontend receberá o erro `no registered origin`.
3. **Appsettings:** O `ClientId` Web deve estar configurado no `appsettings.json`. O `ClientSecret` não é necessário neste fluxo específico, pois a validação é feita via assinatura pública no backend.

### B. O Fluxo Lógico (Frontend para Backend)
1. O usuário clica em "Entrar com Google". Uma janela segura do Google é aberta pelo frontend.
2. Após o consentimento, o Google devolve ao frontend um `id_token` (JWT emitido pelo Google contendo o e-mail verificado, nome e foto).
3. O frontend envia APENAS o `id_token` (sem nenhuma senha) para o backend através da nossa API/Page.

### C. A Mágica no Backend (`LoginGoogleCommandHandler`)
1. **Validação Criptográfica:** O `GoogleAuthService` baixa as chaves públicas do Google, verifica a assinatura do `id_token` e garante que o e-mail realmente pertence a quem enviou a requisição. **Esta etapa impede que hackers forjem logins.**
2. **Identificação do Usuário:** O sistema busca o e-mail validado no nosso banco de dados.

### D. O "Auto-Cadastro Mágico" (Criação Transparente)
Se o e-mail retornado pelo Google **não existir** no nosso banco de dados, o sistema executa um fluxo de auto-cadastro imperceptível ao usuário.

Isso previne que o usuário receba erros frustrantes como "Nenhuma conta encontrada" após já ter se autenticado com sucesso no Google. O backend automaticamente:
1. Gera uma **senha aleatória gigante** (hasheada em BCrypt) apenas para preencher o requisito estrutural da tabela.
2. Cria o registro do `Usuario` com `EmailValidado = true`. **(Benefício Exclusivo):** Como o Google já assegura a autenticidade do dono da conta, o sistema dispensa a etapa de confirmação manual de e-mail. Usuários que se cadastram manualmente pelo formulário padrão precisam passar pelo fluxo de verificação (clique no link enviado ao e-mail) antes de terem este status.
3. Cria um Workspace (`Conta`).
4. Cria uma `Empresa` fictícia (padrão LTDA, CNPJ `00000000000000`) atrelada a essa Conta.
5. Vincula o Usuário a essa Conta e Empresa, colocando-o em um `Grupo de Permissão` do tipo Administrador.
6. **(Atenção Técnica):** Durante essa criação em cascata via Dapper, foi essencial garantir que o código **não enviasse colunas obsoletas** (como o antigo booleano `ativo` ou a string fixa `perfil` nas tabelas de vínculos), deixando que o banco de dados PostgreSQL assumisse os `DEFAULT` modernos (como `status` e estrutura de grupos), evitando exceções estritas do Npgsql (ex: `column does not exist`).

Após o Auto-Cadastro, o fluxo segue normalmente gerando a Sessão, Fingerprint e JWT do ERP, e redirecionando o usuário já logado.
