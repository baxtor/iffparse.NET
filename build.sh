#!/bin/bash
export encoding=utf-8

dotnet restore src/iffparse.NET.sln
dotnet build src/iffparse.NET.sln --configuration Release
