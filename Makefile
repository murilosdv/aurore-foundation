.PHONY: test test-local open clean

test:
	docker compose run --build --rm tests

test-local:
	dotnet test Foundation.slnx --configuration Release --results-directory test-results

report:
	cmd.exe /c start test-results/report/index.html

clean:
	rm -rf test-results
