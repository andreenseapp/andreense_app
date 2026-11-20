DO $$
BEGIN
    -- 1. Tabelas de Vendas e Pedidos (itens e parcelas primeiro)
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pedido_venda_itens') THEN EXECUTE $str$DELETE FROM public.pedido_venda_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pedido_venda_parcelas') THEN EXECUTE $str$DELETE FROM public.pedido_venda_parcelas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pedidos_venda') THEN EXECUTE $str$DELETE FROM public.pedidos_venda$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pedido_compra_itens') THEN EXECUTE $str$DELETE FROM public.pedido_compra_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pedido_compra_parcelas') THEN EXECUTE $str$DELETE FROM public.pedido_compra_parcelas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pedidos_compra') THEN EXECUTE $str$DELETE FROM public.pedidos_compra$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'nota_fiscal_itens') THEN EXECUTE $str$DELETE FROM public.nota_fiscal_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'notas_fiscais') THEN EXECUTE $str$DELETE FROM public.notas_fiscais$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'ordem_servico_itens') THEN EXECUTE $str$DELETE FROM public.ordem_servico_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'ordens_servico') THEN EXECUTE $str$DELETE FROM public.ordens_servico$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'orcamento_itens') THEN EXECUTE $str$DELETE FROM public.orcamento_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'orcamentos') THEN EXECUTE $str$DELETE FROM public.orcamentos$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'venda_itens') THEN EXECUTE $str$DELETE FROM public.venda_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'vendas') THEN EXECUTE $str$DELETE FROM public.vendas$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'embarque_itens') THEN EXECUTE $str$DELETE FROM public.embarque_itens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'embarques') THEN EXECUTE $str$DELETE FROM public.embarques$str$; END IF;

    -- 2. Financeiro
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_a_pagar_movimentacao') THEN EXECUTE $str$DELETE FROM public.contas_a_pagar_movimentacao$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_a_pagar') THEN EXECUTE $str$DELETE FROM public.contas_a_pagar$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_pagar') THEN EXECUTE $str$DELETE FROM public.contas_pagar$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_a_receber_movimentacao') THEN EXECUTE $str$DELETE FROM public.contas_a_receber_movimentacao$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_a_receber_comissao_deducao') THEN EXECUTE $str$DELETE FROM public.contas_a_receber_comissao_deducao$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_a_receber_comissao') THEN EXECUTE $str$DELETE FROM public.contas_a_receber_comissao$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_a_receber') THEN EXECUTE $str$DELETE FROM public.contas_a_receber$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_receber') THEN EXECUTE $str$DELETE FROM public.contas_receber$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'conta_a_pagar_credito') THEN EXECUTE $str$DELETE FROM public.conta_a_pagar_credito$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'conta_a_receber_credito') THEN EXECUTE $str$DELETE FROM public.conta_a_receber_credito$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'comissao_deducao_regra') THEN EXECUTE $str$DELETE FROM public.comissao_deducao_regra$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'controle_caixa') THEN EXECUTE $str$DELETE FROM public.controle_caixa$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'configuracoes_emissoes') THEN EXECUTE $str$DELETE FROM public.configuracoes_emissoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'config_responsavel_tecnico') THEN EXECUTE $str$DELETE FROM public.config_responsavel_tecnico$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'certificados_digitais') THEN EXECUTE $str$DELETE FROM public.certificados_digitais$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pagamentos_logs_app') THEN EXECUTE $str$DELETE FROM public.pagamentos_logs_app$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'pagamentos_app') THEN EXECUTE $str$DELETE FROM public.pagamentos_app$str$; END IF;

    -- 3. Mensagens e Notificações
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'mensagens_leituras') THEN EXECUTE $str$DELETE FROM public.mensagens_leituras$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'mensagens_participantes') THEN EXECUTE $str$DELETE FROM public.mensagens_participantes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'mensagens_anexos') THEN EXECUTE $str$DELETE FROM public.mensagens_anexos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'mensagens') THEN EXECUTE $str$DELETE FROM public.mensagens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'mensagens_topicos') THEN EXECUTE $str$DELETE FROM public.mensagens_topicos$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'notificacoes_lidas_globais') THEN EXECUTE $str$DELETE FROM public.notificacoes_lidas_globais$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'notificacoes') THEN EXECUTE $str$DELETE FROM public.notificacoes$str$; END IF;

    -- 4. Agenda, Atividades e Profissionais
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'agendamento_servicos') THEN EXECUTE $str$DELETE FROM public.agendamento_servicos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'agendamento_config') THEN EXECUTE $str$DELETE FROM public.agendamento_config$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'agendamentos') THEN EXECUTE $str$DELETE FROM public.agendamentos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'bloqueios_agenda') THEN EXECUTE $str$DELETE FROM public.bloqueios_agenda$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'atividades') THEN EXECUTE $str$DELETE FROM public.atividades$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_profissionais_horarios') THEN EXECUTE $str$DELETE FROM public.empresas_profissionais_horarios$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_profissionais') THEN EXECUTE $str$DELETE FROM public.empresas_profissionais$str$; END IF;

    -- 5. Clientes, Fornecedores, Transportadores e Leads
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_leads') THEN EXECUTE $str$DELETE FROM public.empresas_leads$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_prospects') THEN EXECUTE $str$DELETE FROM public.empresas_prospects$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_clientes') THEN EXECUTE $str$DELETE FROM public.empresas_clientes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_fornecedores') THEN EXECUTE $str$DELETE FROM public.empresas_fornecedores$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_transportadores') THEN EXECUTE $str$DELETE FROM public.empresas_transportadores$str$; END IF;

    -- 6. Contatos e Endereços
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'enderecos_pf') THEN EXECUTE $str$DELETE FROM public.enderecos_pf$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'enderecos_pj') THEN EXECUTE $str$DELETE FROM public.enderecos_pj$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contatos_pessoas_fisicas') THEN EXECUTE $str$DELETE FROM public.contatos_pessoas_fisicas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contatos_pessoas_juridicas') THEN EXECUTE $str$DELETE FROM public.contatos_pessoas_juridicas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'enderecos') THEN EXECUTE $str$DELETE FROM public.enderecos WHERE id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    -- 7. Produtos, Preços e Estoque
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_precos') THEN EXECUTE $str$DELETE FROM public.erp_precos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_contextos_preco') THEN EXECUTE $str$DELETE FROM public.erp_contextos_preco$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contextos_preco') THEN EXECUTE $str$DELETE FROM public.contextos_preco$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_estoque_movimentacoes') THEN EXECUTE $str$DELETE FROM public.erp_estoque_movimentacoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_estoque_saldo') THEN EXECUTE $str$DELETE FROM public.erp_estoque_saldo$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_locais_estoque') THEN EXECUTE $str$DELETE FROM public.erp_locais_estoque$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_areas_estoque') THEN EXECUTE $str$DELETE FROM public.erp_areas_estoque$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'estoque_movimentacoes') THEN EXECUTE $str$DELETE FROM public.estoque_movimentacoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'estoque_saldo') THEN EXECUTE $str$DELETE FROM public.estoque_saldo$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'locais_estoque') THEN EXECUTE $str$DELETE FROM public.locais_estoque$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'areas_estoque') THEN EXECUTE $str$DELETE FROM public.areas_estoque$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_produtos_variacoes') THEN EXECUTE $str$DELETE FROM public.erp_produtos_variacoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_produtos') THEN EXECUTE $str$DELETE FROM public.erp_produtos WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_marcas') THEN EXECUTE $str$DELETE FROM public.erp_marcas WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_produto_categorias') THEN EXECUTE $str$DELETE FROM public.erp_produto_categorias WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'precos') THEN EXECUTE $str$DELETE FROM public.precos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'produto_variacoes') THEN EXECUTE $str$DELETE FROM public.produto_variacoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'produtos') THEN EXECUTE $str$DELETE FROM public.produtos WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'marcas') THEN EXECUTE $str$DELETE FROM public.marcas WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'produto_categorias') THEN EXECUTE $str$DELETE FROM public.produto_categorias WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_conversao_unidades') THEN EXECUTE $str$DELETE FROM public.erp_conversao_unidades$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'erp_unidades_medida') THEN EXECUTE $str$DELETE FROM public.erp_unidades_medida WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    -- 8. Cashback e Fidelidade
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'cashback_creditos') THEN EXECUTE $str$DELETE FROM public.cashback_creditos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'cashback_saldos') THEN EXECUTE $str$DELETE FROM public.cashback_saldos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'cashback_utilizacoes') THEN EXECUTE $str$DELETE FROM public.cashback_utilizacoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'cashback_tokens') THEN EXECUTE $str$DELETE FROM public.cashback_tokens$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'cashback_regras') THEN EXECUTE $str$DELETE FROM public.cashback_regras$str$; END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'fidelidade_carimbos') THEN EXECUTE $str$DELETE FROM public.fidelidade_carimbos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'fidelidade_logs') THEN EXECUTE $str$DELETE FROM public.fidelidade_logs$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'fidelidade_cartoes') THEN EXECUTE $str$DELETE FROM public.fidelidade_cartoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'fidelidade_saldos') THEN EXECUTE $str$DELETE FROM public.fidelidade_saldos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'fidelidade_tokens') THEN EXECUTE $str$DELETE FROM public.fidelidade_tokens$str$; END IF;

    -- 9. Configurações Fiscais e Tabelas Auxiliares
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'condicao_pagamento_parcelas') THEN EXECUTE $str$DELETE FROM public.condicao_pagamento_parcelas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'condicoes_pagamento') THEN EXECUTE $str$DELETE FROM public.condicoes_pagamento$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'formas_pagamento_fiscal') THEN EXECUTE $str$DELETE FROM public.formas_pagamento_fiscal$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'formas_pagamento') THEN EXECUTE $str$DELETE FROM public.formas_pagamento$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'naturezas_operacao') THEN EXECUTE $str$DELETE FROM public.naturezas_operacao$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'regras_fiscais') THEN EXECUTE $str$DELETE FROM public.regras_fiscais$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresa_cnaes_secundarios') THEN EXECUTE $str$DELETE FROM public.empresa_cnaes_secundarios$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas_funcoes') THEN EXECUTE $str$DELETE FROM public.empresas_funcoes$str$; END IF;

    -- 10. Tabelas de Vínculo de Contas e Empresas
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'usuarios_empresas') THEN EXECUTE $str$DELETE FROM public.usuarios_empresas WHERE usuario_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'usuarios_contas') THEN EXECUTE $str$DELETE FROM public.usuarios_contas WHERE usuario_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    -- 11. Auditoria, Logs, Vendedores e Grupos
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'auditoria_registros') THEN EXECUTE $str$DELETE FROM public.auditoria_registros$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'auditoria') THEN EXECUTE $str$DELETE FROM public.auditoria WHERE usuario_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'tentativas_login') THEN EXECUTE $str$TRUNCATE TABLE public.tentativas_login RESTART IDENTITY$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'vendedores') THEN EXECUTE $str$DELETE FROM public.vendedores$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'tipos_vendedor') THEN EXECUTE $str$DELETE FROM public.tipos_vendedor$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'grupos_permissoes') THEN EXECUTE $str$DELETE FROM public.grupos_permissoes WHERE grupo_id NOT IN (SELECT grupo_id FROM public.usuarios_empresas WHERE usuario_id = '00000000-0000-0000-0000-000000000000' AND grupo_id IS NOT NULL)$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'grupos_usuarios') THEN EXECUTE $str$DELETE FROM public.grupos_usuarios WHERE id NOT IN (SELECT grupo_id FROM public.usuarios_empresas WHERE usuario_id = '00000000-0000-0000-0000-000000000000' AND grupo_id IS NOT NULL)$str$; END IF;

    -- 12. Sessões, Profissões e Recursos
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'sessoes_usuario') THEN EXECUTE $str$DELETE FROM public.sessoes_usuario$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'sessoes_usuarios') THEN EXECUTE $str$DELETE FROM public.sessoes_usuarios$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'usuario_profissoes') THEN EXECUTE $str$DELETE FROM public.usuario_profissoes$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'codigos_validacao') THEN EXECUTE $str$DELETE FROM public.codigos_validacao$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'usuario_recursos_app') THEN EXECUTE $str$DELETE FROM public.usuario_recursos_app$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'documentos_aceites') THEN EXECUTE $str$DELETE FROM public.documentos_aceites$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'documentos_legais') THEN EXECUTE $str$DELETE FROM public.documentos_legais$str$; END IF;

    -- 13. Grupos SaaS
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'usuarios_grupos_saas') THEN EXECUTE $str$DELETE FROM public.usuarios_grupos_saas WHERE usuario_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    -- 14. SaaS e Entitlements
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'faturas_saas') THEN EXECUTE $str$DELETE FROM public.faturas_saas WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_assinaturas_saas') THEN EXECUTE $str$DELETE FROM public.contas_assinaturas_saas WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas_addons_saas') THEN EXECUTE $str$DELETE FROM public.contas_addons_saas WHERE conta_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'saas_planos_assinaturas_faturas') THEN EXECUTE $str$DELETE FROM public.saas_planos_assinaturas_faturas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'saas_planos_assinaturas') THEN EXECUTE $str$DELETE FROM public.saas_planos_assinaturas$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'saas_cupons_desconto') THEN EXECUTE $str$DELETE FROM public.saas_cupons_desconto$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'saas_metodos_pagamento') THEN EXECUTE $str$DELETE FROM public.saas_metodos_pagamento$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'saas_planos') THEN EXECUTE $str$DELETE FROM public.saas_planos$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'servico_contratacoes_app') THEN EXECUTE $str$DELETE FROM public.servico_contratacoes_app$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'servico_desconto') THEN EXECUTE $str$DELETE FROM public.servico_desconto$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'assinaturas_app') THEN EXECUTE $str$DELETE FROM public.assinaturas_app$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'configuracoes_taxas_app') THEN EXECUTE $str$DELETE FROM public.configuracoes_taxas_app$str$; END IF;

    -- 15. Vínculos entre Contas
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'vinculos_contas') THEN EXECUTE $str$DELETE FROM public.vinculos_contas WHERE conta_a_id <> '00000000-0000-0000-0000-000000000000' OR conta_b_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'solicitacoes_vinculo') THEN EXECUTE $str$DELETE FROM public.solicitacoes_vinculo WHERE conta_solicitante_id <> '00000000-0000-0000-0000-000000000000' OR conta_alvo_id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

    -- 16. Entidades Principais
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'empresas') THEN EXECUTE $str$DELETE FROM public.empresas WHERE id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'contas') THEN EXECUTE $str$DELETE FROM public.contas WHERE id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'usuarios') THEN EXECUTE $str$DELETE FROM public.usuarios WHERE id <> '00000000-0000-0000-0000-000000000000'$str$; END IF;

END $$;
