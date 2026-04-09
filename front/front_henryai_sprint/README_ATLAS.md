# Henry.AI – Frontend

Defina a URL do backend (Core.Host):
```
cp VITE.env.sample .env
# edite se necessário
```
Execute:
```
npm i
npm run dev
```
O chat envia texto para `POST /documentation/rawcode` e a aba **Documentações** lê da API `/documentation`.
