import { Routes, Route, Link, Navigate, useNavigate } from "react-router-dom";
// src/App.jsx
import React, { useMemo, useState, useEffect } from "react";
import logo from "./assets/logo.png";
import {Settings, MessageSquare, ChevronRight, Search, X, Send, BookOpen, Wrench, Brain} from "lucide-react";
import { LogOut } from "lucide-react";
import { Card, Pill } from "./components/UI";
import SuccessBanner from "./components/SuccessBanner";



// Normaliza a entrada do usuário para o chat
function normalizeUserText(input) {
  let raw = (input ?? "").trim();

  // Extrai código de blocos ``` ``` (com ou sem linguagem)
  const fence = raw.match(/```[a-zA-Z0-9_-]*\n([\s\S]*?)```/);
  if (fence && fence[1]) {
    raw = fence[1];
  }

  // Normalizações: quebras, aspas “inteligentes” e espaços extras
  raw = raw
    .replace(/\r\n/g, "\n")
    .replace(/[“”]/g, '"')
    .replace(/[‘’]/g, "'")
    .trim();

  // Limite de segurança (100k chars)
  const MAX = 100000;
  if (raw.length > MAX) {
    raw = raw.slice(0, MAX);
  }

  return raw;
}
// -------------------- chat dock (in-page) --------------------
function ChatDock({ open, onClose }) {
  const [input, setInput] = React.useState("");
  const [msgs, setMsgs] = React.useState([
    { role: "assistant", text: "Envie o JSON com o campo \"code\" que eu encaminho para o backend e retorno a resposta aqui." }
  ]);
  const [busy, setBusy] = React.useState(false);

  if (!open) return null;

  async function handleSend() {
    const rawText = normalizeUserText(input);
    if (!rawText || busy) return;
    setInput("");
    setMsgs(m => [...m, { role: "user", text: rawText }]);

    // Tenta interpretar o que veio: JSON com { code: "..." } ou texto solto
    let payload;
    try {
      const parsed = JSON.parse(rawText);
      if (typeof parsed === "object" && parsed.code) {
        payload = parsed;
      } else {
        payload = { code: rawText };
      }
    } catch {
      payload = { code: rawText };
    }

    setBusy(true);
    try {
      const base = import.meta.env.VITE_API_URL || "http://localhost:5000";
      const res = await fetch(`${base}/documentation/rawcode`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      const raw = await res.text();
      let data; try { data = JSON.parse(raw); } catch { data = { raw }; }
      let text = ""; 
      if (data) {
  if (typeof data.documentedCode === "string" && data.documentedCode.trim()) {
    text = data.documentedCode;
  } else if (typeof data.documentation === "string" && data.documentation.trim()) {
    text = data.documentation;
  } else if (data.output && typeof data.output.documentedCode === "string") {
    text = data.output.documentedCode;
  } else {
    text = JSON.stringify(data, null, 2);
  }
}
const prefix = data && data.id ? `Salvo (id: ${data.id})

` : "";
      setMsgs(m => [...m, { role: "assistant", text: prefix + ((text) || "[sem corpo na resposta]") }]);
    } catch (err) {
      setMsgs(m => [...m, { role: "assistant", text: "Erro ao chamar backend: " + err.message }]);
    } finally {
      setBusy(false);
    }
  }

  function onKey(e){
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSend();
    }
  }

  return (
    <div className="mt-8 border-t border-neutral-800 pt-4">
      <div className="flex items-center justify-between mb-3">
        <div className="text-sm text-sky-300 font-semibold">Chat com a IA</div>
        <button onClick={onClose} className="p-1 rounded hover:bg-white/5" aria-label="Fechar chat">
          <X size={18} className="text-gray-400" />
        </button>
      </div>

      <div className="space-y-3 max-h-80 overflow-y-auto pr-1">
        {msgs.map((m, i) => (
          <div key={i} className={m.role === "user" ? "text-sm text-gray-200" : "text-sm text-gray-400"}>
            {m.role === "assistant" ? <span className="text-sky-300 mr-2">IA:</span> : <span className="text-emerald-300 mr-2">Você:</span>}
            <pre className="whitespace-pre-wrap break-words inline">{m.text}</pre>
          </div>
        ))}
        {busy && <div className="text-xs text-gray-500">Chamando backend…</div>}
      </div>

      <div className="mt-3 flex items-center gap-2">
        <textarea
          value={input}
          onChange={e => setInput(e.target.value)}
          onKeyDown={onKey}
          rows={3}
          placeholder='Cole aqui seu JSON. Ex: {"code":"..."}'
          className="flex-1 bg-neutral-900 border border-gray-700 rounded-lg px-3 py-3 outline-none focus:border-sky-400 resize-none"
        />
        <button
          onClick={handleSend}
          disabled={busy}
          className="p-3 rounded-lg border border-sky-700/60 bg-sky-900/30 hover:bg-sky-900/50 text-sky-200 disabled:opacity-50"
          title="Enviar"
        >
          <Send size={18} />
        </button>
      </div>
    </div>
  );
}




// -------------------- chat overlay --------------------
function ChatOverlay({ open, onClose }) {
  if (!open) return null;
  return (
    <div className="fixed inset-0 pointer-events-none z-50">
      <div className="absolute bottom-4 right-4 w-[min(720px,calc(100vw-2rem))] h-[60vh] bg-neutral-950/95 border border-neutral-800 rounded-2xl shadow-2xl overflow-hidden pointer-events-auto flex flex-col">
        <header className="h-12 border-b border-neutral-800 px-4 flex items-center justify-between">
          <div className="text-sm text-sky-300 font-semibold">Chat com a IA</div>
          <button onClick={onClose} className="p-1 rounded hover:bg-white/5">
            <X size={18} className="text-gray-400" />
          </button>
        </header>
        <main className="flex-1 p-4 overflow-y-auto space-y-4">
          <div className="text-sm text-gray-400">
            Oi! Recebi o código que você precisa de ajuda! O código diz respeito a uma soma de array.
          </div>
        </main>
        <footer className="p-3 border-t border-neutral-800">
          <div className="flex gap-2">
            <input
              type="text"
              placeholder="Digite sua mensagem..."
              className="flex-1 bg-neutral-900 border border-gray-700 rounded-lg px-3 py-2 outline-none focus:border-sky-400"
            />
            <button className="px-4 py-2 bg-sky-600 hover:bg-sky-500 rounded-lg font-semibold text-white">
              Enviar
            </button>
          </div>
        </footer>
      </div>
    </div>
  );
}


// -------------------- helpers --------------------
const cx = (...c) => c.filter(Boolean).join(" ");

function usernameFromEmail(email = "") {
  const first = (email.split("@")[0] || "").replace(/[._]/g, " ").trim();
  if (!first) return "Usuário";
  return first
    .split(" ")
    .map((p) => p.charAt(0).toUpperCase() + p.slice(1))
    .join(" ");
}

function StatusTag({ status }) {
  const map = {
    "EM PROCESSAMENTO": "bg-amber-200/20 text-amber-300 border-amber-400/30",
    "NÃO INICIADO": "bg-gray-200/10 text-gray-300 border-gray-400/20",
  };
  return (
    <span className={cx("text-[11px] px-2 py-1 rounded border", map[status] || "bg-gray-200/10 text-gray-300 border-gray-400/20")}>
      {status}
    </span>
  );
}

// -------------------- página: seleção de repositório --------------------
function RepoSelect({ onSelect, onOpenChat, chatOpen, onCloseChat }) {
  const email = localStorage.getItem("userEmail") || "";
  const username = usernameFromEmail(email);

  // dados fake (pode vir de API)
  const [query, setQuery] = useState("");
  const repos = [
    { id: "r1", name: "Nome repositório 1", status: "EM PROCESSAMENTO" },
    { id: "r2", name: "Nome repositório 2", status: "NÃO INICIADO" },
    { id: "r3", name: "Nome repositório 3", status: "NÃO INICIADO" },
    { id: "r4", name: "Nome repositório 4", status: "NÃO INICIADO" },
  ];

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    if (!q) return repos;
    return repos.filter((r) => r.name.toLowerCase().includes(q));
  }, [query]);

  const processingCount = repos.filter((r) => r.status === "EM PROCESSAMENTO").length;
  const notStartedCount = repos.filter((r) => r.status === "NÃO INICIADO").length;

  return (
    <div className="min-h-screen bg-neutral-950 text-gray-100">
      {/* Header escuro do mock */}
      <header className="h-16 border-b border-neutral-800 px-6 flex items-center justify-between">
        <div className="flex items-center gap-2 font-semibold"><img src={logo} alt="Henry.AI" className="h-10 object-contain" />
        </div>
        <div className="flex items-center gap-3 text-sm">
          <span className="text-gray-300">Olá, <strong className="font-semibold">{username}</strong></span>
          <img
            alt=""
            className="w-8 h-8 rounded-full border border-neutral-700 object-cover"
            src={`https://api.dicebear.com/8.x/identicon/svg?seed=${encodeURIComponent(username)}`}
          />
        </div>
      </header>

      <div className="grid grid-cols-[260px_1fr] gap-6 px-6 py-6">
        {/* sidebar minimal como no mock */}
        <aside className="rounded-2xl bg-neutral-900/40 border border-neutral-800 p-2 flex flex-col gap-1">
          <button className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-gray-200">
            <Settings size={18} />
            <span>Configurações</span>
          </button>
          <button onClick={onOpenChat} className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-gray-200">
            <MessageSquare size={18} />
            <span>Chat com a IA</span>
          </button>
      <Link to="/docs" className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-gray-200">
        <BookOpen size={18} />
        <span>Documentações</span>
      </Link>

          <Link to="/refactor" className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5">
            <Wrench size={18} />
            <span>Refatoração</span>
          </Link>
          <Link to="/code-understanding" className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5">
            <Brain size={18} />
            <span>Code Understanding</span>
          </Link>
<div className="mt-auto pt-3 text-[11px] text-gray-500 px-3">
          <Link
            to="/logout"
            className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-red-400 mt-auto"
          >
            <LogOut size={18} />
            <span>Sair</span>
          </Link>

            © Henry.AI
          </div>
        </aside>

        {/* conteúdo principal */}
        <section className="rounded-2xl bg-neutral-900/40 border border-neutral-800 p-4">
          <h2 className="text-lg font-semibold mb-3">
            Selecione um repositório para mais informações:
          </h2>

          {/* busca + tags de filtro à direita */}
          <div className="flex items-center justify-between gap-3 mb-3">
            <div className="flex items-center bg-neutral-900 border border-neutral-800 rounded-lg px-3 w-full max-w-xl">
              <Search size={16} className="text-gray-500" />
              <input
                placeholder="Procurar um repositório"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="bg-transparent outline-none px-2 py-2 text-sm w-full text-gray-100 placeholder:text-gray-500"
              />
            </div>

            <div className="flex items-center gap-4 text-xs text-gray-400">
              <span className="whitespace-nowrap">
                Em processamento (<span className="text-gray-200">{processingCount}</span>)
              </span>
              <span className="whitespace-nowrap">
                Não iniciados (<span className="text-gray-200">{notStartedCount}</span>)
              </span>
            </div>
          </div>

          {/* "tabela" */}
          <div className="rounded-lg border border-neutral-800 overflow-hidden">
            <div className="grid grid-cols-[1fr_160px] items-center text-xs uppercase tracking-wide bg-neutral-900/60 border-b border-neutral-800 px-4 py-2 text-gray-400">
              <span>Nome</span>
              <span className="text-right">Status</span>
            </div>

            <div className="divide-y divide-neutral-800">
              {filtered.map((r) => (
                <button
                  key={r.id}
                  onClick={() => onSelect(r)}
                  className="w-full grid grid-cols-[1fr_160px_28px] items-center text-left px-4 py-3 hover:bg-white/5 transition"
                >
                  <div className="text-sm">{r.name}</div>
                  <div className="flex justify-end"><StatusTag status={r.status} /></div>
                  <ChevronRight size={16} className="justify-self-end text-gray-500" />
                </button>
              ))}
              {!filtered.length && (
                <div className="px-4 py-6 text-sm text-gray-500">Nenhum repositório encontrado.</div>
              )}
            </div>
          </div>
        
          <ChatDock open={chatOpen} onClose={onCloseChat} />
        </section>
      </div>
    </div>
  );
}

// -------------------- tela do repositório após seleção --------------------
function RepoDashboard({ repo, onBack, onOpenChat, chatOpen, onCloseChat }) {
  const navigate = useNavigate();

  const email = localStorage.getItem("userEmail") || "";
  const username = usernameFromEmail(email);

  // ---- estado de modo: "view" (resumo) | "analyze" (tela do mock novo) ----
  const [mode, setMode] = useState("view");

  // sucesso ao gerar testes
  const [showSuccess, setShowSuccess] = useState(false);
  const handleGerarTestes = () => {
    setShowSuccess(true);
    clearTimeout(window.__hideSuccessTimer__);
    window.__hideSuccessTimer__ = setTimeout(() => setShowSuccess(false), 4000);
  };

  // dados mocks (troque por API)
  const percentRefactor = 70;
  const complexity = "Alta";
  const language = "Java";
  const migratingTo = "Python";

  const findings = [
    {
      id: "f1",
      title: "LINHA 1559 DO CÓDIGO ANTERIOR",
      before: `public class SumExample {
  public static void main(String[] args) {
    int[] numbers = {1, 2, 3, 4, 5};
    int sum = 0;
    for (int num : numbers) {
      sum += num;
    }
    System.out.println("The sum of the numbers is: " + sum);
  }
}`,
      after: `# This program calculates the sum of a list of numbers
numbers = [1, 2, 3, 4, 5]
sum_value = 0
for num in numbers:
    sum_value += num
print(f"The sum of the numbers is: {sum_value}")`,
    },
    {
      id: "f2",
      title: "FUNÇÃO LEGACY: CALCULAR TOTAL",
      before: `public static double total(double[] values) {
  double t = 0;
  for (int i = 0; i < values.length; i++) {
    t += values[i];
  }
  return t;
}`,
      after: `def total(values: list[float]) -> float:
    return sum(values)`,
    },
  ];

  const codeBefore = `// This program demonstrates calculating the area...
public class GeometryCalculator { ... }`;
  const codeAfter = `import math
# Circle calculation ...
print(f"Summary -> ...")`;

  const ProgressBar = ({ value }) => (
    <div className="h-2 w-full rounded bg-neutral-800 overflow-hidden">
      <div className="h-full bg-sky-500" style={{ width: `${Math.min(100, Math.max(0, value))}%` }} />
    </div>
  );

  const CodeCard = ({ header, code, footer, right }) => (
    <div className="rounded-xl border border-neutral-800 bg-neutral-950/50">
      <div className="flex items-center justify-between text-xs text-gray-400 border-b border-neutral-800 px-3 py-2">
        <span>{header}</span>
        {right}
      </div>
      <pre className="p-3 text-[12px] leading-5 text-gray-100 overflow-auto max-h-[320px] whitespace-pre-wrap">
{code}
      </pre>
      {footer}
    </div>
  );

  const ActionBar = ({ children }) => (
    <div className="flex flex-wrap gap-2 px-3 py-2 border-t border-neutral-800">
      {children}
    </div>
  );

  const Header = (
    <header className="h-16 border-b border-neutral-800 px-6 flex items-center justify-between">
      <div className="flex items-center gap-2 font-semibold"><img src={logo} alt="Henry.AI" className="h-10 object-contain" />
      </div>

      <div className="flex items-center gap-2">
        <button
          onClick={onBack}
          className="text-xs font-medium border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 rounded px-3 py-2"
        >
          Voltar para repositórios
        </button>
        {mode === "analyze" ? (
          <button
            onClick={() => setMode("view")}
            className="text-xs font-medium border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 rounded px-3 py-2"
          >
            Voltar para resumo
          </button>
        ) : (
          <a
            href="../public/documentacao_tecnica_legacy_ai.pdf"
            target="_blank"
            rel="noopener noreferrer"
            className="text-xs font-medium border border-sky-700/60 bg-sky-900/30 hover:bg-sky-900/50 text-sky-200 rounded px-3 py-2"
          >
            DOCUMENTAÇÃO EM PDF
          </a>

        )}
        <div className="mx-2 w-px h-5 bg-neutral-800" />
        <span className="text-sm text-gray-300">
          Olá, <strong className="font-semibold">{username}</strong>
        </span>
        <img
          alt=""
          className="w-8 h-8 rounded-full border border-neutral-700 object-cover"
          src={`https://api.dicebear.com/8.x/identicon/svg?seed=${encodeURIComponent(username)}`}
        />
      </div>
    </header>
  );
  const handleOpenAnalytics = () => navigate('/analytics');

  const ActionsBar = (
    <div className="px-6 pt-4">
      <div className="flex flex-wrap gap-3">
        <button
          className="text-sm font-semibold rounded border border-sky-700/40 bg-sky-900/30 hover:bg-sky-900/50 text-sky-200 px-3 py-2"
          onClick={onOpenChat}
        >
          USAR ESSE REPOSITÓRIO NO CHAT
        </button>
        <button
          className="text-sm font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-3 py-2"
          onClick={() => setMode("analyze")}
        >
          ANALISAR NOVO CÓDIGO
        </button>
        <button
          className="text-sm font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-3 py-2"
          onClick={handleGerarTestes}
        >
          GERAR TESTES
        </button>
        <button
          className="text-sm font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-3 py-2"
          onClick={handleOpenAnalytics}
        >
          VER GRÁFICOS & DADOS
        </button>
      </div>
    </div>
  );


  const Sidebar = (
    <aside className="rounded-2xl bg-neutral-900/40 border border-neutral-800 p-2 flex flex-col gap-1">
      <button className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-gray-200">
        <Settings size={18} />
        <span>Configurações</span>
      </button>

      <Link to="/docs" className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-gray-200">
        <BookOpen size={18} />
        <span>Documentações</span>
      </Link>
      <button onClick={onOpenChat} className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-gray-200">
        <MessageSquare size={18} />
        <span>Chat com a IA</span>
      </button>
      <div className="mt-auto pt-3 text-[11px] text-gray-500 px-3">© Henry.AI</div>
      <Link
        to="/logout"
        className="flex items-center gap-3 px-3 py-2 rounded-lg hover:bg-white/5 text-red-400"
      >
        <LogOut size={18} />
        <span>Sair</span>
          </Link>
    </aside>
  );

  if (mode === "view") {
    return (
      <div className="min-h-screen bg-neutral-950 text-gray-100">
        {Header}
        {ActionsBar}
        <div className="grid grid-cols-[260px_1fr] gap-6 px-6 py-4">
          {Sidebar}

          <section className="rounded-2xl bg-neutral-900/40 border border-neutral-800 p-5">
            <div className="flex items-start justify-between gap-6">
              <div className="flex-1">
                <h1 className="text-xl font-bold">{repo?.name}</h1>
                <div className="mt-3">
                  <ProgressBar value={percentRefactor} />
                  <div className="text-xs text-gray-400 mt-1">{percentRefactor}% REFATORADO</div>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-6 text-sm">
                <div>
                  <div className="text-gray-400 uppercase text-xs">COMPLEXIDADE</div>
                  <div className="mt-1">{complexity}</div>
                </div>
                <div>
                  <div className="text-gray-400 uppercase text-xs">LINGUAGEM</div>
                  <div className="mt-1">
                    {language}{" "}
                    <span className="text-xs text-gray-400">(Atualmente refatorando em {migratingTo})</span>
                  </div>
                </div>
              </div>
            </div>

            <div className="mt-6">
              <div className="text-sm text-gray-400 uppercase">RESUMO DO REPOSITÓRIO</div>
              <p className="mt-1 text-sm text-gray-300">
                Resumo rápido criado com base na análise feita pela IA.
              </p>

              <div className="hidden mt-4 flex flex-wrap gap-3">
                <button
                  className="text-sm font-semibold rounded border border-sky-700/40 bg-sky-900/30 hover:bg-sky-900/50 text-sky-200 px-3 py-2"
                  onClick={() => alert("Abrir chat com repo selecionado")}
                >
                  USAR ESSE REPOSITÓRIO NO CHAT
                </button>
                <button
                  className="text-sm font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-3 py-2"
                  onClick={() => setMode("analyze")}
                >
                  ANALISAR NOVO CÓDIGO
                </button>
                <button
                  className="text-sm font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-3 py-2"
                  onClick={() => alert("Gerar testes")}
                >
                  GERAR TESTES
                </button>
              </div>
            </div>

            <div className="mt-6 grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div className="rounded-xl border border-neutral-800 bg-neutral-950/50">
                <div className="text-xs text-gray-400 border-b border-neutral-800 px-3 py-2">Trecho (Antes)</div>
                <pre className="p-3 text-[12px] leading-5 text-gray-100 overflow-auto">
{codeBefore}
                </pre>
              </div>

              <div className="rounded-xl border border-neutral-800 bg-neutral-950/50">
                <div className="text-xs text-gray-400 border-b border-neutral-800 px-3 py-2">Trecho refatorado (Depois)</div>
                <pre className="p-3 text-[12px] leading-5 text-gray-100 overflow-auto">
{codeAfter}
                </pre>
              </div>
            </div>

            <div className="mt-6">
              <div className="text-sm text-gray-400 uppercase mb-2">Sugestões de melhoria</div>
              <div className="rounded-xl border border-neutral-800 bg-neutral-950/50 p-4 text-sm text-gray-200">
                <p className="mb-2">
                  Explicação do trecho/função e sugestões de melhoria…
                </p>
              </div>
            </div>
          
            <ChatDock open={chatOpen} onClose={onCloseChat} />
          </section>
        </div>
      </div>
    );
  }

  // Modo "analyze": layout do mock
  return (
    <div className="min-h-screen bg-neutral-950 text-gray-100">
      {Header}
      {ActionsBar}
      <div className="grid grid-cols-[260px_1fr] gap-6 px-6 py-4">
        {Sidebar}

        <section className="rounded-2xl bg-neutral-900/40 border border-neutral-800 p-5">
          <div className="flex items-start justify-between gap-6">
            <div className="flex-1">
              <h1 className="text-xl font-bold">{repo?.name}</h1>
              <div className="mt-3">
                <ProgressBar value={percentRefactor} />
                <div className="text-xs text-gray-400 mt-1">{percentRefactor}% REFATORADO</div>
              </div>
            </div>
            <div className="grid grid-cols-2 gap-6 text-sm">
              <div>
                <div className="text-gray-400 uppercase text-xs">COMPLEXIDADE</div>
                <div className="mt-1">{complexity}</div>
              </div>
              <div>
                <div className="text-gray-400 uppercase text-xs">LINGUAGEM</div>
                <div className="mt-1">
                  {language} <span className="text-xs text-gray-400">(Atualmente refatorando em {migratingTo})</span>
                </div>
              </div>
            </div>
          </div>

          {/* lista de achados */}
          <div className="mt-6 grid grid-cols-1 gap-6">
            {findings.map((f) => (
              <div key={f.id} className="grid lg:grid-cols-2 gap-6">
                <div className="rounded-xl border border-neutral-800 bg-neutral-950/50">
                  <div className="text-xs text-gray-400 border-b border-neutral-800 px-3 py-2">{f.title}</div>
                  <pre className="p-3 text-[12px] leading-5 text-gray-100 overflow-auto max-h-[320px] whitespace-pre-wrap">
{f.before}
                  </pre>
                  <div className="flex flex-wrap gap-2 px-3 py-2 border-t border-neutral-800">
                    <button
                      className="text-[12px] font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-2.5 py-1.5"
                      onClick={() => alert(`Inserir trecho (Antes) - ${f.id}`)}
                    >
                      INSERIR TRECHO NO REPOSITÓRIO
                    </button>
                    <button
                      className="text-[12px] font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-2.5 py-1.5"
                      onClick={() => alert(`Editar trecho (Antes) - ${f.id}`)}
                    >
                      EDITAR
                    </button>
                  </div>
                </div>

                <div className="rounded-xl border border-neutral-800 bg-neutral-950/50">
                  <div className="text-xs text-gray-400 border-b border-neutral-800 px-3 py-2">Trecho refatorado (Depois)</div>
                  <pre className="p-3 text-[12px] leading-5 text-gray-100 overflow-auto max-h-[320px] whitespace-pre-wrap">
{f.after}
                  </pre>
                  <div className="flex flex-wrap gap-2 px-3 py-2 border-t border-neutral-800">
                    <button
                      className="text-[12px] font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-2.5 py-1.5"
                      onClick={() => alert(`Inserir trecho (Depois) - ${f.id}`)}
                    >
                      INSERIR TRECHO NO REPOSITÓRIO
                    </button>
                    <button
                      className="text-[12px] font-semibold rounded border border-neutral-700 bg-neutral-800 hover:bg-neutral-700 text-gray-100 px-2.5 py-1.5"
                      onClick={() => alert(`Editar trecho (Depois) - ${f.id}`)}
                    >
                      EDITAR
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
          
          <ChatDock open={chatOpen} onClose={onCloseChat} />
            <SuccessBanner show={showSuccess} onClose={() => setShowSuccess(false)}>
              <strong>Sucesso:</strong> Código refatorado realiza função de código anterior.
            </SuccessBanner>
        </section>
      </div>
    </div>
  

);
}

// -------------------- AppShell --------------------
export function AppShell() {
  const [showChat, setShowChat] = useState(false);
  const navigate = useNavigate();

  const [selectedRepo, setSelectedRepo] = useState(null);
  const [isDark, setIsDark] = useState(() => {
    return localStorage.getItem("theme") === "dark" || window.matchMedia?.("(prefers-color-scheme: dark)").matches;
  });

  useEffect(() => {
    const root = document.documentElement;
    if (isDark) { root.classList.add("dark"); localStorage.setItem("theme", "dark"); }
    else { root.classList.remove("dark"); localStorage.setItem("theme", "light"); }
  }, [isDark]);

  const handleLogout = () => {
    localStorage.removeItem("onboarded");
    localStorage.removeItem("userEmail");
    navigate("/login", { replace: true });
  };

  if (!selectedRepo) {
    return <>
    <RepoSelect onSelect={(r) => { setSelectedRepo(r); }} onOpenChat={() => setShowChat(true)} chatOpen={showChat} onCloseChat={() => setShowChat(false)} />
  </>;
  }

  return <>
    <RepoDashboard repo={selectedRepo} onBack={() => setSelectedRepo(null)} onOpenChat={() => setShowChat(true)} chatOpen={showChat} onCloseChat={() => setShowChat(false)} />
    
  </>;
}

import Docs from "./pages/Docs";
import DocView from "./pages/DocView";

export function ChatHome() {
  return (
    <div className="min-h-screen bg-neutral-900 text-neutral-100">
      <div className="max-w-4xl mx-auto p-4 pb-2 flex items-center justify-between">
        <div className="font-semibold">Henry.AI</div>
        <div className="flex items-center gap-2">
          <Link to="/" className="text-xs px-2 py-1 bg-white/10 hover:bg-white/20 rounded">Chat</Link>
          <Link to="/docs" className="text-xs px-2 py-1 bg-white/10 hover:bg-white/20 rounded">Documentações</Link>
        </div>
      </div>
      
    </div>
  );
}

export default function App() {
  return (
    <div className="min-h-screen bg-neutral-900 text-neutral-100">
      <div className="max-w-4xl mx-auto p-4 pb-2 flex items-center justify-between">
        <div className="font-semibold">Henry.AI</div>
        <div className="flex items-center gap-2">
          <Link to="/" className="text-xs px-2 py-1 bg-white/10 hover:bg-white/20 rounded">Chat</Link>
          <Link to="/docs" className="text-xs px-2 py-1 bg-white/10 hover:bg-white/20 rounded">Documentações</Link>
        </div>
      </div>
      
    </div>
  );
}