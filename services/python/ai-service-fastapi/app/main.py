from fastapi import FastAPI

app = FastAPI(title="SaptariX AI Service")


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}
