import { Pencil, Trash2, Save, X } from 'lucide-react';
import React from "react";
import { Link, useNavigate } from "react-router-dom";
import { useEffect, useMemo, useState } from "react";

export default function Docs() {
  const navigate = useNavigate();
  const goBackToSummary = () => {
    const repoId = localStorage.getItem('selectedRepoId');
    if (repoId) navigate(`/repo/${repoId}`);
    else navigate('/app');
  };
  const [query, setQuery] = useState("");
  const [language, setLanguage] = useState("");
  const [func, setFunc] = useState("");
  const [page, setPage] = useState(0);
  const [limit, setLimit] = useState(10);
  const [total, setTotal] = useState(0);
  const [items, setItems] = useState([]);
  const [editing, setEditing] = useState(null);
  const [busy, setBusy] = useState(false);
  const base = import.meta.env.VITE_API_URL || "http://localhost:5000";

  const params = useMemo(() => {
    const sp = new URLSearchParams();
    if (query) sp.set("query", query);
    if (language) sp.set("language", language);
    if (func) sp.set("function", func);
    sp.set("skip", String(page * limit));
    sp.set("limit", String(limit));
    return sp.toString();
  }, [query, language, func, page, limit]);

  async function load() {
    const res = await fetch(`${base}/documentation?${params}`);
    const data = await res.json().catch(() => ({ items: [], total: 0 }));
    const arr = data.items || data || [];
    setItems(Array.isArray(arr) ? arr : []);
    setTotal(data.total ?? arr.length ?? 0);
  }

  useEffect(() => { load(); /* eslint-disable-next-line */ }, [params]);

  const getId = (d) => d?.id || d?._id?.$oid || d?._id || d?.Id || "";
  const fmtDate = (s) => {
    try { return new Date(s).toLocaleString(); } catch { return s || ""; }
  };

  const languages = ["", "python", "csharp", "javascript", "java", "unknown"];

  
  async function removeDoc(id) {
    if (!confirm("Excluir esta documentação?")) return;
    try {
      setBusy(true);
      await fetch(`${base}/documentation/${id}`, { method: "DELETE", cache: 'no-store' });
      const res = await fetch(`${base}/documentation?${params}`, { cache: 'no-store' });
      const data = await res.json().catch(()=>({items:[]}));
      setItems(data.items || data.data || []);
    } finally {
      setBusy(false);
    }
  }

  function beginEdit(doc) {
    setEditing({
      _id: doc._id,
      title: doc.title || "",
      language: doc.language || "",
      function: doc.function || "",
      content: (doc.content) || doc.documentedCode || "",
    });
  }

  function cancelEdit(){ setEditing(null); }

  async function saveEdit() {
    if (!editing?._id) return;
    try {
      setBusy(true);
      const payload = {
        title: editing.title,
        language: editing.language,
        function: editing.function,
        content: editing.content,
        documentedCode: editing.content,
      };
      await fetch(`${base}/documentation/${editing._id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      setEditing(null);
      const res = await fetch(`${base}/documentation?${params}`, { cache: 'no-store' });
      const data = await res.json().catch(()=>({items:[]}));
      setItems(data.items || data.data || []);
    } finally {
      setBusy(false);
    }
  }

  return (<div className="min-h-screen bg-neutral-900 text-neutral-200 p-4">
  <div className="mb-4 flex justify-start"><button onClick={goBackToSummary} className="px-3 py-2 rounded border border-neutral-700 hover:bg-white/10 text-sm">← Voltar ao resumo</button></div>
      <div className="max-w-5xl mx-auto space-y-4">
        <h1 className="text-xl font-semibold">Documentações</h1>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-2">
          <input value={query} onChange={e=>{setPage(0);setQuery(e.target.value);}}
                 placeholder="Buscar no título/conteúdo..." className="bg-white/5 rounded px-3 py-2 outline-none"/>
          <select value={language} onChange={e=>{setPage(0);setLanguage(e.target.value);}}
                  className="bg-white/5 rounded px-3 py-2">
            {languages.map(l => <option key={l} value={l}>{l || "Todas as linguagens"}</option>)}
          </select>
          <input value={func} onChange={e=>{setPage(0);setFunc(e.target.value);}}
                 placeholder="Função (ex.: cálculo de frete)" className="bg-white/5 rounded px-3 py-2 outline-none"/>
          <div className="flex items-center gap-2">
            <button onClick={()=>{setPage(0); load();}} className="px-3 py-2 rounded bg-white/10 hover:bg-white/20">Buscar</button>
            <button onClick={()=>{setQuery("");setLanguage("");setFunc("");setPage(0);}}
                    className="px-3 py-2 rounded bg-white/5 hover:bg-white/10">Limpar</button>
          </div>
        </div>

        <div className="text-sm opacity-75">Total: {total}</div>

        <div className="divide-y divide-white/10 rounded-lg overflow-hidden border border-white/10">
          {items.map(d => (
            <div key={getId(d)} className="p-3 flex items-start gap-3 hover:bg-white/5">
              <div className="flex-1">
                <div className="font-medium">{d.title || "(Sem título)"}</div>
                <div className="text-xs opacity-70">
                  {d.language || "unknown"} · {d.function || "—"} · {fmtDate(d.createdAt)}
                </div>
              </div>
              <a href={`/docs/${getId(d)}`}
                 className="text-xs px-2 py-1 bg-white/10 hover:bg-white/20 rounded">Abrir</a>
            </div>
          ))}
          {items.length === 0 && <div className="p-6 text-center text-sm opacity-60">Nada encontrado.</div>}
        </div>

        <div className="flex justify-between items-center">
          <div>Página {page+1} / {Math.max(1, Math.ceil(total/limit))}</div>
          <div className="flex items-center gap-2">
            <button disabled={page===0} onClick={()=>setPage(p=>Math.max(0,p-1))}
                    className="px-3 py-1 rounded bg-white/10 disabled:opacity-30">Anterior</button>
            <button disabled={(page+1)*limit>=total} onClick={()=>setPage(p=>p+1)}
                    className="px-3 py-1 rounded bg-white/10 disabled:opacity-30">Próxima</button>
          </div>
        </div>
      </div>
    </div>
  );
}