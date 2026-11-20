Inteligência Artificial (agente) não é pra vc fazer nenhum item daqui sozinho, eu vou te mandar explicitamente no chat

AMANHA
- Supabase (ensinar a dar acesso)
- Passo a passo da entrega (1º: fazer o item do dia com calma, começar e terminar)
    1. Ao sentar no computador, antes de começar qualquer coisa, atualize os fontes.
    2. Escolha um tema.
    3. Envie para a IA o manifesto do nosso sistema.
    4. Reflita com a IA. Conforme ela responder o plano de implementação, não é necessário ler o documento todo. 
    5. Veja atentamente quando ela disser algo "IMPORTANTE"; o restante pode ser ignorado.
    6. Reforce a sua ideia com o plano: "Olha, de acordo com seu plano, vai acontecer isso, isso e isso, certo?" 
    7. Aprove o plano (ou ajuste antes de executar): "Ok, pode fazer" ou "Muda X antes de começar"
    8. Com o ajuste feito: reforçar com ela que, se precisar, pode criar componente ou "form" base, caso enxergue necessidade no resultado da alteração.
    9. Questione se houve SQL novo e se ela rodou no localhost e no Supabase (como ela deve "saber" fazer isso já está descrito no manifesto).
    10. Teste manualmente para ver se o sistema não quebra (crash).
    11. Peça para melhorar/confirmar se está no padrão visual e na identidade do código.
    12. Confirme ou peça para fazer a Unit de Teste.
    13. Revise se o manifesto foi seguido.
    14. Verifique se a alteração deixou sujeira, arquivos python .pyc ou .pyo, lixo de formatação no código, etc. Se sim, peça para limpar.
    15. Verifique se a alteração deixou arquivos sql fora do Scripts/banco_script_completo.sql... peça para excluir os SQL soltos que foi rodado no localhost e supabase, unificar no completo.
    16. Revise se "REVISAR SEMPRE" foi seguido. Inclusive, se a IA deixar de fazer alguma coisa, não seguir algum padrão, voce adiciona nesse REVISAO SEMPRE
    17. Atualize os fontes.
	
Revisão sempre que possível
- Código em português, só deixar inglês se for exclusivo da arquitetura ou reservado (se a tela que estivermos mexendo tiver algo em ingles, já podemos ajustar)
- Sem comentários no código. O máximo é de "como os dados deveriam estar numa variavel" se for um tratamento muito especifico. Ex.:
- Codigo .css e .js devem ficar em seus arquivos separados e de mesmo nome da pagina no mesmo diretório
Compromissos
- Não usar hardcoded
- Usar ModuloRotas
- Fontes: Plus Jakarta Sans
	<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=Instrument+Serif:ital@0;1&display=swap" rel="stylesheet">
- Se a tela faz uma "consultar" sempre carregar em lotes ou paginação para não sobrecarregar.
- Componetizar como TagHelpers, qualquer campo novo. (e usar o que ja existe)
- Se for cadastro normal, crud... vamos usar entity framework pra poder capturar facil qual campo foi modificado. (deletar command que parar de usar)

AJUSTES (DO QUE JA FOI IMPLEMENTADO)
- Talvez no banco de dados devêssemos ter nossos próprios tipos. Ex.: dolar MOEDA.
- "Cliente": definir se vai buscar cadastro interno (pessoa física/jurídica) ou outro usuário.
- [x] Ao criar um usuário, criar automaticamente uma empresa LTDA com o mesmo nome e CNPJ 000. Assim, ele terá acesso fácil às rotinas de ERP sem precisar de uma empresa real.
- KingHost (criar script com a IA baseado no script antigo para atualizar o projeto, ou adotar algo como Docker).
- 
- Consulta/Listagem
    - Adicionar botão de exportar em vários formatos, para legalmente não retermos a base do cliente (eu garanto a permissão de quem pode exportar).
- Botão SELECIONAR
    - Em rotinas onde é preciso selecionar itens ou registros no GRID, precisamos padronizar via componente, evitando replicação e garantindo que toda tela faça do mesmo jeito.
- [x] Criar o "loading" padrão que seja facil mudar depois, ja implementar nas chamadas das telas atuais
- Rotina de logar com e-mail (falta o google service)
- Rotina de cloudflare R3 (ja implementado, configurar no appsetiings chave)
- Testar a recuperação de senha
- No login, Ao inves de dar erro de credenciais inválidas, sem ter que usuario ou senha inválidos.
- Quando clicar em detalhes do cliente e tivermos dados das vendas, aparecer lá.
- Proibição técnica de descontos de 100%; toda transação deve gerar um valor líquido final maior que zero. (Permitir inclusão de produto de 1 centavo.)
- Cuidado com a matematica do percentual com 4 casas e o desconto em valor em 2 casas

NÍVEL 1
- LinkedIn
    - Cadastro de Currículo na "Área do Cliente".
    - Cadastro de Vagas vinculado a uma empresa.
    - Opção de se candidatar a uma vaga.
    - Não exibir dados do usuário para outro sem consentimento (LGPD). Ex.: no currículo, o usuário autoriza a empresa a ver seu e-mail, telefone e endereço apenas ao se candidatar.
- Criador de Código de Barras Online
- Criador de Gerador de Etiqueta Correios
- CRM
    - (Perguntar para a IA o que é e para que serve; perguntar para a New Century como funciona o CRM deles hoje, até onde vai e o que falta. Provavelmente será algo simples: criar um novo "papel de usuário" chamado Prospect, com uma tela única que reaproveita o cadastro de pessoa por trás, mas focada apenas nos dados essenciais de prospecção, sem as abas de cliente/vendedor. Adicionar campos importantes na aba Prospect. Essencial ter um histórico de contato, para registrar tentativas, agendar próximos contatos, etc.)

NÍVEL 2
- Cashback
- Fidelidade
- Documento de aceite (REIMPLEMENTAR, JÁ TEM NO APP)
- Agendamento
- Produto (REIMPLEMENTAR, JÁ TEM NO APP): lidar com variações e preços. Deve ser por empresa, mas acessível pela conta, igual aos outros cadastros.
    Quando informar o NCM e ele não existir, já configurar regra fiscal/situação tributária
    Tabela de situação do produto, onde lá a gente vincula (aparece PDV, aparece PEDIDO, aparece Y, apareceu X, faz aquilo)

- Observação Fiscal (REIMPLEMENTAR, JÁ TEM NO APP):
    - Validar IPI
    - Validar ICMS
    - Validar PIS
    - Validar COFINS
    - Validar IBS
    - Validar CBS
- Ordem de Serviço (nova, mas analisar o que já existe no Finac).
- Achados e Perdidos (na área da cidade).
- Incluir tunel do Cloudflare
- Subdomínio (cliente.andreense.com.br)
    - Criar para areacliente.andreense e servicos.andreense, utilizando logins diferentes.
- Cotação
- Orçamento
- Pedido (REIMPLEMENTAR, JÁ TEM NO APP): permitir desmembrar o pedido em dois (faturar apenas metade, respeitando os mesmos impostos originais).
    Testar fluxo casado: Exclusão de Itens: Antes de excluir um item do pedido, é obrigatório excluir a forma de recebimento vinculada. Caso contrário, ocorrerá erro de validação ao tentar salvar a alteração.
- Embarque (REIMPLEMENTAR, JÁ TEM NO APP).
- Separação/Conferência (REIMPLEMENTAR, JÁ TEM NO APP): não permitir parcial (se precisar de parcial, deve desmembrar o pedido na etapa anterior).
- Relatórios (
        1 relatório de texto livre pos pedido.

NÍVEL 3
- Envio de Nota Fiscal
- PDV
- TEF
- Delivery (REIMPLEMENTAR, JÁ TEM NO APP).
- Vínculo/Convite/Revogação: permitir que um usuário/e-mail faça parte de outra empresa.
- Criar uma tabela “perfil saas” para a gente “rankiar” os usuários e prever quem é o usuário comum, curioso, possível hacker de acordo com a sequência de ações no sistema

REVISAR SEMPRE (toda entrega)
- Log eficiente
- God Classes / rotinas grandes em tela
- Nomenclatura pela metodologia
- Testes: sim/não, repetição 2x, lote (1, N, N-1)
- Queries Dapper filtram @contaId
- Herança correta (AppBasePageModel)
- Sem HttpContext na camada Aplicação
- Mensagens em PT-BR
- Mensagens Humanizadas → Não utilizar termos técnicos (como "O campo X") nas validações. Falar de forma clara com o usuário (ex: "Informe a senha").
- LGPD → se a feature envolve dados de terceiros
- Modularidade → se tem integração externa
- SEM INFORMAÇÃO → se criou/alterou tabela
- **AJAX para Submissões Focadas** → Em submissões em blocos específicos de páginas (ex: gerenciar itens em cards), utilize AJAX (`fetch`) para atualizar o HTML cirurgicamente, evitando recarregamentos que redefinem a posição do scroll (efeito "flicker" ou pulos ao topo da tela).
- **Mensagens Padronizadas em Modal** → Utilize o componente global `window.showModal(title, message, type)` para exibir alertas e feedbacks do sistema, garantindo um visual Glassmorphic premium consistente por toda a aplicação.
- **TagHelpers em Grids (Campos Sozinhos)** → O atributo `style` (especialmente `style="grid-column: 1 / -1;"`) passado para `<app-input>`, `<app-select>` e `<app-textarea>` deve ser renderizado no elemento wrapper (`<div class="form-group">`) e não ser ignorado/apagado. Se ignorado, o elemento ocupará apenas 1 coluna no `form-grid`, causando o deslocamento dos itens subsequentes e fazendo com que campos fiquem "sozinhos" ou mal alinhados na tela (ex: campo Data e Genero).

SEGURANÇA
- Inexistência de politica de privacidade
- OAuth Google (federada) precisa ter finalidade, prazos legais e prazo de retenção ou destinatário dos dados
- Carregamento de cookies analíticos sem consentimento prévio (garantir rodar o script javascript apenas quando ela aceitar) Não foi identificado o o mecanismo de bloqueio prévio, outpin mecanismos legal
- armazenamento de endereço de IP sem anomilização, em registros de clicks sem aplicação da hash, anomilização ou prazo de retenção definido, o endereço IP com dados de outras informações é dado pessoal.
- Se usar algo como SENTRY, que pega os headers, precisamos avisar que "terceiros" vão ter tal informação 
- endpoint de exportação de dados
- opção de deletar conta
- canal de contato de DPO
- cookie de locate com ano de duração sem aviso
