// src/components/UI.jsx
import React from "react";

// Navegação/Logout
import { useNavigate } from 'react-router-dom';import { Search, Menu, LogOut } from "lucide-react";

export const cx = (...c) => c.filter(Boolean).join(" ");

export const Pill = ({ children }) => (
  <span className="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-700 border border-gray-200 dark:bg-white/10 dark:text-gray-200 dark:border-gray-700">
    {children}
  </span>
);

export const NavItem = ({ icon: Icon, label, active, onClick }) => (
  <button
    onClick={onClick}
    aria-selected={active}
    className={cx(
      "group relative flex items-center gap-3 w-full text-left px-4 py-2 rounded-xl transition font-medium border",
      active
        ? "bg-white text-gray-900 border-gray-900 shadow-sm dark:bg-white dark:text-gray-900 dark:border-gray-900"
        : "border-transparent hover:bg-gray-100 text-gray-700 dark:hover:bg-white/5 dark:text-gray-200"
    )}
  >
    <Icon size={18} className={active ? "text-gray-900" : "text-gray-500 group-hover:text-gray-700 dark:group-hover:text-gray-300"} />
    <span className="flex-1">{label}</span>
  </button>
);

export const Card = ({ title, children, right }) => (
  <div className="bg-white dark:bg-neutral-900 rounded-2xl shadow-sm border border-gray-100 dark:border-gray-800 p-5">
    <div className="flex items-center justify-between mb-3">
      <h3 className="text-sm font-semibold text-gray-700 dark:text-gray-300">{title}</h3>
      {right}
    </div>
    {children}
  </div>
);

export const TopBar = ({ project, setProject, onToggleTheme, isDark, onLogout }) => (
  <div className="h-16 border-b border-gray-100 dark:border-gray-800 bg-white/80 dark:bg-neutral-950/80 backdrop-blur sticky top-0 z-10">
    <div className="h-full px-4 flex items-center justify-between">
      <div className="flex items-center gap-3 text-gray-900 dark:text-gray-100">
        <Menu />
        <span className="font-semibold">Legacy AI</span>
        <span className="text-gray-300 dark:text-gray-600">/</span>
        <select
          className="text-sm bg-gray-100 dark:bg-neutral-900 border border-gray-200 dark:border-gray-700 rounded-lg px-2 py-1"
          value={project}
          onChange={(e) => setProject(e.target.value)}
        >
          <option>pilot-legacy-repo</option>
          <option>ecommerce-vb6</option>
          <option>advpl-financeiro</option>
        </select>
      </div>
      <div className="flex items-center gap-2">
        <div className="flex items-center bg-gray-100 dark:bg-neutral-900 rounded-xl px-3 border border-gray-200 dark:border-gray-700">
          <Search size={16} className="text-gray-400" />
          <input placeholder="Search files, symbols, docs…" className="bg-transparent text-sm px-2 py-2 outline-none text-gray-900 dark:text-gray-100" />
        </div>
        <span className="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-700 border border-gray-200 dark:bg-white/10 dark:text-gray-200 dark:border-gray-700">beta</span>
        <button
          onClick={onToggleTheme}
          className="ml-2 px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-neutral-900 text-gray-700 dark:text-gray-200"
        >
          {isDark ? "☀️" : "🌙"}
        </button>
        <button
          onClick={onLogout}
          className="ml-2 px-3 py-2 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-neutral-900 text-gray-700 dark:text-gray-200 flex items-center gap-2"
          title="Sair"
        >
          <LogOut size={16} /> <span className="hidden sm:inline">Sair</span>
        </button>
      </div>
    </div>
  </div>
);
