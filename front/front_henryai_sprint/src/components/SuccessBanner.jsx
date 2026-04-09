import React from "react";

export default function SuccessBanner({ show, onClose, children }) {
  if (!show) return null;
  return (
    <div className="fixed bottom-4 right-4 z-50 max-w-md">
      <div className="flex items-start gap-3 rounded-lg border border-green-600 bg-green-50 px-4 py-3 text-green-800 shadow-lg">
        <span className="mt-0.5 inline-block h-2 w-2 shrink-0 rounded-full bg-green-600" />
        <div className="text-sm leading-5">{children}</div>
        <button
          className="ml-2 text-green-800/70 hover:text-green-900"
          onClick={onClose}
          aria-label="Fechar"
          title="Fechar"
        >
          ✕
        </button>
      </div>
    </div>
  );
}
