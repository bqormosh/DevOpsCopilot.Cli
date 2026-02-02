# DevOps Copilot CLI (Semantic Kernel + Azure OpenAI)

A chat-driven DevOps assistant built with **Microsoft Semantic Kernel** and **Azure OpenAI**.  
It reads build logs, produces structured analysis, and can perform DevOps actions (deploy, branch creation) via tool-calling — with guardrails like **production approval** and **prompt redaction**.

## What this project demonstrates

- **Semantic Kernel tool calling** (functions exposed as plugins)
- **Chat-only UX** (no slash commands; natural language only)
- **Versioned prompts** stored as **YAML + Handlebars**
- **Guardrails**
  - Production deployment requires explicit user approval (Function Invocation Filter)
  - Prompts are redacted to reduce risk of secrets leaking (Prompt Render Filter)


---

## Features

### 1) Build log analysis → safe staging deploy
The assistant:
1. Reads the latest build log.
2. Analyzes it using a structured YAML prompt.
3. Deploys to staging only if the analysis indicates it’s safe.

### 2) Branch creation
The assistant can parse a natural-language branch request (branch name + base branch).  
If details are missing, it asks one short follow-up question.

### 3) Production deployment approval gate
Any call to production deployment is blocked unless the user explicitly approves.

---

## Tech stack

- .NET (Console App)
- Microsoft Semantic Kernel
- Azure OpenAI (Chat Completion)
- YAML prompt functions + Handlebars templating
- Function Invocation Filters + Prompt Render Filters

---

## Repository structure

```
DevOpsCopilot.Cli/
  Files/
    build.log
  Prompts/
    AnalyzeBuildLog.yaml
    ParseBranchRequest.yaml
  Sessions/
  Program.cs
  ...
```

- `Files/build.log` is a sample build log used for local testing.
- `Prompts/*.yaml` contains versioned prompt templates.
- `Sessions/` stores chat transcripts (saved locally).

---

## Getting started

### Prerequisites
- Visual Studio 2022 (or .NET SDK installed)
- Azure OpenAI resource + a chat deployment

### 1) Clone the repo
```bash
git clone <your-repo-url>
cd DevOpsCopilot.Cli
```

### 2) Install dependencies
Restore packages:
```bash
dotnet restore
```

### 3) Configure Azure OpenAI using User Secrets (recommended)
This repo does **not** store secrets in `appsettings.json`.

Initialize user secrets (only needed once):
```bash
dotnet user-secrets init
```

Set values:
```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://YOUR-RESOURCE.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_KEY"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "YOUR_DEPLOYMENT"
```

### 4) Run
```bash
dotnet run
```

> Note: On some managed Windows environments, launching the generated `.exe` may be blocked by policy.
> Running via `dotnet run` (or `dotnet <dll>`) typically works.

---

## Example prompts you can try

### Analyze and summarize the build log
- “Analyze the latest build log and summarize the top issues.”

### Deploy to staging (safe-guarded)
- “If the build log looks clean, deploy to staging.”

### Create a branch
- “Create a branch called `hotfix/login-timeout` from `main`.”

### Deploy to production (approval required)
- “Deploy to production.”
You will be prompted to approve before the action is executed.

---

## Configuration

### appsettings.json
`appsettings.json` is committed with placeholders only (no secrets).  
Runtime config is loaded in this order (later wins):
1. `appsettings.json`
2. User Secrets
3. Environment variables

### Environment variables (CI/CD)
For pipelines, you can use environment variables (note the `__`):
- `AzureOpenAI__Endpoint`
- `AzureOpenAI__ApiKey`
- `AzureOpenAI__DeploymentName`

---

## Safety & guardrails

This project includes defensive controls:
- Production deploy requires explicit approval (hard-blocked).
- Prompt redaction reduces risk of sending sensitive strings to the model.
- Build log reading is sandboxed to prevent accessing unexpected file paths.

---

## Roadmap ideas

- Replace simulated DevOps actions with real integrations (Azure DevOps / GitHub APIs)
- Add structured telemetry (OpenTelemetry) for tool calls and latency
- Add “policy-as-code” for deployment gates (staging/prod rules)
- Add unit tests around prompt JSON parsing and filters
