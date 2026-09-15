.PHONY: test test-local report clean

test:
	docker compose run --build --rm tests

# Excludes Postgres-backed integration tests (they need Docker reachable via the Windows
# named pipe, which Testcontainers can't use from here) and ignores exit code 8 (Messaging
# has no tests yet, which Microsoft.Testing.Platform treats as a failure on its own).
# `make test` runs the full, unfiltered suite inside Docker instead.
test-local:
	dotnet test Foundation.slnx --configuration Release --results-directory test-results -- \
		--filter-not-trait "Category=Integration" --ignore-exit-code 8

report:
	cmd.exe /c start test-results/report/index.html

clean:
	rm -rf test-results
