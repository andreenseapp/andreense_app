import re
import os

dbml_path = r'c:\Projeto\Manifesto_DBML.md'
out_path = r'c:\Projeto\banco_script_inicial.sql'

with open(dbml_path, 'r', encoding='utf-8') as f:
    dbml = f.read()

lines = dbml.split('\n')

enums = {}
tables = {}
refs = []

def determine_schema(table_name):
    servicos_tables = ['empresas_funcoes', 'empresas_profissionais', 'empresas_profissionais_horarios', 'bloqueios_agenda', 'agendamentos', 'agendamento_servicos', 'servico_desconto', 'agendamento_config']
    if table_name in servicos_tables or table_name.startswith('agendamento'):
        return 'servicos'
    if table_name.endswith('_app') or 'app' in table_name.split('_'):
        return 'cliente'
    return 'erp'

current_table = None
current_enum = None

for i, line in enumerate(lines):
    original_line = line
    line = line.strip()
    if not line or line.startswith('//') or line.startswith('#') or line.startswith('>'):
        continue
    
    # enum match
    m_enum = re.match(r'Enum\s+(\w+)\s*\{?', line)
    if m_enum:
        current_enum = m_enum.group(1)
        enums[current_enum] = []
        continue
        
    m_table = re.match(r'Table\s+(\w+)\s*\{?', line)
    if m_table:
        current_table = m_table.group(1)
        tables[current_table] = []
        continue
        
    m_ref = re.match(r'Ref:\s*([\w\.]+)\s*([><-])\s*([\w\.]+)', line)
    if m_ref:
        t1_col1 = m_ref.group(1).split('.')
        op = m_ref.group(2)
        t2_col2 = m_ref.group(3).split('.')
        if len(t1_col1) == 2 and len(t2_col2) == 2:
            t1, c1 = t1_col1
            t2, c2 = t2_col2
            if op == '>':
                refs.append({'fk_table': t1, 'fk_col': c1, 'ref_table': t2, 'ref_col': c2})
            elif op == '<':
                refs.append({'fk_table': t2, 'fk_col': c2, 'ref_table': t1, 'ref_col': c1})
            elif op == '-':
                refs.append({'fk_table': t1, 'fk_col': c1, 'ref_table': t2, 'ref_col': c2, 'unique': True})
        continue
        
    if line == '}' or line.startswith('}'):
        current_enum = None
        current_table = None
        continue
        
    if current_enum:
        val = line.strip()
        if val != '{':
            enums[current_enum].append(val)
        continue
        
    if current_table:
        if line == '{':
            continue
        # handle [attrs] with commas inside them properly by using a regex that matches brackets
        # try to split line into definition and comment
        parts = line.split('//', 1)
        def_part = parts[0].strip()
        comment_part = parts[1].strip() if len(parts) > 1 else ''
        
        # trim trailing comma from def_part
        if def_part.endswith(','):
            def_part = def_part[:-1].strip()
            
        # extract attrs inside brackets
        attrs = ''
        m_attrs = re.search(r'\[(.*?)\]$', def_part)
        if m_attrs:
            attrs = m_attrs.group(1)
            def_part = def_part[:def_part.rfind('[')].strip()
            
        m_col = re.match(r'^(\w+)\s+([\w\.\(\)]+)', def_part)
        if m_col:
            c_name = m_col.group(1)
            c_type = m_col.group(2)
            tables[current_table].append({'name': c_name, 'type': c_type, 'attrs': attrs, 'comment': comment_part})
        else:
            print(f"Warning: Could not parse column on line {i+1}: {original_line}")
        continue

sql = []
sql.append("-- Script gerado a partir do Manifesto_DBML.md")
sql.append("-- Atualizado e expandido contendo todas as tabelas, enums e relações.")
sql.append("")
sql.append("CREATE SCHEMA IF NOT EXISTS erp;")
sql.append("CREATE SCHEMA IF NOT EXISTS servicos;")
sql.append("CREATE SCHEMA IF NOT EXISTS cliente;")
sql.append("")

for enum_name, values in enums.items():
    vals_str = ", ".join([f"'{v}'" for v in values if v])
    sql.append(f"DROP TYPE IF EXISTS public.{enum_name} CASCADE;")
    sql.append(f"CREATE TYPE public.{enum_name} AS ENUM ({vals_str});")
sql.append("")

# Adiciona as functions originais no final do SQL
functions_sql = """
-- ==========================================
-- FUNCIONALIDADES (Linguagem Ouro ex: Obter, Inserir)
-- ==========================================

CREATE OR REPLACE FUNCTION erp.obter_auditoria_recente(conta_param uuid)
RETURNS TABLE (tabela text, acao text, obj_id text) AS $$
BEGIN
    RETURN QUERY 
    SELECT table_name, action, record_id 
    FROM erp.auditoria_registros 
    WHERE conta_id = conta_param 
    ORDER BY action_timestamp DESC 
    LIMIT 10;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION servicos.inserir_novo_agendamento(
    p_empresa_id uuid,
    p_cliente_id uuid,
    p_cliente_tipo varchar,
    p_inicio timestamp,
    p_fim timestamp
)
RETURNS uuid AS $$
DECLARE
    novo_id uuid;
BEGIN
    INSERT INTO servicos.agendamentos (empresa_id, cliente_id, cliente_tipo, inicio, fim)
    VALUES (p_empresa_id, p_cliente_id, p_cliente_tipo, p_inicio, p_fim)
    RETURNING id INTO novo_id;
    
    RETURN novo_id;
END;
$$ LANGUAGE plpgsql;
"""

for table_name, cols in tables.items():
    schema = determine_schema(table_name)
    sql.append(f"CREATE TABLE {schema}.{table_name} (")
    col_lines = []
    comments = []
    
    for col in cols:
        attrs_str = ""
        if col['attrs']:
            # split by comma, ignoring commas inside backticks or quotes
            # since we don't have complex string parsing, simple regex:
            attr_list = re.split(r',\s*(?![^`]*`)', col['attrs'])
            for attr in attr_list:
                attr = attr.strip()
                if attr.lower() == 'primary key':
                    attrs_str += " PRIMARY KEY"
                elif attr.lower() == 'not null':
                    attrs_str += " NOT NULL"
                elif attr.lower() == 'unique':
                    attrs_str += " UNIQUE"
                elif attr.lower().startswith('default:'):
                    def_val = attr.split(':', 1)[1].strip()
                    if def_val.startswith('`') and def_val.endswith('`'):
                        def_val = def_val[1:-1]
                    elif def_val.lower() == 'true' or def_val.lower() == 'false':
                        pass
                    elif not def_val.startswith("'") and not def_val.isdigit() and 'now()' not in def_val:
                         pass
                    attrs_str += f" DEFAULT {def_val}"
        
        col_type = col['type']
        if col_type in enums:
            col_type = f"public.{col_type}"
            
        col_def = f"  {col['name']} {col_type}{attrs_str}"
        col_lines.append(col_def)
        
        if col['comment']:
            comment_clean = col['comment'].replace("'", "''")
            comments.append(f"COMMENT ON COLUMN {schema}.{table_name}.{col['name']} IS '{comment_clean}';")
            
    sql.append(",\n".join(col_lines))
    sql.append(");")
    for comment in comments:
        sql.append(comment)
    sql.append("")

for ref in refs:
    fk_schema = determine_schema(ref['fk_table'])
    ref_schema = determine_schema(ref['ref_table'])
    
    if ref['fk_table'] in tables and ref['ref_table'] in tables:
        sql.append(f"ALTER TABLE {fk_schema}.{ref['fk_table']} ADD CONSTRAINT fk_{ref['fk_table']}_{ref['fk_col']} FOREIGN KEY ({ref['fk_col']}) REFERENCES {ref_schema}.{ref['ref_table']}({ref['ref_col']});")

sql.append(functions_sql)

with open(out_path, 'w', encoding='utf-8') as f:
    f.write("\n".join(sql))

print(f"Total Tables Processed: {len(tables)}")
print(f"Total Enums Processed: {len(enums)}")
print(f"Total FKs Processed: {len(refs)}")
print("Sucesso!")
