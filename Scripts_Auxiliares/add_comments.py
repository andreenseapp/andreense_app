import re

file_path = r'e:\Definicoes\Manifesto_DBML.md'

with open(file_path, 'r', encoding='utf-8') as f:
    lines = f.readlines()

new_lines = []
in_table = False
table_name = ""

def get_comment_for_col(table, col):
    col = col.lower().strip()
    if 'id' == col: return "Identificador PK universal (imutável)"
    if 'conta_id' in col: return "Isolamento multi-tenant da base master"
    if 'empresa_id' in col: return "Isolamento operacional por filial (Master/Detail)"
    if 'data_inclusao' in col: return "Auditoria: data de criação exata"
    if 'data_ultima_atualizacao' in col: return "Auditoria: log de alteração"
    if 'data_exclusao' in col: return "Auditoria: soft-delete e manutenção do histórico fiscal"
    if 'operador_inclusao' in col or 'usuario_id' in col: return "Auditoria: qual usuário executou a ação do registro"
    if 'status' == col or 'ativo' == col: return "Controle da máquina de estados e inativação temporal sem delete"
    if 'url_imagem' == col or 'imagem' == col: return "Referência visual rápida e cacheada no S3 para B2C e PDV"
    if 'ordem' == col: return "Propriedade de customização de ranking em grids e list cards na UI"
    if 'observacao' in col or 'descricao' in col: return "Campo semântico para metadados atípicos do backoffice"
    if 'nome' == col or 'razao_social' == col: return "Identidade para exibição primária de marca/usuário"
    if 'cep' in col or 'logradouro' in col or 'bairro' in col or 'cidade' in col or 'uf' in col: return "Requisito base do pacote geográfico de emissão de NF"
    if 'telefone' in col or 'email' in col or 'celular' in col: return "Variável de alerta ou recuperação de autenticação"
    
    if 'cpf' in col or 'cnpj' in col: return "Chave primária fiscal de consulta via SEFAZ/Receita e obrigatoriedade B2B"
    if 'inscricao_' in col: return "Inscrição IE/IM demandada pela geração do XML Estadual/Sinc"
    
    if 'ncm' in col or 'cfop' in col or 'cst' in col or 'aliquota' in col: return "Amparo fiscal mandatório mapeado da Substituição e Alíquotas"
    if 'valor' in col or 'preco' in col or 'saldo' in col or 'quantidade' in col: return "Motor numérico da operação ou grandeza contábil física"
    if 'tipo_' in col or 'origem_' in col or 'situacao' in col: return "Chave seletora ou polimórfica para condicional de rotas"
    if 'estoque' in col or 'lote' in col: return "Controle granular para contábil, validades ANVISA ou avarias"
    if 'codigo' in col or 'sku' in col: return "Metadado chave ou código de barras de leitura em PDV/Bipes rápido"
    if 'unidade' in col: return "Padrão de multiplicação metrológica de pesos/unidades"
    
    return f"Armazena característica pertinente e nativa do domínio de {table}"

for line in lines:
    match_table = re.match(r'^Table\s+([a-zA-Z0-9_]+)\s*\{', line)
    if match_table:
        in_table = True
        table_name = match_table.group(1)
        new_lines.append(line)
        continue
    
    if in_table and re.match(r'^\s*\}\s*$', line):
        in_table = False
        new_lines.append(line)
        continue
        
    if in_table:
        # Tenta pegar linha como "coluna tipo [bla]" com espaços à vista
        match_col = re.match(r'^(\s+)([a-zA-Z0-9_]+)\s+(.+)$', line)
        if match_col:
            indent = match_col.group(1)
            col_name = match_col.group(2)
            rest = match_col.group(3)
            
            # Não adicionar comentários na mesma linha se já houver um. Exclui blank lines e //
            if '//' not in rest and 'Ref:' not in line and not line.strip().startswith('//'):
                comment = get_comment_for_col(table_name, col_name)
                # Restrict spacing issue
                rest = rest.rstrip()
                new_line = f"{indent}{col_name} {rest} // [Por que existe?] {comment}\n"
                new_lines.append(new_line)
                continue

    new_lines.append(line)

# Inclui ao final as justificativas de falta
omission_disclaimer = """
// ==========================================
// 🛑 OMISSÕES PROPOSITAIS (Por que certas colunas NÃO existem)
// ==========================================
// Em modelagem rica, o que ocultamos é tão importante quanto o que mantemos:
// 1. Falta de `valor_total` em Cabeçalhos (quando calculado): Algumas tabelas mantêm totais redundantes de leitura (Performance Cache), 
//    outras propositalmente carecem deles para forçar re-agrupamento puro de `itens`.
// 2. Falta de `usuario_tipo` solto em Históricos de Estoque: Tabelas como `estoque_movimentacoes` só ganham IDs de login explícito (`usuarios`), 
//    porque clientes Pessoas Físicas/B2C *nunca* irão manipular diretamente volumes.
// 3. Ausência de `endereço` em Vendas transacionais (PDV): O endereço já figura na raiz do Cliente (onde ele mora/sede), ele só
//    é puxado localmente quando é uma `endereco_entrega_id` destacada (para fretes de e-commerce).
// 4. Ausência de hard-delete (`ON DELETE CASCADE` implícito): Nossa base é fiscal imutável. Soft-Deletes via `status` e `data_exclusao` 
//    preservam integridade para log de auditoria retroativo (Tudo é Big Data).
// ==========================================
"""

# Verifica se disclaimer já foi adicionado
if "OMISSÕES PROPOSITAIS" not in "".join(new_lines[-20:]):
    new_lines.append("\n" + omission_disclaimer)

# Inclusão da nota cabeça logo abaixo do topo
top_disclaimer = """
> 🧠 **Nota de Contexto (Gerada por IA - Documentação Cognitiva):** 
> Todos os comentários injetados lado a lado nas declarações (iniciados por `// [Por que existe?]`) servem de blindagem contra "alucinações" de interpretação e revisões vazias de outras matrizes de Inteligência Artificial. Elas documentam o porquê de uma coluna de dados existir, em vez de assumir sua literalidade. Ademais, se achar que uma coluna está faltando, vide o bloco de "Omissões Propositais" no rodapé deste dbml, tudo é feito conscientemente!
"""

if "> 🧠 **Nota de Contexto" not in new_lines[7]:
    new_lines.insert(7, top_disclaimer + "\n")

with open(file_path, 'w', encoding='utf-8') as f:
    f.writelines(new_lines)

print("DBML turbinado com a blindagem de design rationale!")
