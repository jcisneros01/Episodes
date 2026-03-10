#!/usr/bin/env bash
set -e

cleanup() {
  echo ""
  echo "Shutting down..."
  kill 0 2>/dev/null
  wait 2>/dev/null
  echo "Done."
}

trap cleanup EXIT INT TERM

cd "$(dirname "$0")"

if ! docker info &>/dev/null; then
  echo "Starting Docker Desktop..."
  open -a Docker
  while ! docker info &>/dev/null; do
    sleep 1
  done
  echo "Docker Desktop is ready."
fi

echo "Starting Postgres..."
docker compose -f liquibase/docker-compose.yml up -d db

echo "Starting React dev server..."
cd react-web && npm run dev &

wait
