// src/pages/Login.jsx
import React from "react";
import logo from "../assets/logo.png";

const cx = (...c) => c.filter(Boolean).join(" ");

function PrimaryButton({ children, disabled, onClick, className }) {
  return (
    <button
      type="button"
      disabled={disabled}
      onClick={onClick}
      className={cx(
        "w-full py-3 rounded-lg font-semibold text-white bg-sky-600 transition",
        "enabled:hover:bg-sky-500",
        "disabled:opacity-40 disabled:cursor-not-allowed disabled:pointer-events-none",
        className
      )}
    >
      {children}
    </button>
  );
}

export default function Login() {
  const [step, setStep] = React.useState(1);
  const [email, setEmail] = React.useState("");
  const [sector, setSector] = React.useState("");
  const [invites, setInvites] = React.useState("");

  const validEmail = /.+@.+\..+/.test(email);
  const canNext1 = validEmail;
  const canNext2 = validEmail && sector.length > 0;

  const finish = (payload) => {
    // aqui você pode chamar sua API; se OK:
    localStorage.setItem("onboarded", "true");
    localStorage.setItem("userEmail", payload.email || email);
    window.location.href = "/app";
  };

  return (
    <div className="min-h-screen grid place-items-center bg-gray-50 dark:bg-neutral-950">
      <div className="w-full max-w-2xl rounded-2xl border bg-neutral-950 text-white shadow-xl p-10 border-gray-800">
        {/* header (igual ao mock) */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center gap-2 text-sky-300 font-semibold"><img src={logo} alt="Henry.AI" className="h-20 object-contain" />
          </div>
          <h2 className="text-3xl md:text-4xl font-extrabold mt-3">
            {step === 3 ? "Estamos quase lá!" : "Iniciando seu registro..."}
          </h2>
        </div>

        {/* PASSO 1 — email */}
        {step === 1 && (
          <div className="space-y-4">
            <label className="block text-sm text-gray-300">
              Digite seu e-mail corporativo:
            </label>
            <input
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="meuemail@ford.com"
              type="email"
              className="w-full bg-neutral-900 border border-gray-700 rounded-lg px-3 py-3 outline-none focus:border-sky-400"
            />
            <PrimaryButton
              disabled={!canNext1}
              onClick={() => canNext1 && setStep(2)}
              className="mt-2"
            >
              Próximo
            </PrimaryButton>
          </div>
        )}

        {/* PASSO 2 — confirmar email + setor */}
        {step === 2 && (
          <div className="space-y-4">
            <div>
              <label className="block text-sm text-gray-300">
                Digite seu e-mail corporativo:
              </label>
              <input
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="meuemail@ford.com"
                type="email"
                className="w-full bg-neutral-900 border border-gray-700 rounded-lg px-3 py-3 outline-none focus:border-sky-400"
              />
            </div>

            <div>
              <label className="block text-sm text-gray-300">
                Selecione seu setor:
              </label>
              <select
                value={sector}
                onChange={(e) => setSector(e.target.value)}
                className="w-full bg-neutral-900 border border-gray-700 rounded-lg px-3 py-3 outline-none focus:border-sky-400"
              >
                <option value="">Selecione um setor de desenvolvimento da Ford</option>
                <option>Powertrain</option>
                <option>Telemática</option>
                <option>Manufatura Digital</option>
                <option>Financeiro TI</option>
              </select>
            </div>

            <PrimaryButton
              disabled={!canNext2}
              onClick={() => canNext2 && setStep(3)}
              className="mt-2"
            >
              Próximo
            </PrimaryButton>
          </div>
        )}

        {/* PASSO 3 — convites (opcional) */}
        {step === 3 && (
          <div className="space-y-4">
            <p className="text-gray-300 text-sm">
              É a primeira vez que alguém do seu setor entra na plataforma!
              Gostaria de convidar colaboradores?
            </p>

            <input
              value={invites}
              onChange={(e) => setInvites(e.target.value)}
              placeholder="email@ford.com, email2@ford.com"
              type="text"
              className="w-full bg-neutral-900 border border-gray-700 rounded-lg px-3 py-3 outline-none focus:border-sky-400"
            />

            <div className="flex gap-3">
              <button
                type="button"
                onClick={() =>
                  finish({ email, sector, invites, invited: false })
                }
                className="flex-1 py-3 rounded-lg font-semibold bg-gray-300 text-gray-900 hover:bg-gray-200"
              >
                Pular
              </button>
              <PrimaryButton
                disabled={false}
                onClick={() =>
                  finish({ email, sector, invites, invited: true })
                }
                className="flex-1"
              >
                Próximo
              </PrimaryButton>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}