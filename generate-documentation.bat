dotnet publish .\src\GraphQL.Client.Extensions\GraphQL.Client.Extensions.csproj -c Release -o out
dotnet tool run xmldoc2md .\out\GraphQL.Client.Extensions.dll .\docs\api
