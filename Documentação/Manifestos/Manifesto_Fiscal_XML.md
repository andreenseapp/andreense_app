# Manifesto Oficial — ERP Web (Fiscal e Integração XML)

> ⚠️ **ESCOPO DESTE DOCUMENTO**: Este manifesto define as regras de emissão de NFe/NFCe e mapeamento de campos XML SEFAZ. Para o projeto ativo `c:\Projeto\Sistema\Modulo_Monolitico`, o **manifesto de referência único é o [`Manifesto_Monolitico.md`](./Manifesto_Monolitico.md)**. Este documento é consultado apenas quando trabalhando no módulo fiscal/faturamento.

Este documento define a arquitetura e correlação estrita entre as emissões de Documentos Fiscais Eletrônicos (NFe mod 55 e NFCe mod 65) e a estrutura do nosso banco de dados. Qualquer alteração nas tabelas deve invariavelmente suprir os requisitos listados pela SEFAZ mapeados abaixo.


## 1. Regras de Ouro (Armazenamento Fiscal)
* **Persistência Completa**: Dados geradores de nota fiscal JAMAIS sofrem *Soft Delete* sem validação contábil severa (Cancelar NF-e impõe regras de prazo na SEFAZ e guarda de histórico). 
* **Tabelas Envolvidas**: O espelho exato do XML se apoia essencialmente nas tabelas `faturas` (Header da Nota) e `fatura_itens` (Produtos da Nota).
* **Guarda de XML**: A tabela faturas deve persistir obrigatoriamente a assinatura (`xml_url` ou payload completo retornado pela SEFAZ) e a `chave_acesso` de 44 dígitos autêntica.

---

## 2. De-Para Estrutural: Header da Nota (Tag `<ide>` e `<total>`)

A tabela `faturas` armazena o corpo agregador da transação de saída/entrada, com ligações essenciais da Identificação (`<ide>`) e Totalização (`<total>`).

| Tag XML SEFAZ | Coluna `faturas` | Tipo Padrão | Descrição de Uso |
| --- | --- | --- | --- |
| `<nNF>` | `numero_nota_fiscal` | TEXT / INT | Número sequencial da Nota Fiscal. |
| `<serie>` | `serie_nota_fiscal` | TEXT / INT | Série do documento (Ex: "1", "001"). |
| `<dhEmi>` | `data_emissao` | TIMESTAMPTZ | Data e Hora exata da emissão UTC. |
| `<tpNF>` | `tipo_operacao` | VARCHAR(20) | Indica Entrada (0) ou Saída (1). |
| `<finNFe>` | `finalidade_emissao` | INT | Ex: 1-Normal, 2-Complementar, 3-Ajuste, 4-Devolução. |
| `<chNFe>` | `chave_acesso` | TEXT | Chave eletrônica autenticada pela SEFAZ. |
| Ref `<protNFe>` | `status_sefaz` | TEXT | Ex: "Autorizado", "Rejeitado", "Cancelado". |
| `<vNF>` | `valor_total` | NUMERIC | Somatória final de tudo (Produtos + Tributos Ativos).|
| `<vDesc>` | `valor_desconto` | NUMERIC | Desconto global aplicado no documento. |
| `<vSeg>` / `<vOutro>` | `valor_seguro` / `valor_outras_despesas` | NUMERIC | Acréscimos que compõem a base tributária. |
| `<vTotTrib>` | `valor_total_tributos` | NUMERIC | Carga tributária transparente (Lei da Transparência).|

---

## 3. De-Para Estrutural: Itens da Nota e Tributação (Tag `<det>` e `<imposto>`)

A tabela `fatura_itens` destrinça linha a linha, englobando a Reforma Tributária (IBS/CBS/IS) mapeada nos DOCXs.

| Tag XML SEFAZ | Coluna `fatura_itens` | Descrição de Uso |
| --- | --- | --- |
| `<cProd>` | `produto_variacao_id` | Reflete o SKU (Sistema próprio). |
| `<xProd>` | `nome_produto` | Nome descritivo persistido (imutável após emissão). |
| `<qCom>` / `<vUnCom>` | `quantidade` / `preco_unitario` | A volumetria da venda. |
| `<CFOP>` | `cfop` | Código Fiscal de Operações e Prestações (Ex: 5102, 5901). |
| `<NCM>` | Extraído via `produto_variacao_id` | Nomenclatura Comum do Mercosul atrelada. |

### Tributos Tradicionais (ICMS, IPI, PIS, COFINS)
| Tag SEFAZ | Subtipo | Coluna `fatura_itens` |
| --- | --- | --- |
| `<ICMS>` | CST/CSOSN | `cst_icms` |
| `<ICMS>` | Alíquota (%) | `aliquota_icms` |
| `<ICMS>` | Valor R$ | `valor_icms` |
| `<IPI>` | CST | `cst_ipi` |
| `<PIS>` | CST / Alíquota | `cst_pis` / `aliquota_pis` |
| `<COFINS>`| CST / Alíquota | `cst_cofins` / `aliquota_cofins` |

### Reforma Tributária (IBS e CBS) - *Tag Completa Nova*
Conforme constatação dos XMLs fiscais e a matriz definida, o bloco XML `<IBSCBS>` é fundamental.

| Tag SEFAZ | Coluna `fatura_itens` | Descrição |
| --- | --- | --- |
| `<CST>` | `cst_ibs_cbs` | Posição tributária unificada do item. |
| `<vBC>` | `valor_base_ibs` / `valor_base_cbs` | Base de cálculo dos Impostos CBS e IBS. |
| `<pIBS>` | `aliquota_ibs` | Percentual exigido do IBS (Estadual/Municipal).|
| `<vIBS>` | `valor_ibs` | R$ devido pelo Contribuinte. |
| `<pCBS>` | `aliquota_cbs` | Percentual exigido do CBS (Federal). |
| `<vCBS>` | `valor_cbs` | R$ devido pelo Contribuinte. |
| bloco `<IS>`*| `cst_is`, `aliquota_is`, `valor_is`| Imposto Seletivo ("Imposto do Pecado"). |

---

## 4. De-Para Estrutural: Emitente e Destinatário (Tags `<emit>` e `<dest>`)

Os dados do emitente são extraídos em tempo real da tabela `empresas` (campos: `razao_social`, `cnpj`, `inscricao_estadual`, `crt`, `endereco`, `uf`, `cidade`, `cep`). O destinatário é referenciado via `pessoas_fisicas` ou `pessoas_juridicas` (via `empresas_clientes`), com endereço em `enderecos_pf` / `enderecos_pj`. Nada é "chumbado": a Engine sempre lê o estado atual do cadastro.

---

## 5. De-Para Estrutural: Pagamento (Tag `<pag>`) e Frete (Tag `<transp>`)

Para a NFe (Mod 55) e NFCe (Mod 65), a SEFAZ exige estritamente a amarração de como o dinheiro foi entregue e a responsabilidade civil pelo deslocamento da mercadoria.

### Modais Fiscais de Pagamento (`<pag>`)
| Tag SEFAZ | Domínio do App | Descrição de Uso |
| --- | --- | --- |
| `<tPag>` | `formas_pagamento.codigo_sefaz` | A SEFAZ determina chaves estritas (Ex: `01` = Dinheiro, `04` = Cartão de Crédito, `17` = PIX). |
| `<vPag>` | Colunas de `contas_a_receber` | O valor que, teoricamente, constará em nosso Contas a Receber ligado à Fatura.|
| Ref `<detPag>`| Tabela `contas_a_receber` / `pedido_parcelas` | Detalhamento sobre parcelamentos quando houver mais de um trâmite.|

### Modalidade de Frete (`<transp>`)
A tabela de Fatura delega o conhecimento do Frete original gerado do pedido.

| Tag SEFAZ | Tabela Original (BD) | Descrição de Uso |
| --- | --- | --- |
| `<modFrete>` | `pedidos.modalidade_frete` | (Ex: `0` = Contratação pelo Remetente CIF, `1` = Destinatário FOB, `9` = Sem Frete). |

---

## 6. Dados Complementares (Tags `<infAdic>`, `<infRespTec>`, `<Signature>`)

### Módulo de Excedentes e Carimbos
| Tag SEFAZ | Domínio do App | Descrição |
| --- | --- | --- |
| `<infCpl>` | `faturas.observacao` | Reservado a leis complementares atadas que repassamos vindas de `orcamentos.observacoes`. |
| `<infRespTec>` | `config_responsavel_tecnico` | Os dados técnicos globais da provedora do software (CNPJ, Nome, Email e Hash CSRT). |
| `<qrCode>` | Payload nativo ou N/A | Na emissão de NFCe (XML 65), o link retornado pela própria SEFAZ, o Back-End entregará traduzido para impressão do PDV. |

---

## 7. Matriz Tributária e Responsabilidades

* O Cadastro de **`naturezas_operacao`** dirá se aquela ação `gera_financeiro` e `movimenta_estoque`. *(Ex: Remessa para Industrialização não gera cobrança de Cliente).*
* **Ambientes**: Homologação (Testes SEFAZ) e Produção são geridos na Configuração Global de Empresa (`tpAmb`).
* O cálculo tributário retroativo deve sempre seguir a hierarquia de `regras_fiscais` (Matriz de NCM x UF do Destinatário x CFOP). As tabelas `orcamentos` e `pedidos` operam simuladores das rotinas prontas destas naturezas; já a `fatura` é a última parada, o chancelamento real e imutável de comunicação com a Nuvem Nacional.

---

## 8. Dinamicidade Fiscal e Eliminação de Origens Estáticas "Hardcodes"

Nenhuma tag XML deve ser inferida ou "chumbada" diretamente pelo código (*hardcoded*). Todas as condições fiscais são dinâmicas, orientadas a banco de dados.

* **Regime Tributário do Emissor (Tag `<CRT>`)**: A tabela `empresas` conta com a coluna `crt` (Código de Regime Tributário). Dependendo do valor numérico *(Ex: 1 - Simples Nacional, 3 - Regime Normal)*, o motor ou a trigger encarregada do faturamento montará tributos distintos para a SEFAZ (ex: tag `CSOSN` substitui a `CST` se for Simples Nacional).
* **Parâmetros de Origem**: Quando uma regra depende inerentemente do "tipo" de filiação da empresa, nunca escreveremos `If (empresa.id == x)`, mas sim leremos as parametrizações `crt`, `inscricao_estadual`, `cnae_principal` vindas diretamente do DTD base da organização emissora.
* A regra unânime é: **A Engine Geradora de XML é passiva e reflete a Base de Dados.**
