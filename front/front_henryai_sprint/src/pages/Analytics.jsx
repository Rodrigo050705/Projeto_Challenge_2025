import React from 'react';

/** Dashboard/Analytics (mock) — gráficos e dados */
export default function Analytics() {
  const donut = (pct) => ({
    background: `conic-gradient(rgb(56 189 248) ${pct*3.6}deg, rgb(38 38 38) 0)`,
  });
  return (
    <div className="min-h-screen bg-neutral-950 text-gray-100">
      <div className="px-6 py-6">
        <div className="flex items-center justify-between mb-4">
          <h1 className="text-xl font-semibold">Dashboard</h1>
          <div className="text-xs opacity-60">Mock de visualização</div>
        </div>

        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4 flex items-center gap-4">
            <div className="w-16 h-16 rounded-full grid place-items-center" style={donut(60)}>
              <div className="w-12 h-12 rounded-full bg-neutral-950 grid place-items-center text-sm">60%</div>
            </div>
            <div>
              <div className="text-sm opacity-70">Analisar Novo</div>
              <div className="text-xs text-gray-400">progresso do pipeline</div>
            </div>
          </div>

          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4 flex items-center gap-4">
            <div className="w-16 h-16 rounded-full grid place-items-center" style={donut(40)}>
              <div className="w-12 h-12 rounded-full bg-neutral-950 grid place-items-center text-sm">40%</div>
            </div>
            <div>
              <div className="text-sm opacity-70">Refatorado</div>
              <div className="text-xs text-gray-400">módulos finalizados</div>
            </div>
          </div>

          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4">
            <div className="text-3xl font-bold">5</div>
            <div className="text-sm opacity-70">Testes Automatizados</div>
          </div>

          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4">
            <div className="text-3xl font-bold">3</div>
            <div className="text-sm opacity-70">Vulnerabilidades Encontradas</div>
          </div>
        </div>

        <div className="grid md:grid-cols-2 gap-4 mt-4">
          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4">
            <div className="flex items-center justify-between mb-2">
              <div className="font-medium">Resumo Funcional</div>
            </div>
            <div className="grid grid-cols-2 gap-3 text-sm">
              <div className="rounded-lg bg-neutral-800/50 border border-neutral-700 p-3">
                <div className="opacity-70">Complexidade</div>
                <div className="text-lg font-semibold">Alta</div>
              </div>
              <div className="rounded-lg bg-neutral-800/50 border border-neutral-700 p-3">
                <div className="opacity-70">Acoplamento</div>
                <div className="text-lg font-semibold">Elevado</div>
              </div>
            </div>
          </div>

          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4">
            <div className="flex items-center justify-between mb-2">
              <div className="font-medium">Mapa de Dependências</div>
            </div>
            <div className="grid grid-cols-2 gap-3 text-sm">
              <div className="rounded-lg bg-neutral-800/50 border border-neutral-700 p-3">
                <div className="opacity-70">Linguagem</div>
                <div className="text-lg font-semibold">COBOL</div>
              </div>
              <div className="rounded-lg bg-neutral-800/50 border border-neutral-700 p-3">
                <div className="opacity-70">Coupling</div>
                <div className="text-lg font-semibold">85%</div>
              </div>
            </div>
          </div>
        </div>

        <div className="grid md:grid-cols-2 gap-4 mt-4">
          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4">
            <div className="font-medium mb-2">Sugestão de Refatoração</div>
            <div className="text-sm text-gray-300">Priorizar extração de módulos com alto acoplamento e criar testes para validadores de entrada.</div>
            <div className="mt-3 flex gap-2">
              <button className="px-3 py-2 rounded bg-white/5 hover:bg-white/10 border border-neutral-700 text-sm">Sugestão de Refatoração</button>
              <button className="px-3 py-2 rounded bg-white/5 hover:bg-white/10 border border-neutral-700 text-sm">Gerar Testes</button>
            </div>
          </div>
          <div className="rounded-2xl bg-neutral-900 border border-neutral-800 p-4">
            <div className="font-medium mb-2">Copilot</div>
            <div className="text-sm text-gray-300">Este módulo possui alta complexidade. Deseja convertê-lo para Java com testes unitários?</div>
            <div className="mt-3 flex gap-2">
              <button className="px-3 py-2 rounded bg-sky-900/30 hover:bg-sky-900/50 border border-sky-700/60 text-sky-200 text-sm">Converter</button>
              <button className="px-3 py-2 rounded bg-white/5 hover:bg-white/10 border border-neutral-700 text-sm">Ver Código Gerado</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
