# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Versions.props Foundation.slnx ./

COPY --parents src/*/*.csproj tests/*/*.csproj ./

RUN dotnet restore Foundation.slnx

RUN dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.*

ENV PATH="$PATH:/root/.dotnet/tools"

COPY src/ src/
COPY tests/ tests/

COPY <<EOF run-tests.sh
#!/usr/bin/env bash
set -euo pipefail

packages=(Core AspNetCore EntityFrameworkCore OpenTelemetry)
results_dir=test-results

for package in "\${packages[@]}"; do
    dotnet test "tests/Tests.\$package" --configuration Release \\
        --results-directory "\$results_dir/\$package" \\
        --coverage --coverage-output-format cobertura \\
        --coverage-output "\$package.cobertura.xml"
done

reportgenerator \\
    "-reports:\$results_dir/*/*.cobertura.xml" \\
    "-targetdir:\$results_dir/report" \\
    "-reporttypes:Html;TextSummary" \\
    "-classfilters:-Microsoft.AspNetCore.OpenApi.Generated;-System.Runtime.CompilerServices"

cat "\$results_dir/report/Summary.txt"
EOF

RUN chmod +x run-tests.sh

FROM build AS test

ENTRYPOINT ["./run-tests.sh"]
