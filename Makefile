.PHONY: help prepare format lint test start

ifneq (,$(wildcard .env))
include .env
endif

help:
	@echo "Usage:"
	@echo "  make start     Start the development server"
	@echo "  make prepare   Restore dependencies"
	@echo "  make format    Format code"
	@echo "  make lint      Check code formatting"
	@echo "  make test      Run tests"

prepare:
	dotnet restore example-auth-dotnet.slnx

format:
	dotnet format example-auth-dotnet.slnx

lint:
	dotnet format example-auth-dotnet.slnx --verify-no-changes

test:
	dotnet test example-auth-dotnet.slnx

start:
	dotnet restore example-auth-dotnet.slnx
	dotnet run
