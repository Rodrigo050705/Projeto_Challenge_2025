// Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
// dotnet run --launch-profile http
// src/main.jsx
import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider, Navigate } from 'react-router-dom'
import { AppShell } from './App.jsx'
import Login from './pages/Login.jsx'
import Docs from './pages/Docs.jsx'
import DocView from './pages/DocView.jsx'
import './index.css'
import Refactor from "./pages/Refactor.jsx"
import Analytics from "./pages/Analytics.jsx"
import CodeUnderstanding from "./pages/CodeUnderstanding.jsx"

// rota de logout: limpa sessão e volta ao login
function Logout(){
  try {
    localStorage.removeItem('onboarded');
localStorage.removeItem('userEmail');
  } catch (e) {}
  return <Navigate to="/login" replace />;
}



// Helpers avaliados em tempo de render
const isOnboarded = () => localStorage.getItem('onboarded') === 'true';

const RequireAuth = ({ children }) => (
  isOnboarded() ? children : <Navigate to="/login" replace />
);

const RequireGuest = ({ children }) => (
  children
);

function HomeRedirect(){ return <Navigate to="/login" replace />; }

const router = createBrowserRouter([
  { path: "/refactor", element: <Refactor /> },
  { path: "/analytics", element: <Analytics /> },
  { path: "/code-understanding", element: <CodeUnderstanding /> },
{ path: '/logout', element: <Logout /> },
  { path: '/', element: <HomeRedirect /> },
  { path: '/login', element: <RequireGuest><Login /></RequireGuest> },
  { path: '/app', element: <RequireAuth><AppShell /></RequireAuth> },
  { path: '/docs', element: <RequireAuth><Docs /></RequireAuth> },
  { path: '/docs/:id', element: <RequireAuth><DocView /></RequireAuth> },
]);

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
)
