using Dapper;
using ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Base;
using ModuloMonolitico.Dominio.Entidades;
using ModuloMonolitico.Dominio.InterfacesDeRepositorio;
using ModuloMonolitico.Dominio.Compartilhado;

namespace ModuloMonolitico.Infraestrutura.AcessoADados.Repositorios.Contatos;

public class ContatoRepositorio : RepositorioBase, IContatoRepositorio
{
    public ContatoRepositorio(string connectionString) : base(connectionString)
    {
    }

    public async Task CadastrarContatoComPapeisAsync(Contato pessoaBase, IEnumerable<IContatoPapel> papeis)
    {
        var papeisList = papeis.ToList();
        var ehCliente = papeisList.Any(p => p is EmpresaCliente);
        var ehFornecedor = papeisList.Any(p => p is EmpresaFornecedor);
        var ehTransportador = papeisList.Any(p => p is EmpresaTransportador);
        var ehLead = papeisList.Any(p => p is EmpresaLead);
        var ehProspect = papeisList.Any(p => p is EmpresaProspect);
        var ehFuncionario = papeisList.Any(p => p is EmpresaFuncionario);

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            if (pessoaBase.TipoPessoa == "fisica")
            {
                const string sqlPessoa = """
                    INSERT INTO contatos_pessoas_fisicas
                        (id, conta_id, nome, cpf, rg, data_nascimento, email, telefone,
                         is_cliente, is_fornecedor, is_transportador, is_lead, is_prospect, is_funcionario, vendedor_1_id, vendedor_2_id, status, envia_email_agendamento)
                    VALUES
                        (@Id, @ContaId, @Nome, @Cpf, @Rg, @DataNascimento, @Email, @Telefone,
                         @EhCliente, @EhFornecedor, @EhTransportador, @EhLead, @EhProspect, @EhFuncionario, @Vendedor1Id, @Vendedor2Id, true, @EnviaEmailAgendamento)
                    """;

                await conexao.ExecuteAsync(sqlPessoa, new
                {
                    Id = pessoaBase.Id,
                    ContaId = pessoaBase.ContaId,
                    Nome = pessoaBase.NomeCompleto ?? "",
                    Cpf = pessoaBase.Cpf,
                    Rg = pessoaBase.Rg,
                    DataNascimento = pessoaBase.DataNascimento,
                    Email = pessoaBase.Email,
                    Telefone = pessoaBase.Telefone,
                    EhCliente = ehCliente,
                    EhFornecedor = ehFornecedor,
                    EhTransportador = ehTransportador,
                    EhLead = ehLead,
                    EhProspect = ehProspect,
                    EhFuncionario = ehFuncionario,
                    Vendedor1Id = pessoaBase.Vendedor1Id,
                    Vendedor2Id = pessoaBase.Vendedor2Id,
                    EnviaEmailAgendamento = pessoaBase.EnviaEmailAgendamento
                });
            }
            else 
            {
                const string sqlPessoa = """
                    INSERT INTO contatos_pessoas_juridicas
                        (id, conta_id, razao_social, nome_fantasia, cnpj, inscricao_estadual,
                         email, telefone, is_cliente, is_fornecedor, is_transportador, is_lead, is_prospect, is_funcionario, vendedor_1_id, vendedor_2_id, status, envia_email_agendamento)
                    VALUES
                        (@Id, @ContaId, @RazaoSocial, @NomeFantasia, @Cnpj, @InscricaoEstadual,
                         @Email, @Telefone, @EhCliente, @EhFornecedor, @EhTransportador, @EhLead, @EhProspect, @EhFuncionario, @Vendedor1Id, @Vendedor2Id, true, @EnviaEmailAgendamento)
                    """;

                await conexao.ExecuteAsync(sqlPessoa, new
                {
                    Id = pessoaBase.Id,
                    ContaId = pessoaBase.ContaId,
                    RazaoSocial = pessoaBase.RazaoSocial ?? "",
                    NomeFantasia = pessoaBase.NomeFantasia,
                    Cnpj = pessoaBase.Cnpj,
                    InscricaoEstadual = pessoaBase.InscricaoEstadual,
                    Email = pessoaBase.Email,
                    Telefone = pessoaBase.Telefone,
                    EhCliente = ehCliente,
                    EhFornecedor = ehFornecedor,
                    EhTransportador = ehTransportador,
                    EhLead = ehLead,
                    EhProspect = ehProspect,
                    EhFuncionario = ehFuncionario,
                    Vendedor1Id = pessoaBase.Vendedor1Id,
                    Vendedor2Id = pessoaBase.Vendedor2Id,
                    EnviaEmailAgendamento = pessoaBase.EnviaEmailAgendamento
                });
            }


            if (pessoaBase.Enderecos != null && pessoaBase.Enderecos.Any())
            {
                const string sqlEndereco = """
                    INSERT INTO enderecos
                        (id, contato_id, conta_id, cep, logradouro, numero, complemento, bairro, cidade, uf)
                    VALUES
                        (@Id, @ContatoId, @ContaId, @Cep, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Uf)
                    """;
                
                foreach (var end in pessoaBase.Enderecos)
                {
                    await conexao.ExecuteAsync(sqlEndereco, new
                    {
                        Id = end.Id,
                        ContatoId = pessoaBase.Id,
                        ContaId = pessoaBase.ContaId,
                        Cep = end.Cep,
                        Logradouro = end.Logradouro,
                        Numero = end.Numero,
                        Complemento = end.Complemento,
                        Bairro = end.Bairro,
                        Cidade = end.Cidade,
                        Uf = end.Uf
                    });
                }
            }

            string tabelaContato = pessoaBase.TipoPessoa == "fisica" ? "contatos_pessoas_fisicas" : "contatos_pessoas_juridicas";
            string sqlAtualizarFKs = $"""
                UPDATE {tabelaContato}
                SET endereco_entrega_id = @EnderecoEntregaId,
                    endereco_faturamento_id = @EnderecoFaturamentoId,
                    endereco_fiscal_id = @EnderecoFiscalId,
                    endereco_cobranca_id = @EnderecoCobrancaId,
                    endereco_retirada_id = @EnderecoRetiradaId
                WHERE id = @Id AND conta_id = @ContaId
                """;
            
            await conexao.ExecuteAsync(sqlAtualizarFKs, new
            {
                Id = pessoaBase.Id,
                ContaId = pessoaBase.ContaId,
                EnderecoEntregaId = pessoaBase.EnderecoEntregaId,
                EnderecoFaturamentoId = pessoaBase.EnderecoFaturamentoId,
                EnderecoFiscalId = pessoaBase.EnderecoFiscalId,
                EnderecoCobrancaId = pessoaBase.EnderecoCobrancaId,
                EnderecoRetiradaId = pessoaBase.EnderecoRetiradaId
            });


            foreach (var papel in papeisList)
            {
                if (papel is EmpresaCliente c)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_clientes (empresa_id, contato_id, contato_tipo, limite_credito, desconto_padrao, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Limite, @Desconto, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Limite = c.LimiteCredito,
                        Desconto = c.DescontoPadrao,
                        Obs = c.Observacao
                    });
                }
                else if (papel is EmpresaFornecedor f)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_fornecedores (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = f.Observacao
                    });
                }
                else if (papel is EmpresaTransportador t)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_transportadores (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = t.Observacao
                    });
                }
                else if (papel is EmpresaLead l)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_leads (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = l.Observacao
                    });
                }
                else if (papel is EmpresaProspect pr)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_prospects (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = pr.Observacao
                    });
                }
            }
        });
    }

    public async Task<IEnumerable<Contato>> ListarPorContaIdAsync(Guid contaId, string? filtroNome = null, string? filtroTipo = null)
    {
        var parametros = new DynamicParameters();
        parametros.Add("ContaId", contaId);

        var sqlFisica = """
            SELECT 
                id, conta_id AS ContaId, 'fisica' AS TipoPessoa, nome AS NomeCompleto, 
                email, telefone, cpf, rg, data_nascimento::timestamp AS DataNascimento, status
            FROM contatos_pessoas_fisicas 
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            """;
            
        var sqlJuridica = """
            SELECT 
                id, conta_id AS ContaId, 'juridica' AS TipoPessoa, razao_social AS RazaoSocial, 
                email, telefone, cnpj, inscricao_estadual AS InscricaoEstadual, NULL::timestamp AS DataNascimento, status
            FROM contatos_pessoas_juridicas 
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            """;

        if (!string.IsNullOrWhiteSpace(filtroNome))
        {
            sqlFisica += " AND nome ILIKE @FiltroNome";
            sqlJuridica += " AND razao_social ILIKE @FiltroNome";
            parametros.Add("FiltroNome", $"%{filtroNome}%");
        }

        string sqlFinal;
        if (filtroTipo == "pf" || filtroTipo == "fisica")
        {
            sqlFinal = sqlFisica + " ORDER BY nome";
        }
        else if (filtroTipo == "pj" || filtroTipo == "juridica")
        {
            sqlFinal = sqlJuridica + " ORDER BY razao_social";
        }
        else
        {
            sqlFinal = $"""
                SELECT * FROM (
                    {sqlFisica}
                    UNION ALL
                    {sqlJuridica}
                ) q
                ORDER BY COALESCE(NomeCompleto, RazaoSocial)
                """;
        }

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryAsync<Contato>(sqlFinal, parametros);
    }

    public async Task<ResultadoPaginado<Contato>> ListarPaginadoAsync(Guid contaId, int pagina, int tamanhoPagina, string? busca = null, string? tipo = null, DateTime? desde = null, string? ordenarPor = null, string? ordenarDirecao = null, string? papel = null)
    {
        var deslocamento = (pagina - 1) * tamanhoPagina;

        var sqlFisica = """
            SELECT 
                id, conta_id AS ContaId, 'fisica' AS TipoPessoa, nome AS NomeCompleto, 
                email, telefone, cpf, rg, data_nascimento::timestamp AS DataNascimento, status
            FROM contatos_pessoas_fisicas 
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            """;

        var sqlJuridica = """
            SELECT 
                id, conta_id AS ContaId, 'juridica' AS TipoPessoa, razao_social AS RazaoSocial, 
                email, telefone, cnpj, inscricao_estadual AS InscricaoEstadual, NULL::timestamp AS DataNascimento, status
            FROM contatos_pessoas_juridicas 
            WHERE conta_id = @ContaId AND deleted_at IS NULL
            """;


        if (!string.IsNullOrWhiteSpace(papel))
        {
            var filtro = papel.ToLower() switch
            {
                "lead"          => "is_lead = true",
                "prospect"      => "is_prospect = true",
                "cliente"       => "is_cliente = true",
                "fornecedor"    => "is_fornecedor = true",
                "funcionario"   => "is_funcionario = true",
                "transportador" => "is_transportador = true",
                _               => null
            };
            if (filtro != null)
            {
                sqlFisica   += $" AND {filtro}";
                sqlJuridica += $" AND {filtro}";
            }
        }

        var sqlFiltradaFisica = sqlFisica;
        var sqlFiltradaJuridica = sqlJuridica;

        var parametros = new DynamicParameters();
        parametros.Add("ContaId", contaId);
        parametros.Add("TamanhoPagina", tamanhoPagina);
        parametros.Add("Deslocamento", deslocamento);

        if (!string.IsNullOrWhiteSpace(busca))
        {
            sqlFiltradaFisica += " AND (nome ILIKE @Busca OR email ILIKE @Busca OR cpf ILIKE @Busca OR rg ILIKE @Busca)";
            sqlFiltradaJuridica += " AND (razao_social ILIKE @Busca OR email ILIKE @Busca OR cnpj ILIKE @Busca OR inscricao_estadual ILIKE @Busca)";
            parametros.Add("Busca", $"%{busca}%");
        }

        if (desde.HasValue)
        {
            sqlFiltradaFisica += " AND data_inclusao >= @Desde";
            sqlFiltradaJuridica += " AND data_inclusao >= @Desde";
            parametros.Add("Desde", desde.Value);
        }

        var direcaoOrdenacao = "ASC";
        if (string.Equals(ordenarDirecao, "desc", StringComparison.OrdinalIgnoreCase))
        {
            direcaoOrdenacao = "DESC";
        }

        string sqlTotal;
        string sqlItens;

        if (tipo == "fisica")
        {
            var clausulaOrdem = ordenarPor?.ToLower() switch
            {
                "nome" => $"nome {direcaoOrdenacao}",
                "email" => $"email {direcaoOrdenacao}",
                "documento" => $"cpf {direcaoOrdenacao}",
                "status" => $"status {direcaoOrdenacao}",
                _ => $"nome {direcaoOrdenacao}"
            };

            sqlTotal = $"SELECT COUNT(1) FROM ({sqlFiltradaFisica}) q";
            sqlItens = $"{sqlFiltradaFisica} ORDER BY {clausulaOrdem} LIMIT @TamanhoPagina OFFSET @Deslocamento";
        }
        else if (tipo == "juridica")
        {
            var clausulaOrdem = ordenarPor?.ToLower() switch
            {
                "nome" => $"razao_social {direcaoOrdenacao}",
                "email" => $"email {direcaoOrdenacao}",
                "documento" => $"cnpj {direcaoOrdenacao}",
                "status" => $"status {direcaoOrdenacao}",
                _ => $"razao_social {direcaoOrdenacao}"
            };

            sqlTotal = $"SELECT COUNT(1) FROM ({sqlFiltradaJuridica}) q";
            sqlItens = $"{sqlFiltradaJuridica} ORDER BY {clausulaOrdem} LIMIT @TamanhoPagina OFFSET @Deslocamento";
        }
        else
        {
            var clausulaOrdem = ordenarPor?.ToLower() switch
            {
                "nome" => $"NomeExibicao {direcaoOrdenacao}",
                "email" => $"email {direcaoOrdenacao}",
                "documento" => $"Documento {direcaoOrdenacao}",
                "tipo" => $"TipoPessoa {direcaoOrdenacao}",
                "status" => $"status {direcaoOrdenacao}",
                _ => $"NomeExibicao {direcaoOrdenacao}"
            };

            var sqlUniao = $"""
                SELECT id, ContaId, TipoPessoa, NomeCompleto AS NomeExibicao, email, telefone, cpf AS Documento, rg, DataNascimento, NULL AS InscricaoEstadual, status FROM ({sqlFiltradaFisica}) f
                UNION ALL
                SELECT id, ContaId, TipoPessoa, RazaoSocial AS NomeExibicao, email, telefone, cnpj AS Documento, NULL AS rg, DataNascimento, InscricaoEstadual, status FROM ({sqlFiltradaJuridica}) j
                """;
            sqlTotal = $"SELECT COUNT(1) FROM ({sqlUniao}) q";
            sqlItens = $"""
                SELECT 
                    id, ContaId, TipoPessoa, 
                    CASE WHEN TipoPessoa = 'fisica' THEN NomeExibicao ELSE NULL END AS NomeCompleto,
                    CASE WHEN TipoPessoa = 'juridica' THEN NomeExibicao ELSE NULL END AS RazaoSocial,
                    email, telefone, 
                    CASE WHEN TipoPessoa = 'fisica' THEN Documento ELSE NULL END AS Cpf,
                    CASE WHEN TipoPessoa = 'juridica' THEN Documento ELSE NULL END AS Cnpj,
                    rg, 
                    InscricaoEstadual,
                    DataNascimento, status
                FROM ({sqlUniao}) q
                ORDER BY {clausulaOrdem}
                LIMIT @TamanhoPagina OFFSET @Deslocamento
                """;
        }

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var total = await conexao.ExecuteScalarAsync<int>(sqlTotal, parametros);
        var itens = await conexao.QueryAsync<Contato>(sqlItens, parametros);

        return new ResultadoPaginado<Contato>(itens, total, pagina, tamanhoPagina);
    }

    public async Task<Contato?> ObterPorIdEContaAsync(Guid id, Guid contaId)
    {
        const string sqlPessoa = """
            SELECT 
                id, conta_id AS ContaId, 'fisica' AS TipoPessoa, nome AS NomeCompleto, 
                email, telefone, cpf, rg, data_nascimento::timestamp AS DataNascimento, status,
                endereco_entrega_id AS EnderecoEntregaId, endereco_faturamento_id AS EnderecoFaturamentoId,
                endereco_fiscal_id AS EnderecoFiscalId, endereco_cobranca_id AS EnderecoCobrancaId,
                endereco_retirada_id AS EnderecoRetiradaId,
                vendedor_1_id AS Vendedor1Id, vendedor_2_id AS Vendedor2Id
            FROM contatos_pessoas_fisicas
            WHERE id = @Id AND conta_id = @ContaId AND deleted_at IS NULL
            
            UNION ALL
            
            SELECT 
                id, conta_id AS ContaId, 'juridica' AS TipoPessoa, razao_social AS RazaoSocial, 
                email, telefone, cnpj, inscricao_estadual AS InscricaoEstadual, NULL::timestamp AS DataNascimento, status,
                endereco_entrega_id AS EnderecoEntregaId, endereco_faturamento_id AS EnderecoFaturamentoId,
                endereco_fiscal_id AS EnderecoFiscalId, endereco_cobranca_id AS EnderecoCobrancaId,
                endereco_retirada_id AS EnderecoRetiradaId,
                vendedor_1_id AS Vendedor1Id, vendedor_2_id AS Vendedor2Id
            FROM contatos_pessoas_juridicas
            WHERE id = @Id AND conta_id = @ContaId AND deleted_at IS NULL
            """;

        const string sqlEnderecos = """
            SELECT id, contato_id AS ContatoId, conta_id AS ContaId, cep, logradouro, numero, complemento, bairro, cidade, uf
            FROM enderecos
            WHERE contato_id = @Id AND conta_id = @ContaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        var contato = await conexao.QueryFirstOrDefaultAsync<Contato>(sqlPessoa, new { Id = id, ContaId = contaId });

        if (contato != null)
        {
            var enderecos = await conexao.QueryAsync<Endereco>(sqlEnderecos, new { Id = id, ContaId = contaId });
            contato.Enderecos = enderecos.ToList();
        }

        return contato;
    }

    public async Task AtualizarContatoComPapeisAsync(Contato pessoaBase, IEnumerable<IContatoPapel> papeis)
    {
        var papeisList = papeis.ToList();
        var ehCliente = papeisList.Any(p => p is EmpresaCliente);
        var ehFornecedor = papeisList.Any(p => p is EmpresaFornecedor);
        var ehTransportador = papeisList.Any(p => p is EmpresaTransportador);
        var ehLead = papeisList.Any(p => p is EmpresaLead);
        var ehProspect = papeisList.Any(p => p is EmpresaProspect);
        var ehFuncionario = papeisList.Any(p => p is EmpresaFuncionario);

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("DELETE FROM enderecos WHERE contato_id = @Id AND conta_id = @ContaId", new { Id = pessoaBase.Id, ContaId = pessoaBase.ContaId });
            
            if (pessoaBase.Enderecos != null && pessoaBase.Enderecos.Any())
            {
                const string sqlEndereco = """
                    INSERT INTO enderecos
                        (id, contato_id, conta_id, cep, logradouro, numero, complemento, bairro, cidade, uf)
                    VALUES
                        (@Id, @ContatoId, @ContaId, @Cep, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Uf)
                    """;
                
                foreach (var end in pessoaBase.Enderecos)
                {
                    await conexao.ExecuteAsync(sqlEndereco, new
                    {
                        Id = end.Id,
                        ContatoId = pessoaBase.Id,
                        ContaId = pessoaBase.ContaId,
                        Cep = end.Cep,
                        Logradouro = end.Logradouro,
                        Numero = end.Numero,
                        Complemento = end.Complemento,
                        Bairro = end.Bairro,
                        Cidade = end.Cidade,
                        Uf = end.Uf
                    });
                }
            }

            if (pessoaBase.TipoPessoa == "fisica")
            {
                await conexao.ExecuteAsync("""
                    UPDATE contatos_pessoas_fisicas
                    SET nome = @Nome, cpf = @Cpf, rg = @Rg, data_nascimento = @DataNascimento, 
                        email = @Email, telefone = @Telefone, is_cliente = @EhCliente, is_fornecedor = @EhFornecedor,
                        is_transportador = @EhTransportador,
                        is_lead = @EhLead, is_prospect = @EhProspect, is_funcionario = @EhFuncionario,
                        vendedor_1_id = @Vendedor1Id, vendedor_2_id = @Vendedor2Id,
                        endereco_entrega_id = @EnderecoEntregaId, endereco_faturamento_id = @EnderecoFaturamentoId,
                        endereco_fiscal_id = @EnderecoFiscalId, endereco_cobranca_id = @EnderecoCobrancaId,
                        endereco_retirada_id = @EnderecoRetiradaId,
                        envia_email_agendamento = @EnviaEmailAgendamento
                    WHERE id = @Id AND conta_id = @ContaId
                    """, new
                {
                    Id = pessoaBase.Id,
                    ContaId = pessoaBase.ContaId,
                    Nome = pessoaBase.NomeCompleto ?? "",
                    Cpf = pessoaBase.Cpf,
                    Rg = pessoaBase.Rg,
                    DataNascimento = pessoaBase.DataNascimento,
                    Email = pessoaBase.Email,
                    Telefone = pessoaBase.Telefone,
                    EhCliente = ehCliente,
                    EhFornecedor = ehFornecedor,
                    EhTransportador = ehTransportador,
                    EhLead = ehLead,
                    EhProspect = ehProspect,
                    EhFuncionario = ehFuncionario,
                    Vendedor1Id = pessoaBase.Vendedor1Id,
                    Vendedor2Id = pessoaBase.Vendedor2Id,
                    EnderecoEntregaId = pessoaBase.EnderecoEntregaId,
                    EnderecoFaturamentoId = pessoaBase.EnderecoFaturamentoId,
                    EnderecoFiscalId = pessoaBase.EnderecoFiscalId,
                    EnderecoCobrancaId = pessoaBase.EnderecoCobrancaId,
                    EnderecoRetiradaId = pessoaBase.EnderecoRetiradaId,
                    EnviaEmailAgendamento = pessoaBase.EnviaEmailAgendamento
                });
            }
            else
            {
                await conexao.ExecuteAsync("""
                    UPDATE contatos_pessoas_juridicas
                    SET razao_social = @RazaoSocial, nome_fantasia = @NomeFantasia, cnpj = @Cnpj, 
                        inscricao_estadual = @InscricaoEstadual, email = @Email, telefone = @Telefone,
                        is_cliente = @EhCliente, is_fornecedor = @EhFornecedor,
                        is_transportador = @EhTransportador,
                        is_lead = @EhLead, is_prospect = @EhProspect, is_funcionario = @EhFuncionario,
                        vendedor_1_id = @Vendedor1Id, vendedor_2_id = @Vendedor2Id,
                        endereco_entrega_id = @EnderecoEntregaId, endereco_faturamento_id = @EnderecoFaturamentoId,
                        endereco_fiscal_id = @EnderecoFiscalId, endereco_cobranca_id = @EnderecoCobrancaId,
                        endereco_retirada_id = @EnderecoRetiradaId,
                        envia_email_agendamento = @EnviaEmailAgendamento
                    WHERE id = @Id AND conta_id = @ContaId
                    """, new
                {
                    Id = pessoaBase.Id,
                    ContaId = pessoaBase.ContaId,
                    RazaoSocial = pessoaBase.RazaoSocial ?? "",
                    NomeFantasia = pessoaBase.NomeFantasia,
                    Cnpj = pessoaBase.Cnpj,
                    InscricaoEstadual = pessoaBase.InscricaoEstadual,
                    Email = pessoaBase.Email,
                    Telefone = pessoaBase.Telefone,
                    EhCliente = ehCliente,
                    EhFornecedor = ehFornecedor,
                    EhTransportador = ehTransportador,
                    EhLead = ehLead,
                    EhProspect = ehProspect,
                    EhFuncionario = ehFuncionario,
                    Vendedor1Id = pessoaBase.Vendedor1Id,
                    Vendedor2Id = pessoaBase.Vendedor2Id,
                    EnderecoEntregaId = pessoaBase.EnderecoEntregaId,
                    EnderecoFaturamentoId = pessoaBase.EnderecoFaturamentoId,
                    EnderecoFiscalId = pessoaBase.EnderecoFiscalId,
                    EnderecoCobrancaId = pessoaBase.EnderecoCobrancaId,
                    EnderecoRetiradaId = pessoaBase.EnderecoRetiradaId,
                    EnviaEmailAgendamento = pessoaBase.EnviaEmailAgendamento
                });
            }

            await conexao.ExecuteAsync("DELETE FROM empresas_clientes WHERE contato_id = @Id", new { Id = pessoaBase.Id });
            await conexao.ExecuteAsync("DELETE FROM empresas_fornecedores WHERE contato_id = @Id", new { Id = pessoaBase.Id });
            await conexao.ExecuteAsync("DELETE FROM empresas_transportadores WHERE contato_id = @Id", new { Id = pessoaBase.Id });
            await conexao.ExecuteAsync("DELETE FROM empresas_leads WHERE contato_id = @Id", new { Id = pessoaBase.Id });
            await conexao.ExecuteAsync("DELETE FROM empresas_prospects WHERE contato_id = @Id", new { Id = pessoaBase.Id });
            await conexao.ExecuteAsync("DELETE FROM empresas_funcionarios WHERE contato_id = @Id", new { Id = pessoaBase.Id });


            foreach (var papel in papeisList)
            {
                if (papel is EmpresaCliente c)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_clientes (empresa_id, contato_id, contato_tipo, limite_credito, desconto_padrao, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Limite, @Desconto, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Limite = c.LimiteCredito,
                        Desconto = c.DescontoPadrao,
                        Obs = c.Observacao
                    });
                }
                else if (papel is EmpresaFornecedor f)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_fornecedores (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = f.Observacao
                    });
                }
                else if (papel is EmpresaTransportador t)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_transportadores (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = t.Observacao
                    });
                }
                else if (papel is EmpresaLead l)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_leads (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = l.Observacao
                    });
                }
                else if (papel is EmpresaProspect pr)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_prospects (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = pr.Observacao
                    });
                }
                else if (papel is EmpresaFuncionario fun)
                {
                    await conexao.ExecuteAsync("""
                        INSERT INTO empresas_funcionarios (empresa_id, contato_id, contato_tipo, observacao)
                        VALUES (@EmpresaId, @ContatoId, @Tipo, @Obs)
                        """, new
                    {
                        EmpresaId = pessoaBase.EmpresaId,
                        ContatoId = pessoaBase.Id,
                        Tipo = pessoaBase.TipoPessoa,
                        Obs = fun.Observacao
                    });
                }
            }
        });
    }

    public async Task<EmpresaCliente?> ObterPapelClienteAsync(Guid pessoaId, Guid empresaId)
    {
        const string sql = """
            SELECT contato_id AS PessoaId, empresa_id AS ContaId, limite_credito AS LimiteCredito, desconto_padrao AS DescontoPadrao, observacao AS Observacao
            FROM empresas_clientes
            WHERE contato_id = @ContatoId AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<EmpresaCliente>(sql, new { ContatoId = pessoaId, EmpresaId = empresaId });
    }

    public async Task<EmpresaFornecedor?> ObterPapelFornecedorAsync(Guid pessoaId, Guid empresaId)
    {
        const string sql = """
            SELECT contato_id AS PessoaId, empresa_id AS ContaId, observacao AS Observacao
            FROM empresas_fornecedores
            WHERE contato_id = @ContatoId AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<EmpresaFornecedor>(sql, new { ContatoId = pessoaId, EmpresaId = empresaId });
    }

    public async Task<EmpresaTransportador?> ObterPapelTransportadorAsync(Guid pessoaId, Guid empresaId)
    {
        const string sql = """
            SELECT contato_id AS PessoaId, empresa_id AS ContaId, observacao AS Observacao
            FROM empresas_transportadores
            WHERE contato_id = @ContatoId AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<EmpresaTransportador>(sql, new { ContatoId = pessoaId, EmpresaId = empresaId });
    }

    public async Task<EmpresaLead?> ObterPapelLeadAsync(Guid pessoaId, Guid empresaId)
    {
        const string sql = """
            SELECT contato_id AS ContatoId, empresa_id AS ContaId, observacao AS Observacao
            FROM empresas_leads
            WHERE contato_id = @ContatoId AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<EmpresaLead>(sql, new { ContatoId = pessoaId, EmpresaId = empresaId });
    }

    public async Task<EmpresaProspect?> ObterPapelProspectAsync(Guid pessoaId, Guid empresaId)
    {
        const string sql = """
            SELECT contato_id AS ContatoId, empresa_id AS ContaId, observacao AS Observacao
            FROM empresas_prospects
            WHERE contato_id = @ContatoId AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<EmpresaProspect>(sql, new { ContatoId = pessoaId, EmpresaId = empresaId });
    }

    public async Task<EmpresaFuncionario?> ObterPapelFuncionarioAsync(Guid pessoaId, Guid empresaId)
    {
        const string sql = """
            SELECT contato_id AS ContatoId, empresa_id AS ContaId, observacao AS Observacao
            FROM empresas_funcionarios
            WHERE contato_id = @ContatoId AND empresa_id = @EmpresaId
            """;

        await using var conexao = CriarConexao();
        await conexao.OpenAsync();
        return await conexao.QueryFirstOrDefaultAsync<EmpresaFuncionario>(sql, new { ContatoId = pessoaId, EmpresaId = empresaId });
    }

    public async Task DeletarSeguroAsync(Guid id, Guid contaId)
    {
        await ExecutarEmTransacaoAsync(async conexao =>
        {
            var registrosAfetadosPf = await conexao.ExecuteAsync("UPDATE contatos_pessoas_fisicas SET deleted_at = NOW() WHERE id = @Id AND conta_id = @ContaId", new { Id = id, ContaId = contaId });
            if (registrosAfetadosPf == 0)
                await conexao.ExecuteAsync("UPDATE contatos_pessoas_juridicas SET deleted_at = NOW() WHERE id = @Id AND conta_id = @ContaId", new { Id = id, ContaId = contaId });
        });
    }

    public async Task DeletarSeguroEmLoteAsync(IEnumerable<Guid> ids, Guid contaId)
    {
        var arrayIds = ids.ToArray();
        if (!arrayIds.Any()) return;

        await ExecutarEmTransacaoAsync(async conexao =>
        {
            await conexao.ExecuteAsync("UPDATE contatos_pessoas_fisicas SET deleted_at = NOW() WHERE id = ANY(@Ids) AND conta_id = @ContaId", new { Ids = arrayIds, ContaId = contaId });
            await conexao.ExecuteAsync("UPDATE contatos_pessoas_juridicas SET deleted_at = NOW() WHERE id = ANY(@Ids) AND conta_id = @ContaId", new { Ids = arrayIds, ContaId = contaId });
        });
    }
}
