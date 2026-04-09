import React, { useState } from "react";

/**
 * Code Understanding (mock) — estilo dark igual ao Refator.jsx
 * - Cola código puro
 * - Botão "Analisar" gera resultado mockado
 */
export default function CodeUnderstanding() {
  const [code, setCode] = useState(`function soma(arr){ return arr.reduce((a,b)=>a+b,0) }`);
  const [result, setResult] = useState(null);
  const [busy, setBusy] = useState(false);
  const [title, setTitle] = useState("Code Understanding");

  function analyze(){
    setBusy(true);
    setTimeout(() => {
      setResult({
        languageGuess: "javascript",
        summary: "Função pura que reduz um array e retorna a soma. Complexidade O(n). Sem efeitos colaterais.",
        metrics: { cyclomatic: 2, lines: code.split('\n').length, functions: 1 },
        symbols: [
          { kind: "function", name: "soma", params: ["arr"], returns: "number" },
          { kind: "method", name: "reduce", of: "Array", params: ["callback","initial"] },
        ],
        risks: ["Sem validação de entrada (arr pode não ser array)."],
        recommendations: [
          "Validar tipos de entrada antes de processar.",
          "Adicionar testes para arrays vazios e com valores não numéricos."
        ],
      });
      setBusy(false);
    }, 400);
  }

  return (
    <div className="min-h-screen bg-neutral-950 text-gray-100">
      <div className="px-6 py-6">
        {/* Header de página (igual padrão do Refator.jsx) */}
        <div className="flex items-center justify-between mb-4">
          <div className="flex items-center gap-3">
            <input
              value={title}
              onChange={(e)=>setTitle(e.target.value)}
              className="px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none text-sm"
              placeholder="Título"
            />
            <button
              onClick={analyze}
              disabled={busy || !code.trim()}
              className="px-3 py-2 rounded bg-white/5 hover:bg-white/10 border border-neutral-700 disabled:opacity-50"
            >
              {busy ? "Analisando..." : "Analisar"}
            </button>
          </div>
        </div>

        <div className="grid md:grid-cols-2 gap-4">
          {/* Editor de código */}
          <div className="space-y-2">
            <div className="text-sm opacity-70">Código</div>
            <textarea
              value={code}
              onChange={(e)=>setCode(e.target.value)}
              rows={18}
              spellCheck={false}
              className="w-full px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none font-mono text-sm"
              placeholder="// Cole seu código puro aqui..."
            />
          </div>

          {/* Resultado */}
          <div className="space-y-2">
            <div className="text-sm opacity-70">Análise</div>
            <div className="w-full p-3 rounded bg-neutral-900 border border-neutral-800 text-sm">
              {!result ? (
                <div className="text-gray-400">Resultados aparecerão aqui após a análise.</div>
              ) : (
                <div className="space-y-3">
                  <div><span className="text-gray-400">Linguagem (estimada):</span> <b>{result.languageGuess}</b></div>
                  <div><span className="text-gray-400">Resumo:</span> {result.summary}</div>
                  <div className="flex flex-wrap gap-2">
                    <div className="px-2 py-1 rounded bg-neutral-800 border border-neutral-700">CC: {result.metrics.cyclomatic}</div>
                    <div className="px-2 py-1 rounded bg-neutral-800 border border-neutral-700">Linhas: {result.metrics.lines}</div>
                    <div className="px-2 py-1 rounded bg-neutral-800 border border-neutral-700">Funções: {result.metrics.functions}</div>
                  </div>
                  <div>
                    <div className="text-gray-300 font-medium mb-1">Símbolos</div>
                    <ul className="list-disc ml-5 space-y-1">
                      {result.symbols.map((s, i)=>(
                        <li key={i}>
                          <b>{s.kind}</b>: {s.name}{s.params ? `(${s.params.join(", ")})` : ""}{s.of ? ` de ${s.of}` : ""}{s.returns ? ` → ${s.returns}` : ""}
                        </li>
                      ))}
                    </ul>
                  </div>
                  <div>
                    <div className="text-gray-300 font-medium mb-1">Riscos</div>
                    <ul className="list-disc ml-5 space-y-1">
                      {result.risks.map((r,i)=>(<li key={i}>{r}</li>))}
                    </ul>
                  </div>
                  <div>
                    <div className="text-gray-300 font-medium mb-1">Recomendações</div>
                    <ul className="list-disc ml-5 space-y-1">
                      {result.recommendations.map((r,i)=>(<li key={i}>{r}</li>))}
                    </ul>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
