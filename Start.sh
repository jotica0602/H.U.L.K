#!/bin/bash
if [[ $DISPLAY ]]; then
  terminal
else
  dotnet run --project App
fi
