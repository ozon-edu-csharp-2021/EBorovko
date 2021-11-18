#!/bin/bash

set -e
run_cmd="dotnet Ozon.MerchandiseService.Api.dll --no-build -v d"

dotnet Ozon.MerchandiseService.Migrator.dll --no-build -v d -- --dryrun

dotnet Ozon.MerchandiseService.Migrator.dll --no-build -v d

>&2 echo "MerchandiseService DB Migrations complete, starting app."
>&2 echo "Run MerchandiseService: $run_cmd"
exec $run_cmd