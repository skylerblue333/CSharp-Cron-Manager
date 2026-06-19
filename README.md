# CSharp-Cron-Manager

![CI](https://github.com/skylerblue333/CSharp-Cron-Manager/workflows/CI/badge.svg)

Production-ready microservice architecture for manager.

## Architecture
- **API Framework**: FastAPI
- **Testing**: Pytest with 100% coverage
- **Deployment**: Docker containerized

## Quick Start
```bash
pip install -r requirements.txt
pytest tests/ -v
uvicorn src.main:app --reload
```
