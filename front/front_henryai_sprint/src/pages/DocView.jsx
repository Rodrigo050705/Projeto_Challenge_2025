import { Pencil, Trash2, Save, X } from 'lucide-react';
import React from "react";
import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";

export default function DocView() {
  const { id } = useParams();
  const [doc, setDoc] = useState(null);
  const [editing, setEditing] = useState(false);
  const [busy, setBusy] = useState(false);
  const base = import.meta.env.VITE_API_URL || "http://localhost:5000";

  useEffect(() => {
    (async () => {
      const res = await fetch(`${base}/documentation/${id}`);
      const data = await res.json().catch(() => null);
      setDoc(data);
    })();
  }, [id]);

  const content = (doc?.content) || doc?.documentedCode || "";

  async function removeDoc() {
    if (!confirm("Excluir esta documentação?")) return;
    setBusy(true);
    await fetch(`${base}/documentation/${id}`, { method: "DELETE", cache: 'no-store' });
    window.location.href = "/docs";
  }

  async function save() {
    setBusy(true);
    const payload = {
      title: doc.title,
      language: doc.language,
      function: doc.function,
      content: doc.content || doc.documentedCode || "",
    };
    await fetch(`${base}/documentation/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ ...payload, documentedCode: payload.content }),
      cache: 'no-store',
    });
    setBusy(false);
    setEditing(false);
  }

  return (
    <div className="min-h-screen bg-neutral-900 text-neutral-200 p-4">
      <div className="max-w-4xl mx-auto space-y-4">
        <div className="flex items-center justify-between">
          <h1 className="text-xl font-semibold">Documento</h1>

          <div className="flex items-center gap-2">
            {!editing && (
              <button
                onClick={() => setEditing(true)}
                className="text-xs px-2 py-1 rounded border border-neutral-700 hover:bg-white/10 flex items-center gap-1"
              >
                <Pencil size={14} /> Editar
              </button>
            )}
            <button
              onClick={removeDoc}
              disabled={busy}
              className="text-xs px-2 py-1 rounded border border-red-700/50 text-red-300 hover:bg-red-900/20 flex items-center gap-1"
            >
              <Trash2 size={14} /> Excluir
            </button>
            <Link
              to="/docs"
              className="text-xs px-2 py-1 bg-white/10 hover:bg-white/20 rounded"
            >
              Voltar
            </Link>
            <a
              href={`${base}/documentation/${id}/pdf`}
              target="_blank"
              rel="noreferrer"
              className="text-xs px-2 py-1 rounded border border-neutral-700 hover:bg-white/10"
            >
              Exportar PDF
            </a>
          </div>
        </div>

        {doc ? (
          <div className="space-y-2">
            <div className="text-sm opacity-70">
              {doc.language || "unknown"} · {doc.function || "—"}
            </div>
            <div className="text-lg font-medium">{doc.title || "(Sem título)"}</div>
            {!editing ? (
              <pre className="whitespace-pre-wrap bg-white/5 p-3 rounded">
                {content}
              </pre>
            ) : (
              <div className="space-y-2">
                <input
                  value={doc?.title || ""}
                  onChange={(e) => setDoc({ ...doc, title: e.target.value })}
                  className="w-full px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none"
                />
                <div className="grid md:grid-cols-2 gap-2">
                  <input
                    value={doc?.language || ""}
                    onChange={(e) => setDoc({ ...doc, language: e.target.value })}
                    className="px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none"
                  />
                  <input
                    value={doc?.function || ""}
                    onChange={(e) => setDoc({ ...doc, function: e.target.value })}
                    className="px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none"
                  />
                </div>
                <textarea
                  value={doc?.content || doc?.documentedCode || ""}
                  onChange={(e) => setDoc({ ...doc, content: e.target.value })}
                  rows={14}
                  className="w-full px-3 py-2 rounded bg-neutral-800 border border-neutral-700 outline-none resize-y"
                />
                <div className="flex items-center justify-end gap-2">
                  <button
                    onClick={() => setEditing(false)}
                    className="px-3 py-2 rounded-lg border border-neutral-700 hover:bg-white/5"
                  >
                    <X size={16} /> Cancelar
                  </button>
                  <button
                    onClick={save}
                    disabled={busy}
                    className="px-3 py-2 rounded-lg border border-sky-700 bg-sky-900/30 hover:bg-sky-900/50 text-sky-200 flex items-center gap-2"
                  >
                    <Save size={16} /> Salvar
                  </button>
                </div>
              </div>
            )}
          </div>
        ) : (
          <div className="opacity-60">Carregando…</div>
        )}
      </div>
    </div>
  );
}
