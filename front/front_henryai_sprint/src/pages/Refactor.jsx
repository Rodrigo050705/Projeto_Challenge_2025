import React, { useState } from 'react';

/**
 * Refatoração (mock) — foco em melhorar o mesmo código (mesma linguagem).
 * Remove a ideia de "linguagem origem/alvo" e adiciona objetivos/checagens.
 */
export default function Refactor() {
  const base = import.meta.env.VITE_API_URL || 'http://localhost:5000';

  // Estados
  const [title, setTitle] = useState('Refatoração');
  const [original, setOriginal] = useState('function soma(arr){return arr.reduce((a,b)=>a+b,0)}');
  const [refactored, setRefactored] = useState('');
  const [busy, setBusy] = useState(false);

  // Controles de objetivo/checklist
  const [preset, setPreset] = useState('legibilidade');
  const [applySOLID, setApplySOLID] = useState(true);
  const [removeDead, setRemoveDead] = useState(true);
  const [extractFuncs, setExtractFuncs] = useState(true);
  const [addTestsHint, setAddTestsHint] = useState(true);
  const [immutability, setImmutability] = useState(false);

  // Mock de métricas
  const [metrics, setMetrics] = useState(null);
  const [plan, setPlan] = useState([]);

  // Gera um "refactor" mockado
  function doRefactor(){
    if (!original.trim()) { alert('Cole o código original.'); return; }
    setBusy(true);

    // Simular análise e resultado
    setTimeout(() => {
      let code = original;

      // Mocks simples
      code = code.replace(/var\s/g, 'let ');
      code = code.replace(/return\s+([^;\n]+);?/g, (m, expr) => `return ${expr};`);
      code = code.replace(/\{\s*return\s+/g, '{ return ');
      code = code.replace(/\)\s*\{\s*return\s+/g, '){ return ');

      // Caso específico do exemplo
      code = code.replace(/function\s+soma\(arr\)\{return\s+arr\.reduce\(\(a,b\)=>a\+b,0\)\}/,
        'function soma(arr) {\n  if (!Array.isArray(arr)) throw new TypeError("arr deve ser array");\n  return arr.reduce((acc, n) => acc + Number(n || 0), 0);\n}');

      const planned = [
        applySOLID && 'Aplicar SRP: separar validação de entrada e cálculo.',
        removeDead && 'Remover código morto/variáveis não usadas.',
        extractFuncs && 'Extrair funções pequenas e nomeadas.',
        immutability && 'Evitar mutação compartilhada; preferir const/estruturas imutáveis.',
        addTestsHint && 'Sugerir testes de unidade para casos vazios e não numéricos.',
      ].filter(Boolean);

      const m = {
        before: { cyclomatic: 3, lines: original.split('\n').length },
        after: { cyclomatic: 2, lines: code.split('\n').length },
      };

      setRefactored(code);
      setPlan(planned);
      setMetrics(m);
      setBusy(false);
    }, 450);
  }

  // Mock "salvar como doc"
  async function saveAsDoc(){
    if (!refactored.trim()) { alert('Refatore antes de salvar.'); return; }
    try{
      const payload = {
        title,
        type: 'refactor-doc',
        plan,
        metrics,
        original,
        refactored,
      };
      const res = await fetch(`${base}/documentation/rawcode`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });
      const raw = await res.text();
      let data; try { data = JSON.parse(raw); } catch { data = { raw }; }
      alert('Mock: documentação enviada. ' + (data && (data.id ? `id: ${data.id}` : '')));
    }catch(err){
      alert('Erro ao salvar documentação: ' + err.message);
    }
  }

  return (
    <div className="min-h-screen bg-neutral-950 text-gray-100">
      <div className="px-6 py-6">
        {/* Header */}
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-3">
            <input
              value={title}
              onChange={e=>setTitle(e.target.value)}
              className="px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none text-sm"
              placeholder="Título"
            />
            <button
              onClick={saveAsDoc}
              disabled={!refactored.trim()}
              className="px-3 py-2 rounded bg-white/5 hover:bg-white/10 border border-neutral-700 disabled:opacity-50"
            >
              Salvar como documentação
            </button>
          </div>
        </div>

        {/* Controles de objetivo/checagens */}
        <div className="mb-4 flex flex-wrap items-center gap-3">
          <select
            value={preset}
            onChange={e=>setPreset(e.target.value)}
            className="px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none text-sm"
            title="Objetivo de refatoração"
          >
            <option value="legibilidade">Legibilidade</option>
            <option value="performance">Performance</option>
            <option value="manutenibilidade">Manutenibilidade</option>
            <option value="seguranca">Segurança</option>
          </select>

          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={applySOLID} onChange={e=>setApplySOLID(e.target.checked)} />
            Aplicar SRP/SOLID
          </label>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={removeDead} onChange={e=>setRemoveDead(e.target.checked)} />
            Remover código morto
          </label>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={extractFuncs} onChange={e=>setExtractFuncs(e.target.checked)} />
            Extrair funções
          </label>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={immutability} onChange={e=>setImmutability(e.target.checked)} />
            Imutabilidade
          </label>
          <label className="flex items-center gap-2 text-sm">
            <input type="checkbox" checked={addTestsHint} onChange={e=>setAddTestsHint(e.target.checked)} />
            Sugerir testes
          </label>

          <button
            onClick={doRefactor}
            disabled={busy || !original.trim()}
            className="ml-auto px-3 py-2 rounded bg-white/5 hover:bg-white/10 border border-neutral-700 disabled:opacity-50"
          >
            {busy ? 'Refatorando...' : 'Refatorar'}
          </button>
        </div>

        {/* Editor side-by-side */}
        <div className="grid md:grid-cols-2 gap-4">
          <div className="space-y-2">
            <div className="text-sm opacity-70">Código original</div>
            <textarea
              value={original}
              onChange={e=>setOriginal(e.target.value)}
              rows={18}
              spellCheck={false}
              className="w-full px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none font-mono text-sm"
              placeholder="// Cole aqui o código antigo..."
            />
          </div>
          <div className="space-y-2">
            <div className="text-sm opacity-70">Código refatorado</div>
            <textarea
              value={refactored}
              onChange={e=>setRefactored(e.target.value)}
              rows={18}
              spellCheck={false}
              className="w-full px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none font-mono text-sm"
              placeholder="// O resultado aparecerá aqui e pode ser editado..."
            />
          </div>
        </div>

        {/* Plano + Métricas */}
        <div className="grid md:grid-cols-2 gap-4 mt-4">
          <div className="p-3 rounded bg-neutral-900 border border-neutral-800">
            <div className="text-sm opacity-70 mb-2">Plano de refatoração</div>
            {!plan.length ? (
              <div className="text-sm text-gray-400">Nenhum item ainda. Marque opções e clique em Refatorar.</div>
            ) : (
              <ul className="list-disc ml-5 space-y-1 text-sm">
                {plan.map((p,i)=>(<li key={i}>{p}</li>))}
              </ul>
            )}
          </div>
          <div className="p-3 rounded bg-neutral-900 border border-neutral-800">
            <div className="text-sm opacity-70 mb-2">Métricas</div>
            {!metrics ? (
              <div className="text-sm text-gray-400">Sem métricas ainda.</div>
            ) : (
              <div className="text-sm space-y-1">
                <div>CC antes: <b>{metrics.before.cyclomatic}</b> · linhas: <b>{metrics.before.lines}</b></div>
                <div>CC depois: <b>{metrics.after.cyclomatic}</b> · linhas: <b>{metrics.after.lines}</b></div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
