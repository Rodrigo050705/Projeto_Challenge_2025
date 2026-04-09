# Henry.AI – Backend (Core + Agent) com MongoDB Atlas

## Variáveis de ambiente obrigatórias (Core.Host)
- `MONGODB_URI` — **use a string SRV do Atlas** (`mongodb+srv://<user>:<pass>@<cluster>/?retryWrites=true&w=majority&appName=Cluster0`)
- `MONGODB_DB` — nome do banco (ex.: `henry_ai`)
- `URL_AGENT` — URL do Agent.Host (ex.: `http://localhost:5001`)

> A senha deve estar *URL-encoded*. Ex.: se tiver `@` use `%40`.

### Teste rápido
```bash
cd Henry.AI.Core/Henry.AI.Core.Host
dotnet run --launch-profile http
# em outra aba
curl "http://localhost:5000/healthz"
```

## Fluxo
1. Front envia o **código cru** para `POST /documentation/rawcode` (Core.Host).
2. Core chama o Agent (`URL_AGENT`) para gerar a documentação.
3. Core salva a documentação no Mongo Atlas (coleção `documentation`).
4. Front lista em `/documentation` e exibe/edita via `/documentation/{id}`.
