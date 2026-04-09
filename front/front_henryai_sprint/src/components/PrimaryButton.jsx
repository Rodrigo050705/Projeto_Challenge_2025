// src/components/PrimaryButton.jsx
import React from "react";
import { cx } from "./UI";

export default function PrimaryButton({ children, disabled, onClick, className }) {
  return (
    <button
      type="button"
      disabled={disabled}
      onClick={onClick}
      className={cx(
        "w-full py-3 rounded-md font-semibold text-white bg-sky-500 transition enabled:hover:bg-sky-600 disabled:opacity-40 disabled:cursor-not-allowed disabled:pointer-events-none",
        className
      )}
    >
      {children}
    </button>
  );
}
