from fastapi import FastAPI
import asyncio

app = FastAPI(title="CSharp-Cron-Manager Service", version="2.0.0")

@app.get("/health")
def health():
    return {"status": "ok", "service": "CSharp-Cron-Manager"}

@app.post("/api/v1/execute")
async def execute_task(payload: dict):
    await asyncio.sleep(0.1) # simulate work
    return {"status": "success", "domain": "manager", "result": payload}
