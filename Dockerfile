FROM microsoft/dotnet:2.2-sdk as builder
WORKDIR /src

COPY . .
RUN dotnet publish -c Release -o /published SkiaBlur.Api/SkiaBlur.Api.csproj


FROM microsoft/dotnet:2.2-aspnetcore-runtime as runner
RUN apt-get update && apt-get install -y libfontconfig1


FROM runner
WORKDIR /app
COPY --from=builder /published .

ENTRYPOINT ["dotnet", "SkiaBlur.Api.dll"]
