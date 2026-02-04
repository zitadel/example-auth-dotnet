.PHONY: prepare format lint test start

prepare:
	dotnet restore

format:
	dotnet format

lint:
	dotnet format --verify-no-changes

test:
	dotnet test

start:
	dotnet run
