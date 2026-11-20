namespace ModuloMonolitico.Dominio.Configuracoes
{
    public static class MensagensSistema
    {
        public static class Geral
        {
            public const string Sucesso = "Concluído";
            public const string Cancelar = "Cancelar";
            public const string ErroDesconhecido = "Ocorreu um erro desconhecido ao processar sua solicitação.";
            public const string RegistroNaoEncontrado = "O registro não foi encontrado.";
        }

        public static class Validacao
        {
            public const string CampoObrigatorio = "O campo é obrigatório.";
            public const string TamanhoMaximoExcedido = "O tamanho máximo de caracteres foi excedido.";
            public const string NomeObrigatorio = "O nome é obrigatório.";
            public const string SiglaObrigatoria = "A sigla é obrigatória.";
            public const string RangeInvalido = "O valor deve estar entre {1} e {2}.";
            public const string OrdemInvalida = "A ordem deve estar entre {1} e {2}.";
        }

        public static class Produto
        {
            public const string ErroSalvar = "Erro ao salvar o produto.";
            public const string ErroDeletar = "Erro ao deletar o produto.";
            public const string VerifiqueDados = "Verifique os dados informados.";
            public const string IdNaoFornecido = "ID do produto não fornecido para atualização.";
            public const string VariacoesPrecisamSKU = "Quando há mais de uma variação, todas precisam ter um SKU / Código preenchido.";
            public const string NaoEncontrado = "Produto não encontrado ou não pertence a esta conta.";
            public const string CategoriaCadastrada = "Categoria cadastrada com sucesso.";
            public const string MarcaCadastrada = "Marca cadastrada com sucesso.";
        }

        public static class Precos
        {
            public const string ErroSalvar = "Erro ao salvar contexto de preço.";
            public const string ErroDeletar = "Erro ao deletar contexto de preço.";
            public const string NaoEncontrado = "Contexto de preço não encontrado.";
            public const string VerifiqueDados = "Verifique os dados informados.";
        }

        public static class Gestao
        {
            public static class TiposVendedor
            {
                public const string NomeObrigatorio = "O nome da categoria é obrigatório.";
                public const string NomeTamanhoExcedido = "O nome não pode exceder 200 caracteres.";
                public const string DescricaoTamanhoExcedido = "A descrição não pode exceder 500 caracteres.";
                public const string NaoEncontrado = "Tipo de vendedor não encontrado no sistema.";
            }

            public static class Vendedores
            {
                public const string NenhumSelecionado = "Nenhum vendedor selecionado.";
                public const string NaoEncontrado = "Vendedor não encontrado.";
            }
        }
        public static class Contatos
        {
            public static class Vinculos
            {
                public const string ErroSolicitacaoPropria = "Você não pode enviar uma solicitação para si mesmo.";
                public const string ErroEnviarSolicitacao = "Erro ao enviar solicitação.";
            }

            public static class Cadastro
            {
                public const string ErroDeletar = "Erro ao deletar contato.";
                public const string NenhumSelecionadoExclusao = "Nenhum contato selecionado para exclusão.";
                public const string ErroDeletarSelecionados = "Erro ao deletar contatos selecionados.";
                public const string NaoEncontrado = "Contato não encontrado.";
            }
        }

        public static class Empresas
        {
            public const string NaoEncontrada = "Empresa não encontrada.";
            public const string NenhumaSelecionada = "Nenhuma empresa selecionada.";
            public const string ErroSelecionar = "Ocorreu um erro interno ao tentar selecionar a empresa.";
            public const string TextoConvite = "Você foi convidado para participar da empresa {0}. Acesse este link: [Clique aqui para responder ao convite](/Convite/ResponderConviteEquipe?empresaId={1}).";
        }

        public static class Estoque
        {
            public const string ErroRegistrarEntrada = "Erro ao registrar entrada de estoque.";
            public const string QuantidadeEntradaInvalida = "A quantidade de entrada deve ser maior que zero.";
            public const string ErroRegistrarSaida = "Erro ao registrar saída de estoque.";
            public const string QuantidadeSaidaInvalida = "A quantidade de saída deve ser maior que zero.";
            public const string ConfiguracaoNaoEncontrada = "Configuração não encontrada.";
            public const string SaldoNaoEncontrado = "Saldo de estoque não encontrado.";
            public const string UnidadeJaCadastrada = "Esta Unidade de Medida já está cadastrada.";
            public const string UnidadeCadastrada = "Unidade de medida cadastrada com sucesso.";
            public const string SaldoInsuficiente = "Saldo insuficiente. Disponível: {0:N3}, Solicitado: {1:N3}";
        }

        public static class Fidelizacao
        {
            public static class Fidelidade
            {
                public const string SemEmpresaVinculada = "Você precisa estar vinculado a uma empresa para gerenciar cartões de fidelidade.";
                public const string CartaoNaoEncontrado = "Cartão fidelidade não encontrado.";
                public const string ErroGerarToken = "Erro ao gerar token: {0}";
                public const string TokenInvalido = "Token inválido.";
                public const string QrCodeUtilizado = "Este QR Code já foi utilizado.";
                public const string QrCodeExpirou = "Este QR Code expirou.";
                public const string ErroConsumirToken = "Erro ao consumir token: {0}";
                public const string CartaoNaoEncontradoOuInativo = "Cartão fidelidade não encontrado ou inativo.";
                public const string ExigeCodigoReferencia = "Este cartão exige um código de referência (ex: número da venda).";
                public const string ErroRegistrarCarimbo = "Erro ao registrar carimbo: {0}";
                public const string NenhumaRecompensa = "Nenhuma recompensa disponível para resgate.";
                public const string ErroResgatar = "Erro ao resgatar recompensa: {0}";
            }

            public static class Cashback
            {
                public const string SemEmpresaVinculada = "Você precisa estar vinculado a uma empresa para gerenciar cashback.";
                public const string NenhumaCampanhaAtiva = "Nenhuma campanha de cashback ativa.";
                public const string RegraNaoEncontrada = "Regra de cashback não encontrada.";
                public const string ErroGerarToken = "Erro ao gerar token: {0}";
                public const string TokenInvalido = "Token inválido.";
                public const string QrCodeUtilizado = "Este QR Code já foi utilizado.";
                public const string QrCodeExpirou = "Este QR Code expirou.";
                public const string ValorNaoInformado = "Valor da compra não informado.";
                public const string DebitoNaoImplementado = "Operação de débito via QR Code ainda não implementada.";
                public const string TipoTokenDesconhecido = "Tipo de token desconhecido.";
                public const string ErroConsumirToken = "Erro ao consumir token: {0}";
                public const string NenhumaRegraAtiva = "Nenhuma regra de cashback ativa encontrada para esta empresa.";
                public const string ExigeCodigoReferencia = "Esta regra exige um código de referência (ex: número da nota/venda).";
                public const string NaoGeraMinimo = "O valor da compra não gera cashback mínimo.";
                public const string DescricaoPadrao = "Cashback de {0}% sobre compra de R$ {1}";
                public const string ErroCreditar = "Erro ao creditar cashback: {0}";
                public const string SaldoInsuficiente = "Saldo de cashback insuficiente.";
                public const string ErroConsistenciaSaldo = "Erro na consistência de saldo. Tente novamente.";
                public const string ErroDebitar = "Erro ao debitar cashback: {0}";
            }
        }

        public static class InfraAdmin
        {
            public const string ErroCarregarPagina = "Ocorreu um erro interno ao carregar a página.";
            public const string ErroProcessarRequisicao = "Ocorreu um erro interno ao processar a requisição.";
            public const string ErroCarregarLogs = "Ocorreu um erro ao carregar os logs do sistema.";

            public static class Autenticacao
            {
                public const string ErroHistoricoLogin = "Ocorreu um erro interno ao tentar carregar o histórico de logins.";
                public const string ErroListaConectados = "Ocorreu um erro interno ao obter a lista de últimos conectados.";
                public const string ErroRevogarAcesso = "Ocorreu um erro interno ao tentar revogar o acesso do usuário.";
            }

            public static class Documentos
            {
                public const string ErroCriar = "Erro ao criar documento. Verifique se a versão para este tipo já existe.";
                public const string ErroCarregarLista = "Erro ao carregar a lista de documentos.";
                public const string ErroCarregar = "Erro ao carregar o documento legal.";
                public const string NaoEncontrado = "Documento não encontrado.";
                public const string ErroAtualizar = "Erro ao atualizar documento.";
            }

            public static class Grupos
            {
                public const string NaoEncontrado = "Grupo não encontrado.";
                public const string ErroCarregarLista = "Erro ao carregar a lista de grupos.";
                public const string ErroCarregarDados = "Erro ao carregar dados do grupo.";
                public const string ErroProcessarBusca = "Erro ao processar busca.";
            }

            public static class SaaS
            {
                public static class Comunicados
                {
                    public const string ErroCarregarLista = "Erro ao carregar comunicados.";
                    public const string ErroCarregarDetalhes = "Erro ao carregar detalhes.";
                    public const string NaoEncontrado = "Comunicado não encontrado.";
                    public const string ErroExcluir = "Erro ao excluir comunicado.";
                    public const string ErroCadastrar = "Erro ao cadastrar comunicado.";
                    public const string TituloMensagemObrigatorios = "Título e Mensagem são obrigatórios.";
                    public const string EnqueteMinimoOpcoes = "Uma enquete precisa de pelo menos 2 opções de resposta válidas.";
                }

                public static class Planos
                {
                    public const string ErroCarregarLista = "Erro interno ao carregar a listagem de planos. Tente novamente mais tarde.";
                    public const string ValorMensalInvalido = "Informe um valor mensal válido (ex: 99.90).";
                    public const string NaoEncontrado = "Plano não encontrado.";
                }

                public static class Faturas
                {
                    public const string ErroDarBaixa = "Erro ao dar baixa na fatura.";
                    public const string ErroGerar = "Erro ao gerar fatura.";
                }

                public static class Contas
                {
                    public const string AssinaturaNaoEncontrada = "Assinatura não encontrada.";
                    public const string ErroAtualizarSituacao = "Erro ao atualizar status da conta.";
                    public const string ErroAtribuirPlano = "Erro ao atribuir plano à conta.";
                }
            }
        }

        public static class Autenticacao
        {
            public const string FalhaEnvioCodigo = "Não foi possível enviar o código. Tente reenviar ou volte e solicite novamente.";
            public const string InformeEmail = "Por favor, informe seu e-mail.";
            public const string SessaoExpirada = "Sua sessão expirou. Faça login novamente.";
            public const string SessaoEncerradaAdmin = "Sua sessão foi encerrada por um administrador.";
            public const string ErroLoginPosCadastro = "Conta criada, mas ocorreu um erro no login automático. Faça login manualmente.";
            public const string UsuarioNaoEncontrado = "Usuário não encontrado.";
            public const string SenhasNaoCoincidem = "As senhas não coincidem.";
            public const string EmailJaCadastrado = "Este e-mail já está cadastrado. Faça login ou recupere sua senha.";
            public const string EmailECodigoObrigatorios = "E-mail e código de verificação são obrigatórios.";
            public const string EmailNaoFornecido = "E-mail não fornecido.";
            public const string ContaInativa = "Esta conta está inativa. Entre em contato com o suporte.";
            public const string ProblemaMomento = "Estamos com um problema no momento. Tente novamente mais tarde.";
            public const string ErroRecriarVinculos = "Erro ao tentar recriar vínculos com EF Core: {0}";

            public static class Grupos
            {
                public const string ErroSalvar = "Erro ao salvar grupo.";
                public const string ErroCarregarLista = "Erro ao carregar a lista de grupos.";
                public const string ErroExcluir = "Erro ao excluir grupo.";
                public const string NaoEncontrado = "Grupo não encontrado.";
                public const string ErroCarregar = "Erro ao carregar grupo.";
                public const string ErroSalvarConfiguracoes = "Erro ao salvar configurações.";
                public const string NaoPodeInativarComUsuarios = "Não é possível inativar este grupo, pois existem {0} usuário(s) ativo(s) vinculados a ele.";
                public const string NaoPodeExcluirPrincipal = "Não é possível excluir o grupo de Administradores principal do sistema.";
                public const string NaoPodeExcluirComUsuarios = "Não é possível excluir este grupo, pois existem {0} usuário(s) ativo(s) vinculados a ele.";
            }
        }

        public static class Perfil
        {
            public const string ErroDesativar = "Erro ao desativar conta.";
            public const string ErroAtualizar = "Erro ao atualizar perfil.";
            public const string SenhaConfirmacaoObrigatoria = "A senha de confirmação é obrigatória.";
            public const string SenhaConfirmacaoIncorreta = "Senha de confirmação incorreta.";
            public const string ProfissaoVazia = "O nome da profissão não pode ser vazio.";
            public const string ArquivoInvalido = "Selecione um arquivo válido.";
        }

        public static class Curriculos
        {
            public const string ErroAlterarVisibilidade = "Não foi possível alterar a visibilidade do currículo.";
            public const string ErroAtualizarCurso = "Não foi possível atualizar o curso.";
            public const string ErroAtualizarExperiencia = "Não foi possível atualizar a experiência.";
            public const string ErroAtualizarFormacao = "Não foi possível atualizar a formação.";
            public const string ErroCadastrarCurso = "Não foi possível cadastrar o curso.";
            public const string ErroCadastrarExperiencia = "Não foi possível cadastrar a experiência.";
            public const string ErroCadastrarFormacao = "Não foi possível cadastrar a formação.";
            public const string ErroDeletarCurso = "Não foi possível remover o curso.";
            public const string ErroDeletarExperiencia = "Não foi possível remover a experiência.";
            public const string ErroDeletarFormacao = "Não foi possível remover a formação.";
            public const string ErroSalvarCurriculo = "Não foi possível salvar o currículo. Tente novamente.";
            public const string ErroSalvarHabilidades = "Não foi possível salvar as habilidades.";
            public const string ErroSalvarIdiomas = "Não foi possível salvar os idiomas.";
        }

        public static class Base
        {
            public static class Equipe
            {
                public const string ErroCarregar = "Erro ao carregar a equipe.";
                public const string EmailObrigatorio = "O e-mail é obrigatório.";
                public const string ErroEnviarConvite = "Erro ao enviar convite.";
                public const string ErroRemoverSiMesmo = "Você não pode remover a si mesmo da equipe. Caso queira sair, desvincule-se através do seu perfil.";
                public const string ErroRemover = "Erro ao remover membro.";
                public const string ErroAtualizar = "Erro ao atualizar perfil do membro.";
                public const string ErroReenviarConvite = "Erro ao reenviar convite.";
            }
        }

        public static class Agendamentos
        {
            public const string SessaoExpirada = "Sessão expirada. Por favor, recomece o agendamento.";
            public const string DataHoraInvalida = "Data/hora inválida.";
            public const string ErroAlterarSituacao = "Não foi possível alterar o status.";
            public const string ServicoNaoEncontrado = "Serviço não encontrado.";
            public const string ErroCriarAgendamento = "Erro ao criar agendamento: {0}";
            public const string ErroConfirmarAgendamento = "Erro ao confirmar: {0}";
            public const string ApenasNome = "Apenas {0}";
            public const string AgendamentoNaoEncontrado = "Agendamento não encontrado.";
            public const string AgendamentoJaCancelado = "Este agendamento já foi cancelado.";
            public const string AgendamentoJaConcluido = "Não é possível cancelar um agendamento já concluído.";
            public const string CancelamentoAntecedenciaMinima = "Cancelamentos pelo cliente devem ser feitos com pelo menos {0}h de antecedência. Entre em contato diretamente com a empresa.";
            public const string AgendamentoCanceladoComSucesso = "Agendamento cancelado com sucesso.";
            public const string ConflitoHorario = "Já existe um agendamento para este profissional neste horário.";

            public const string AssuntoEmailConfirmado = "✅ Agendamento confirmado — Andreense";
            public const string CorpoEmailConfirmado = """
                <div style="font-family:sans-serif;max-width:520px;margin:0 auto;padding:24px;">
                  <h2 style="color:#6366f1;">Agendamento realizado!</h2>
                  <p>Olá, <strong>{0}</strong>!</p>
                  <p>Seu agendamento foi registrado com sucesso para <strong>{1}</strong>.</p>
                  <p style="color:#6b7280;font-size:0.9rem;">Caso precise cancelar ou reagendar, entre em contato com a empresa com antecedência mínima de 2 horas.</p>
                  <hr style="border-color:#e5e7eb;"/>
                  <p style="font-size:0.8rem;color:#9ca3af;">Andreense ERP — este é um e-mail automático, não responda.</p>
                </div>
                """;

            public const string AssuntoEmailSituacaoAtualizada = "Seu agendamento foi confirmado — Andreense";
            public const string CorpoEmailSituacaoAtualizada = """
                <div style="font-family:sans-serif;max-width:520px;margin:0 auto;padding:24px;">
                  <h2 style="color:#10b981;">Agendamento confirmado!</h2>
                  <p>Olá, <strong>{0}</strong>!</p>
                  <p>Seu agendamento para <strong>{1}</strong> foi <strong>confirmado</strong> pela empresa.</p>
                  <p>Serviço: <strong>{2}</strong></p>
                  <p>Profissional: <strong>{3}</strong></p>
                  <hr style="border-color:#e5e7eb;"/>
                  <p style="font-size:0.8rem;color:#9ca3af;">Andreense ERP — este é um e-mail automático, não responda.</p>
                </div>
                """;

            public const string AssuntoEmailCancelado = "Agendamento cancelado — Andreense";
            public const string CorpoEmailCancelado = """
                <div style="font-family:sans-serif;max-width:520px;margin:0 auto;padding:24px;">
                  <h2 style="color:#ef4444;">Agendamento cancelado</h2>
                  <p>Olá, <strong>{0}</strong>!</p>
                  <p>Seu agendamento de <strong>{1}</strong> ({2}) foi <strong>cancelado</strong>.</p>
                  {3}
                  <p>Para reagendar, acesse a Área do Cliente.</p>
                  <hr style="border-color:#e5e7eb;"/>
                  <p style="font-size:0.8rem;color:#9ca3af;">Andreense ERP — este é um e-mail automático, não responda.</p>
                </div>
                """;
        }

        public static class Imobiliaria
        {
            public const string AnuncioNaoEncontrado = "Anúncio não encontrado.";
            public const string ImovelNaoEncontrado = "Imóvel não encontrado.";
            public const string ErroCadastrarAnuncio = "Erro ao publicar anúncio.";
            public const string ErroAtualizarAnuncio = "Erro ao atualizar anúncio.";
            public const string ErroAtualizarSituacao = "Erro ao alterar status do anúncio.";
            public const string FotoInvalida = "Selecione um arquivo de imagem válido (JPG, PNG, WEBP).";
            public const string LimiteAFotos = "O limite é de 15 fotos por anúncio.";
            public const string AnuncioSemPermissao = "Você não tem permissão para editar este anúncio.";
            public const string ErroUploadFoto = "Erro ao fazer upload da foto. Tente novamente.";
            public const string CepObrigatorio = "O CEP é obrigatório.";
            public const string LogradouroObrigatorio = "O logradouro é obrigatório.";
            public const string NumeroObrigatorio = "O número é obrigatório.";
            public const string BairroObrigatorio = "O bairro é obrigatório.";
            public const string BuscaRecenteNaoEncontrada = "Busca recente não encontrada.";
            public const string BuscaRecenteRemovida = "Busca recente removida.";
            public const string BuscasRecentesLimpas = "Buscas recentes removidas.";
            public const string TituloBuscasRecentes = "Buscas recentes";
            public const string GerenciarBuscasRecentes = "+todas";
            public const string NenhumaBuscaRecente = "Nenhuma busca recente salva.";
            public const string LimparTodasBuscasRecentes = "Limpar todas";
            public const string ExcluirBuscaRecente = "Excluir";
            public const string ConfirmarLimparBuscasRecentes = "Remover todas as buscas recentes?";
            public const string VoltarAosImoveis = "Voltar aos imóveis";
            public const string VerifiqueDados = "Verifique os campos informados.";
            public const string ErroFavoritarImovel = "Não foi possível atualizar o favorito.";
            public const string DestaqueSomenteAtivo = "Somente anúncios ativos podem ser marcados como destaque.";
            public const string DestaqueAtivado = "Anúncio marcado como destaque.";
            public const string DestaqueDesativado = "Destaque removido do anúncio.";
            public const string SeloDestaque = "Destaque";
            public const string SeloVisualizado = "Visto";
            public const string AvisoJaVisualizado = "Você já visualizou este imóvel.";
            public const string ErroRegistrarVisualizacao = "Não foi possível registrar a visualização.";
            public const string ErroAlternarDestaque = "Não foi possível alterar o destaque.";
            public const string DestacarAnuncio = "Marcar como destaque";
            public const string RemoverDestaqueAnuncio = "Remover destaque";
            public const string BuscaAtual = "Busca atual";
            public const string TituloMeusFavoritos = "Meus favoritos";
            public const string NenhumFavorito = "Você ainda não favoritou nenhum imóvel.";
            public const string LoginParaFavoritos = "Faça login para ver seus favoritos.";
            public const string ExplorarImoveis = "Explorar imóveis";
            public const string TituloPerfilAnunciante = "Perfil do Anunciante";
            public const string SubtituloPerfilAnunciante = "Personalize como você e seus imóveis aparecem para os interessados.";
            public const string VoltarMeusImoveis = "Meus Imóveis";
            public const string SecaoPaginaPublica = "Página pública";
            public const string SecaoContatosRedes = "Contatos e redes sociais";
            public const string LabelNomeExibicao = "Nome de exibição da imobiliária / corretor";
            public const string PlaceholderNomeExibicao = "Ex: Imobiliária Silva ou João Corretor";
            public const string LabelCreci = "Registro CRECI (opcional)";
            public const string PlaceholderCreci = "Ex: 12345-F";
            public const string LabelDescricaoPerfil = "Descrição / biografia";
            public const string PlaceholderDescricaoPerfil = "Fale um pouco sobre você, sua empresa e áreas de atuação.";
            public const string LabelUrlFotoLogo = "URL da foto / logo";
            public const string PlaceholderUrlFotoLogo = "https://...";
            public const string AjudaUrlFotoLogo = "Cole o link de uma imagem hospedada. Em breve teremos upload nativo.";
            public const string LabelWhatsapp = "WhatsApp";
            public const string PlaceholderWhatsapp = "(11) 90000-0000";
            public const string LabelInstagram = "Instagram";
            public const string PlaceholderInstagram = "@@imobiliaria";
            public const string LabelSite = "Site";
            public const string PlaceholderSite = "www.imobiliaria.com.br";
            public const string BotaoSalvarPerfil = "Salvar perfil";
        }

        public static class Veiculos
        {
            public const string AnuncioNaoEncontrado = "Anúncio de veículo não encontrado.";
            public const string AnuncioSemPermissao = "Você não tem permissão para alterar este anúncio.";
            public const string DestaqueSomenteAtivo = "Somente anúncios ativos podem ser marcados como destaque.";
            public const string DestaqueAtivado = "Anúncio marcado como destaque.";
            public const string DestaqueDesativado = "Destaque removido do anúncio.";
            public const string LimiteFotosAtingido = "Limite de 20 fotos por anúncio atingido.";
            public const string ErroFavoritarVeiculo = "Não foi possível atualizar o favorito.";
            public const string SeloDestaque = "Destaque";
            public const string VoltarAosVeiculos = "Voltar aos veículos";
            public const string ExplorarVeiculos = "Explorar veículos";
            public const string VoltarMeusVeiculos = "Meus Veículos";
            public const string NenhumFavorito = "Você ainda não favoritou nenhum veículo.";
            public const string LoginParaFavoritos = "Faça login para ver seus favoritos.";
            public const string TituloMeusFavoritos = "Meus favoritos";
            public const string TituloPerfilVendedor = "Perfil do Vendedor";
            public const string SubtituloPerfilVendedor = "Personalize como você e seus veículos aparecem para os interessados.";
            public const string NenhumaBuscaRecente = "Nenhuma busca recente salva.";
            public const string LabelNomeExibicao = "Nome de exibição da loja / vendedor";
            public const string PlaceholderNomeExibicao = "Ex: Loja de Veículos Silva ou João Vendedor";
            public const string LabelDescricaoPerfil = "Descrição / biografia";
            public const string PlaceholderDescricaoPerfil = "Fale um pouco sobre você, sua loja e especialidades.";
            public const string LabelUrlFotoLogo = "URL da foto / logo";
            public const string PlaceholderUrlFotoLogo = "https://...";
            public const string LabelWhatsapp = "WhatsApp";
            public const string PlaceholderWhatsapp = "(11) 90000-0000";
            public const string LabelInstagram = "Instagram";
            public const string PlaceholderInstagram = "@loja";
            public const string LabelSite = "Site";
            public const string PlaceholderSite = "www.lojaveiculo.com.br";
            public const string BotaoSalvarPerfil = "Salvar perfil";
        }

        public static class Cidadania
        {
            public const string ErroGerarCodigoBarras = "Erro inesperado ao gerar código de barras: {0}";
            public const string PublicacaoRealizadaComSucesso = "Publicação realizada com sucesso!";
            public const string TipoObrigatorio = "O tipo de publicação é obrigatório.";
            public const string TipoInvalido = "O tipo de publicação deve ser 'achado' ou 'perdido'.";
            public const string TituloObrigatorio = "O título do item é obrigatório.";
            public const string TituloTamanhoMaximo = "O título pode ter no máximo 100 caracteres.";
            public const string DescricaoTamanhoMaximo = "A descrição pode ter no máximo 1000 caracteres.";
            public const string ContatoTamanhoMaximo = "A informação de contato pode ter no máximo 200 caracteres.";
            public const string LocalVistoTamanhoMaximo = "O local visto pode ter no máximo 200 caracteres.";
        }
        
        public static class ERP
        {
            public const string PostEfetuado = "Post efetuado! ClienteId: {0}, FornecedorId: {1}";
        }
        
        public static class Fiscal
        {
            public const string EdicaoSerieFiscal = "Edição de série fiscal ID {0}";
            public const string CriacaoSerieFiscal = "Criação de série fiscal Modelo {0} / Série {1}";
            public const string ErroSalvarSerie = "Erro ao salvar série: {0}";
            public const string ErroRemoverSerie = "Erro ao remover série: {0}";
            public const string VerifiqueDados = "Verifique os dados informados.";
            public const string ConfiguracaoNaoEncontrada = "Configuração fiscal não encontrada.";
            public const string SerieCadastrada = "Série fiscal cadastrada com sucesso.";
            public const string SerieAtualizada = "Série fiscal atualizada com sucesso.";
            public const string SerieRemovida = "Série fiscal removida com sucesso.";
            public const string ErroSalvar = "Erro ao salvar configuração de emissão.";
            public const string ErroDeletar = "Erro ao deletar configuração de emissão.";
            public const string AmbienteInvalido = "O ambiente deve ser Homologação ou Produção.";
            public const string SerieInvalida = "A série deve estar entre {1} e {2}.";
            public const string NumeroNfInvalido = "O próximo número da NF deve ser maior que zero.";
            public const string TipoEmissaoInvalido = "Informe um tipo de emissão válido.";
            public const string TipoImpressaoInvalido = "Informe um tipo de impressão válido.";
        }

        public static class Diagnostico
        {
            public const string TituloPainel = "Diagnóstico de desempenho";
            public const string AjudaPainel = "Ative com ?diagnostico=1 em qualquer URL. Em Development o painel aparece automaticamente.";
            public const string ColunaEtapa = "Etapa";
            public const string ColunaDuracao = "Duração";
            public const string ColunaBarra = "Proporção";
        }
    }
}
