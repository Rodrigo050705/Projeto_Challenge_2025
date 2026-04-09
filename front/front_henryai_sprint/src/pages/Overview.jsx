import React from "react";
import { Card } from "../components/UI";
const Pill = ({children}) => <span className="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-700 border border-gray-200 dark:bg-white/10 dark:text-gray-200 dark:border-gray-700">{children}</span>;
export default function Overview() {
  return (
    <div className="grid md:grid-cols-3 gap-6">
      <Card title="Hotspots">
        <ul className="text-sm space-y-2">
          <li className="flex items-center justify-between"><span>legacy/financas/NotaFiscal.prw</span><Pill>complexity 14</Pill></li>
          <li className="flex items-center justify-between"><span>legacy/vb6/frmPedidos.frm</span><Pill>io-bound</Pill></li>
          <li className="flex items-center justify-between"><span>legacy/delphi/DM.vcl</span><Pill>db heavy</Pill></li>
        </ul>
      </Card>
      <Card title="Coverage">
        <div className="text-4xl font-extrabold">32%<span className="text-base font-medium text-gray-400"> tests</span></div>
        <div className="text-xs text-gray-500 mt-2">Target this sprint: 50%</div>
      </Card>
      <Card title="Open PRs" right={<a className="text-xs text-blue-400">view all</a>}>
        <div className="space-y-3 text-sm">
          <div className="flex items-center justify-between"><span>refactor: normalize date utils</span><span className="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-700 border border-gray-200 dark:bg-white/10 dark:text-gray-200 dark:border-gray-700">#112</span></div>
          <div className="flex items-center justify-between"><span>migrate: ADO → EF Core</span><span className="text-xs px-2 py-1 rounded-full bg-gray-100 text-gray-700 border border-gray-200 dark:bg-white/10 dark:text-gray-200 dark:border-gray-700">#109</span></div>
        </div>
      </Card>
    </div>
  );
}
